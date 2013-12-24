// Name:    Kerbal Engineer Redux
// Author:  CYBUTEK
// License: Attribution-NonCommercial-ShareAlike 3.0 Unported

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Linq;

namespace KerbalEngineer.Simulation
{
    public class SimulationManager
    {
        #region Instance

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

        #endregion

        #region Fields

        private bool _simRequested = false;
        private bool _simRunning = false;
        private Stopwatch _timer = new Stopwatch();
        private long _millisecondsBetweenSimulations = 0;
        private int _numberOfStages = 0;

        #endregion

        #region Properties

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

        #endregion

        #region Initialisation

        public SimulationManager()
        {
            Stages = new Stage[0];
            LastStage = new Stage();

            Gravity = 9.81d;
            Atmosphere = 0d;
        }

        #endregion

        #region Updating

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

        #endregion

        #region Processing

        private void StartSimulation()
        {
            _simRunning = true;
            _timer.Start();

            List<Part> parts = HighLogic.LoadedSceneIsEditor ? EditorLogic.fetch.ship.Parts : FlightGlobals.ActiveVessel.Parts;

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
            try
            {
                int stagesWithDeltaV = this.Stages.Where(s => s.deltaV > 0d).Count();
                this.Stages = (simObject as Simulation).RunSimulation(this.Gravity, this.Atmosphere);

                this.LastStage = this.Stages.Last();
                if (this.Stages.Length != _numberOfStages || this.Stages.Where(s => s.deltaV > 0d).Count() != stagesWithDeltaV)
                {
                    _numberOfStages = this.Stages.Length;
                    _stagingChanged = true;
                }
            }
            catch { /* Something went wrong! */ }

            _timer.Stop();
            _millisecondsBetweenSimulations = 10 * _timer.ElapsedMilliseconds;

            _timer.Reset();
            _timer.Start();

            _simRunning = false;
        }

        #endregion
    }
}
