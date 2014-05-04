// Project:	KerbalEngineer
// Author:	CYBUTEK
// License:	Attribution-NonCommercial-ShareAlike 3.0 Unported

namespace KerbalEngineer.Flight.Readouts.Rendezvous
{
    public class RendezvousProcessor : IUpdatable, IUpdateRequest
    {
        #region Instance

        private static readonly RendezvousProcessor instance = new RendezvousProcessor();

        /// <summary>
        ///     Gets the current instance of the rendezvous processor.
        /// </summary>
        public static RendezvousProcessor Instance
        {
            get { return instance; }
        }

        #endregion

        #region Fields

        private Orbit sourceOrbit;
        private Orbit targetOrbit;

        #endregion

        #region Properties

        /// <summary>
        ///     Gets whether the details are ready to be shown.
        /// </summary>
        public static bool ShowDetails { get; private set; }

        /// <summary>
        ///     Gets the angular difference between the source and target orbits.
        /// </summary>
        public static double RelativeInclination { get; private set; }

        #endregion

        #region IUpdatable Members

        /// <summary>
        ///     Updates the details by recalculating if requested.
        /// </summary>
        public void Update()
        {
            if (FlightGlobals.fetch.VesselTarget == null)
            {
                ShowDetails = false;
                return;
            }

            ShowDetails = true;

            this.targetOrbit = FlightGlobals.fetch.VesselTarget.GetOrbit();
            this.sourceOrbit = (FlightGlobals.ship_orbit.referenceBody == Planetarium.fetch.Sun || FlightGlobals.ship_orbit.referenceBody == FlightGlobals.ActiveVessel.targetObject.GetOrbit().referenceBody)
                ? FlightGlobals.ship_orbit
                : FlightGlobals.ship_orbit.referenceBody.orbit;

            RelativeInclination = Vector3d.Angle(this.sourceOrbit.GetOrbitNormal(), this.targetOrbit.GetOrbitNormal());
        }

        #endregion

        #region IUpdateRequest Members

        /// <summary>
        ///     Gets and sets whether the updatable object should be updated.
        /// </summary>
        public bool UpdateRequested { get; set; }

        #endregion

        /// <summary>
        ///     Request and update to calculate the details.
        /// </summary>
        public static void RequestUpdate()
        {
            instance.UpdateRequested = true;
        }
    }
}