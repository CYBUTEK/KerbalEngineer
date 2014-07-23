using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

using KerbalEngineer.Flight;

using UnityEngine;

namespace KerbalEngineer.VesselSimulator
{
    public class SimManager: IUpdatable, IUpdateRequest
    {
        public static SimManager Instance = new SimManager();
        public const double RESOURCE_MIN = 0.0001;
        
        private static bool bRequested = false;
        private static bool bRunning = false;
        private static Stopwatch timer = new Stopwatch();
        private static long delayBetweenSims = 0;

        public static Stage[] Stages { get; private set; }
        public static Stage LastStage { get; private set; }
        public static String failMessage { get; private set; }

        public static bool dumpTree = false;
        public static bool logOutput = false;
        public static bool vectoredThrust = false;
        public static long minSimTime = 150;
        public static double Gravity { get; set; }
        public static double Atmosphere { get; set; }
        public static double Velocity { get; set; }

        // Support for RealFuels using reflection to check localCorrectThrust without dependency
        private static bool hasCheckedForRealFuels = false;
        private static bool hasInstalledRealFuels = false;

        private static System.Reflection.FieldInfo RF_ModuleEngineConfigs_locaCorrectThrust = null;
        private static System.Reflection.FieldInfo RF_ModuleHybridEngine_locaCorrectThrust = null;
        private static System.Reflection.FieldInfo RF_ModuleHybridEngines_locaCorrectThrust = null;

        private static void GetRealFuelsTypes()
        {
			hasCheckedForRealFuels = true;

			foreach (AssemblyLoader.LoadedAssembly assembly in AssemblyLoader.loadedAssemblies)
            {
                MonoBehaviour.print("Assembly:" + assembly.assembly.ToString());

                if (assembly.assembly.ToString().Split(',')[0] == "RealFuels")
                {
                    MonoBehaviour.print("Found RealFuels mod");

                    Type RF_ModuleEngineConfigs_Type = assembly.assembly.GetType("RealFuels.ModuleEngineConfigs");
                    if (RF_ModuleEngineConfigs_Type != null)
                        RF_ModuleEngineConfigs_locaCorrectThrust = RF_ModuleEngineConfigs_Type.GetField("localCorrectThrust");

                    Type RF_ModuleHybridEngine_Type = assembly.assembly.GetType("RealFuels.ModuleHybridEngine");
                    if (RF_ModuleHybridEngine_Type != null)
                        RF_ModuleHybridEngine_locaCorrectThrust = RF_ModuleHybridEngine_Type.GetField("localCorrectThrust");

                    Type RF_ModuleHybridEngines_Type = assembly.assembly.GetType("RealFuels.ModuleHybridEngines");
                    if (RF_ModuleHybridEngines_Type != null)
                        RF_ModuleHybridEngines_locaCorrectThrust = RF_ModuleHybridEngines_Type.GetField("localCorrectThrust");

					hasInstalledRealFuels = true;
					break;
				}

			}

		}

        public static bool DoesEngineUseCorrectedThrust(Part theEngine)
        {
            if (!hasInstalledRealFuels /*|| HighLogic.LoadedSceneIsFlight*/)
                return false;

            // Look for any of the Real Fuels engine modules and call the relevant method to find out
            if (RF_ModuleEngineConfigs_locaCorrectThrust != null && theEngine.Modules.Contains("ModuleEngineConfigs"))
            {
                PartModule modEngineConfigs = theEngine.Modules["ModuleEngineConfigs"];
                if (modEngineConfigs != null)
                {
                    // Check the localCorrectThrust
                    if ((bool)RF_ModuleEngineConfigs_locaCorrectThrust.GetValue(modEngineConfigs))
                        return true;
                }
            }

            if (RF_ModuleHybridEngine_locaCorrectThrust != null && theEngine.Modules.Contains("ModuleHybridEngine"))
            {
                PartModule modHybridEngine = theEngine.Modules["ModuleHybridEngine"];
                if (modHybridEngine != null)
                {
                    // Check the localCorrectThrust
                    if ((bool)RF_ModuleHybridEngine_locaCorrectThrust.GetValue(modHybridEngine))
                        return true;
                }
            }

            if (RF_ModuleHybridEngines_locaCorrectThrust != null && theEngine.Modules.Contains("ModuleHybridEngines"))
            {
                PartModule modHybridEngines = theEngine.Modules["ModuleHybridEngines"];
                if (modHybridEngines != null)
                {
                    // Check the localCorrectThrust
                    if ((bool)RF_ModuleHybridEngines_locaCorrectThrust.GetValue(modHybridEngines))
                        return true;
                }
            }

            return false;
        }


