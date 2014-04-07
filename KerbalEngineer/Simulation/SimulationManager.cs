// Project:	KerbalEngineer
// Author:	CYBUTEK
// License:	Attribution-NonCommercial-ShareAlike 3.0 Unported

#region Using Directives

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

using KerbalEngineer.Flight;

using UnityEngine;

#endregion

namespace KerbalEngineer.Simulation
{
    public class SimulationManager : IUpdatable
    {
        private static readonly SimulationManager instance = new SimulationManager();

        public static SimulationManager Instance
        {
            get { return instance; }
        }

        private bool bRequested;
        private bool bRunning;
        private readonly Stopwatch timer = new Stopwatch();
        private long delayBetweenSims;

        private Stopwatch _func = new Stopwatch();

        public Stage[] Stages { get; private set; }
        public Stage LastStage { get; private set; }
        public String failMessage { get; private set; }

        public double Gravity { get; set; }
        public double Atmosphere { get; set; }

        public void RequestSimulation()
        {
            bRequested = true;
            if (!timer.IsRunning)
            {
                timer.Start();
            }
        }

        public void TryStartSimulation()
        {
            if (bRequested && !bRunning && (HighLogic.LoadedSceneIsEditor || FlightGlobals.ActiveVessel != null) && timer.ElapsedMilliseconds > delayBetweenSims)
            {
                bRequested = false;
                timer.Reset();
                StartSimulation();
            }
        }

        public bool ResultsReady()
        {
            return !bRunning;
        }

        private void ClearResults()
        {
            failMessage = "";
            Stages = null;
            LastStage = null;
        }

        private void StartSimulation()
        {
            try
            {
                bRunning = true;
                ClearResults();
                timer.Start();

                List<Part> parts = HighLogic.LoadedSceneIsEditor ? EditorLogic.SortedShipList : FlightGlobals.ActiveVessel.Parts;

                // Create the Simulation object in this thread
                var sim = new Simulation();

                // This call doesn't ever fail at the moment but we'll check and return a sensible error for display
                if (sim.PrepareSimulation(parts, Gravity, Atmosphere))
                {
                    ThreadPool.QueueUserWorkItem(RunSimulation, sim);
                }
                else
                {
                    failMessage = "PrepareSimulation failed";
                    bRunning = false;
                }
            }
            catch (Exception e)
            {
                MonoBehaviour.print("Exception in StartSimulation: " + e);
                failMessage = e.ToString();
                bRunning = false;
            }
        }

        private void RunSimulation(object simObject)
        {
            try
            {
                Stages = (simObject as Simulation).RunSimulation();
                if (Stages != null)
                {
#if LOG
                    foreach (Stage stage in Stages)
                        stage.Dump();
#endif
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
            MonoBehaviour.print("RunSimulation took " + timer.ElapsedMilliseconds + "ms");
            delayBetweenSims = 10 * timer.ElapsedMilliseconds;

            timer.Reset();
            timer.Start();

            bRunning = false;
        }

        public void Update()
        {
            this.TryStartSimulation();
        }
    }
}