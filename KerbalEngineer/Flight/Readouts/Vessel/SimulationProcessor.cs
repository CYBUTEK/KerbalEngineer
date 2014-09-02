using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using KerbalEngineer.VesselSimulator;

namespace KerbalEngineer.Flight.Readouts.Vessel
{
    public class SimulationProcessor : IUpdatable, IUpdateRequest
    {
        #region Instance
        private static readonly SimulationProcessor instance = new SimulationProcessor();

        /// <summary>
        ///     Gets the current instance of the simulation processor.
        /// </summary>
        public static SimulationProcessor Instance
        {
            get { return instance; }
        }
        #endregion

        #region Properies

        /// <summary>
        ///     Gets whether the details are ready to be shown.
        /// </summary>
        public static bool ShowDetails { get; private set; }

        /// <summary>
        ///     Gets an array of the vessel stages.
        /// </summary>
        public static Stage[] Stages { get; private set; }

        /// <summary>
        ///     Gets the currently active vessel stage.
        /// </summary>
        public static Stage LastStage { get; private set; }

        #endregion

        public void Update()
        {
            SimManager.RequestSimulation();

            if (!SimManager.ResultsReady())
            {
                return;
            }

            Stages = SimManager.Stages;
            LastStage = SimManager.LastStage;

            if (Stages != null && LastStage != null)
            {
                ShowDetails = true;
            }


            if (FlightGlobals.ActiveVessel != null)
            {
                SimManager.Gravity = FlightGlobals.ActiveVessel.mainBody.gravParameter / Math.Pow(FlightGlobals.ActiveVessel.mainBody.Radius + FlightGlobals.ActiveVessel.mainBody.GetAltitude(FlightGlobals.ActiveVessel.CoM), 2);
                SimManager.Velocity = FlightGlobals.ActiveVessel.srfSpeed;
            }
            // We should be allowing this to be set too...
            //SimManager.vectoredThrust = vectoredThrust; 
            SimManager.TryStartSimulation();
        }

        public bool UpdateRequested { get; set; }

        public static void RequestUpdate()
        {
            instance.UpdateRequested = true;
        }
    }
}