        public static void RequestSimulation()
        {
            if (!hasCheckedForRealFuels)
                GetRealFuelsTypes();

            bRequested = true;
            if (!timer.IsRunning)
                timer.Start();
        }

        public static void TryStartSimulation()
        {
            if (bRequested && !bRunning && (HighLogic.LoadedSceneIsEditor || FlightGlobals.ActiveVessel != null) && timer.ElapsedMilliseconds > delayBetweenSims)
            {
                bRequested = false;
                timer.Reset();
                StartSimulation();
            }
        }

        public static bool ResultsReady()
        {
            return !bRunning;
        }

        private static void ClearResults()
        {
            failMessage = "";
            Stages = null;
            LastStage = null;
        }

        private static void StartSimulation()
        {
            try
            {
                bRunning = true;
                ClearResults();
                timer.Start();

                List<Part> parts = HighLogic.LoadedSceneIsEditor ? EditorLogic.SortedShipList : FlightGlobals.ActiveVessel.Parts;

                // Create the Simulation object in this thread
                Simulation sim = new Simulation();

                // This call doesn't ever fail at the moment but we'll check and return a sensible error for display
                if (sim.PrepareSimulation(parts, Gravity, Atmosphere, Velocity, dumpTree, vectoredThrust))
                {
                    ThreadPool.QueueUserWorkItem(new WaitCallback(RunSimulation), sim);
                }
                else
                {
                    failMessage = "PrepareSimulation failed";
                    bRunning = false;
                    logOutput = false;
                }
            }
            catch (Exception e)
            {
                MonoBehaviour.print("Exception in StartSimulation: " + e);
                failMessage = e.ToString();
                bRunning = false;
                logOutput = false;
            }
            dumpTree = false;
        }

        private static void RunSimulation(object simObject)
        {
            try
            {
                Stages = (simObject as Simulation).RunSimulation();
                if (Stages != null)
                {
                    if (logOutput)
                    {
                        foreach (Stage stage in Stages)
                            stage.Dump();
                    }
                    LastStage = Stages.Last();
                }
            }
            catch (Exception e)
            {
                MonoBehaviour.print("Exception in RunSimulation: " + e);
                Stages = null;
                LastStage = null;
                failMessage = e.ToString();
            }

            timer.Stop();
#if TIMERS
            MonoBehaviour.print("Total simulation time: " + timer.ElapsedMilliseconds + "ms");
#else
            if (logOutput)
                MonoBehaviour.print("Total simulation time: " + timer.ElapsedMilliseconds + "ms");
#endif
            delayBetweenSims = minSimTime - timer.ElapsedMilliseconds;
            if (delayBetweenSims < 0)
                delayBetweenSims = 0;

            timer.Reset();
            timer.Start();

            bRunning = false;
            logOutput = false;
        }

        public static String GetVesselTypeString(VesselType vesselType)
        {
            switch (vesselType)
            {
                case VesselType.Debris:
                    return "Debris";
                case VesselType.SpaceObject:
                    return "SpaceObject";
                case VesselType.Unknown:
                    return "Unknown";
                case VesselType.Probe:
                    return "Probe";
                case VesselType.Rover:
                    return "Rover";
                case VesselType.Lander:
                    return "Lander";
                case VesselType.Ship:
                    return "Ship";
                case VesselType.Station:
                    return "Station";
                case VesselType.Base:
                    return "Base";
                case VesselType.EVA:
                    return "EVA";
                case VesselType.Flag:
                    return "Flag";
            }
            return "Undefined";
        }

        public void Update()
        {
            TryStartSimulation();
        }

        public bool UpdateRequested { get; set; }

        public static void RequestUpdate()
        {
            RequestSimulation();
            Instance.UpdateRequested = true;
        }
    }
}
