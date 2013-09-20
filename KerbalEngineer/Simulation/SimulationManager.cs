using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Linq;

namespace KerbalEngineer.Simulation
{
    public class SimulationManager
    {
        private static SimulationManager _instance;
        /// <summary>
        /// Gets the current instance of the simulation manager.
        /// </summary>
        public static SimulationManager Instance
        {
            get
            {
                if (_instance == null) _instance = new SimulationManager();
                return _instance;
            }
        }

        private bool _simRequested = false;
        private bool _simRunning = false;
        private Stopwatch _timer = new Stopwatch();
        private long _millisecondsBetweenSimulations = 0;
        private int _numberOfStages = 0;

        public Stage[] Stages { get; private set; }
        public Stage LastStage { get; private set; }

        public double Gravity { get; set; }
        public double Atmosphere { get; set; }

        private bool _stagingChanged = false;
        public bool StagingChanged
        {
            get
            {
                if (_stagingChanged)
                {
                    _stagingChanged = false;
                    return true;
                }
                return false;
            }
        }

        public SimulationManager()
        {
            Stages = new Stage[0];
            LastStage = new Stage();

            Gravity = 9.81d;
            Atmosphere = 0d;
        }

        public void RequestSimulation()
        {
            _simRequested = true;
            if (!_timer.IsRunning) _timer.Start();
        }

        public void TryStartSimulation()
        {
            if ((HighLogic.LoadedSceneIsEditor || FlightGlobals.ActiveVessel != null) && !_simRunning)
            {
                if (_timer.ElapsedMilliseconds > _millisecondsBetweenSimulations)
                {
                    if (_simRequested)
                    {
                        _simRequested = false;
                        _timer.Reset();

                        StartSimulation();
                    }
                }
            }
        }

        private void StartSimulation()
        {
            _simRunning = true;
            _timer.Start();

            List<Part> parts = HighLogic.LoadedSceneIsEditor ? EditorLogic.SortedShipList : FlightGlobals.ActiveVessel.Parts;

            if (parts.Count > 0)
            {
                ThreadPool.QueueUserWorkItem(RunSimulation, new Simulation(parts));
                //RunSimulation(new Simulation(parts));
            }
            else
            {
                this.Stages = new Stage[0];
                this.LastStage = new Stage();
            }
        }

        private void RunSimulation(object simObject)
        {
            int stagesWithDeltaV = this.Stages.Where(s => s.deltaV > 0d).Count();
            this.Stages = (simObject as Simulation).RunSimulation(this.Gravity, this.Atmosphere);
            this.LastStage = this.Stages.Last();
            if (this.Stages.Length != _numberOfStages || this.Stages.Where(s => s.deltaV > 0d).Count() != stagesWithDeltaV)
            {
                _numberOfStages = this.Stages.Length;
                _stagingChanged = true;
            }

            _timer.Stop();
            _millisecondsBetweenSimulations = 10 * _timer.ElapsedMilliseconds;

            _timer.Reset();
            _timer.Start();

            _simRunning = false;
        }
    }
}
