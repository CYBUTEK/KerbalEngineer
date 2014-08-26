// 
//     Kerbal Engineer Redux
// 
//     Copyright (C) 2014 CYBUTEK
// 
//     This program is free software: you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     This program is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General Public License for more details.
// 
//     You should have received a copy of the GNU General Public License
//     along with this program.  If not, see <http://www.gnu.org/licenses/>.
// 

#region Using Directives

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;

using UnityEngine;

#endregion

namespace KerbalEngineer.VesselSimulator
{
    public class SimManager
    {
        public const double RESOURCE_MIN = 0.0001;

        private static bool bRequested;
        private static bool bRunning;
        private static readonly Stopwatch timer = new Stopwatch();
        private static long delayBetweenSims;

        public static bool dumpTree = false;
        public static bool logOutput = false;
        public static bool vectoredThrust = false;
        public static long minSimTime = 150;

        // Support for RealFuels using reflection to check localCorrectThrust without dependency
        private static bool hasCheckedForRealFuels;
        private static bool hasInstalledRealFuels;

        private static FieldInfo RF_ModuleEngineConfigs_locaCorrectThrust;
        private static FieldInfo RF_ModuleHybridEngine_locaCorrectThrust;
        private static FieldInfo RF_ModuleHybridEngines_locaCorrectThrust;
        public static Stage[] Stages { get; private set; }
        public static Stage LastStage { get; private set; }
        public static String failMessage { get; private set; }
        public static double Gravity { get; set; }
        public static double Atmosphere { get; set; }
        public static double Velocity { get; set; }

        private static void GetRealFuelsTypes()
        {
            hasCheckedForRealFuels = true;

            foreach (AssemblyLoader.LoadedAssembly assembly in AssemblyLoader.loadedAssemblies)
            {
                MonoBehaviour.print("Assembly:" + assembly.assembly);

                if (assembly.assembly.ToString().Split(',')[0] == "RealFuels")
                {
                    MonoBehaviour.print("Found RealFuels mod");

                    Type RF_ModuleEngineConfigs_Type = assembly.assembly.GetType("RealFuels.ModuleEngineConfigs");
                    if (RF_ModuleEngineConfigs_Type != null)
                    {
                        RF_ModuleEngineConfigs_locaCorrectThrust = RF_ModuleEngineConfigs_Type.GetField("localCorrectThrust");
                    }

                    Type RF_ModuleHybridEngine_Type = assembly.assembly.GetType("RealFuels.ModuleHybridEngine");
                    if (RF_ModuleHybridEngine_Type != null)
                    {
                        RF_ModuleHybridEngine_locaCorrectThrust = RF_ModuleHybridEngine_Type.GetField("localCorrectThrust");
                    }

                    Type RF_ModuleHybridEngines_Type = assembly.assembly.GetType("RealFuels.ModuleHybridEngines");
                    if (RF_ModuleHybridEngines_Type != null)
                    {
                        RF_ModuleHybridEngines_locaCorrectThrust = RF_ModuleHybridEngines_Type.GetField("localCorrectThrust");
                    }

                    hasInstalledRealFuels = true;
                    break;
                }
            }
        }

        public static bool DoesEngineUseCorrectedThrust(Part theEngine)
        {
            if (!hasInstalledRealFuels /*|| HighLogic.LoadedSceneIsFlight*/)
            {
                return false;
            }

            // Look for any of the Real Fuels engine modules and call the relevant method to find out
            if (RF_ModuleEngineConfigs_locaCorrectThrust != null && theEngine.Modules.Contains("ModuleEngineConfigs"))
            {
                PartModule modEngineConfigs = theEngine.Modules["ModuleEngineConfigs"];
                if (modEngineConfigs != null)
                {
                    // Check the localCorrectThrust
                    if ((bool)RF_ModuleEngineConfigs_locaCorrectThrust.GetValue(modEngineConfigs))
                    {
                        return true;
                    }
                }
            }

            if (RF_ModuleHybridEngine_locaCorrectThrust != null && theEngine.Modules.Contains("ModuleHybridEngine"))
            {
                PartModule modHybridEngine = theEngine.Modules["ModuleHybridEngine"];
                if (modHybridEngine != null)
                {
                    // Check the localCorrectThrust
                    if ((bool)RF_ModuleHybridEngine_locaCorrectThrust.GetValue(modHybridEngine))
                    {
                        return true;
                    }
                }
            }

            if (RF_ModuleHybridEngines_locaCorrectThrust != null && theEngine.Modules.Contains("ModuleHybridEngines"))
            {
                PartModule modHybridEngines = theEngine.Modules["ModuleHybridEngines"];
                if (modHybridEngines != null)
                {
                    // Check the localCorrectThrust
                    if ((bool)RF_ModuleHybridEngines_locaCorrectThrust.GetValue(modHybridEngines))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public static void RequestSimulation()
        {
            if (!hasCheckedForRealFuels)
            {
                GetRealFuelsTypes();
            }

            bRequested = true;
            if (!timer.IsRunning)
            {
                timer.Start();
            }
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
                    ThreadPool.QueueUserWorkItem(RunSimulation, sim);
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
                        {
                            stage.Dump();
                        }
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
            {
                MonoBehaviour.print("Total simulation time: " + timer.ElapsedMilliseconds + "ms");
            }
#endif
            delayBetweenSims = minSimTime - timer.ElapsedMilliseconds;
            if (delayBetweenSims < 0)
            {
                delayBetweenSims = 0;
            }

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
    }
}