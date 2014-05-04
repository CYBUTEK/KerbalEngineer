// Project:	KerbalEngineer
// Author:	CYBUTEK
// License:	Attribution-NonCommercial-ShareAlike 3.0 Unported

#region Using Directives

using System;

using KerbalEngineer.Flight;

#endregion

namespace KerbalEngineer.Simulation
{
    public class SimulationManager : IUpdatable
    {
        #region Instance

        private static readonly SimulationManager instance = new SimulationManager();

        /// <summary>
        ///     Gets the current instance of the simulation manager.
        /// </summary>
        public static SimulationManager Instance
        {
            get { return instance; }
        }

        #endregion

        #region Properties

        public Stage[] Stages
        {
            get { return SimManager.Stages; }
        }

        public Stage LastStage
        {
            get { return SimManager.LastStage; }
        }

        public double Gravity
        {
            get { return SimManager.Gravity; }
            set { SimManager.Atmosphere = value; }
        }

        public double Atmosphere
        {
            get { return SimManager.Atmosphere; }
            set { SimManager.Atmosphere = value; }
        }

        #endregion

        #region IUpdatable Members

        public void Update()
        {
            this.TryStartSimulation();
        }

        #endregion

        #region Methods

        public void RequestSimulation()
        {
            SimManager.RequestSimulation();
        }

        public void TryStartSimulation()
        {
            SimManager.TryStartSimulation();
        }

        #endregion
    }
}