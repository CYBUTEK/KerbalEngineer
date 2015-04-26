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



#endregion

namespace KerbalEngineer.Flight.Readouts.Vessel
{
    #region Using Directives

    using System;
    using VesselSimulator;

    #endregion

    public class SimulationProcessor : IUpdatable, IUpdateRequest
    {
        #region Instance

        #region Fields

        private static readonly SimulationProcessor instance = new SimulationProcessor();

        #endregion

        #region Constructors

        static SimulationProcessor()
        {
            SimManager.OnReady += GetStageInfo;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the current instance of the simulation processor.
        /// </summary>
        public static SimulationProcessor Instance
        {
            get { return instance; }
        }

        #endregion

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the currently active vessel stage.
        /// </summary>
        public static Stage LastStage { get; private set; }

        /// <summary>
        ///     Gets whether the details are ready to be shown.
        /// </summary>
        public static bool ShowDetails { get; private set; }

        /// <summary>
        ///     Gets an array of the vessel stages.
        /// </summary>
        public static Stage[] Stages { get; private set; }

        public bool UpdateRequested { get; set; }

        #endregion

        #region Methods

        private static void GetStageInfo()
        {
            Stages = SimManager.Stages;
            LastStage = SimManager.LastStage;
        }

        public static void RequestUpdate()
        {
            instance.UpdateRequested = true;
        }

        public void Update()
        {
            SimManager.RequestSimulation();
            SimManager.TryStartSimulation();

            if (!SimManager.ResultsReady())
            {
                return;
            }

            if (Stages != null && LastStage != null)
            {
                ShowDetails = true;
            }

            if (FlightGlobals.ActiveVessel != null)
            {
                SimManager.Gravity = FlightGlobals.ActiveVessel.mainBody.gravParameter /
                                     Math.Pow(FlightGlobals.ActiveVessel.mainBody.Radius +
                                              FlightGlobals.ActiveVessel.mainBody.GetAltitude(FlightGlobals.ActiveVessel.CoM), 2);
                SimManager.Mach = FlightGlobals.ActiveVessel.mach;
            }
        }

        #endregion
    }
}