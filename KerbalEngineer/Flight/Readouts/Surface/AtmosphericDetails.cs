// Project:	KerbalEngineer
// Author:	CYBUTEK
// License:	Attribution-NonCommercial-ShareAlike 3.0 Unported

#region Using Directives

using System;
using System.Linq;

using KerbalEngineer.Extensions;

#endregion

namespace KerbalEngineer.Flight.Readouts.Surface
{
    public class AtmosphericDetails : IUpdatable
    {
        #region Instance

        private static AtmosphericDetails instance;

        /// <summary>
        ///     Gets the current instance of atmospheric details.
        /// </summary>
        public static AtmosphericDetails Instance
        {
            get { return instance ?? (instance = new AtmosphericDetails()); }
        }

        #endregion

        #region Fields

        private bool updateRequested;

        #endregion

        #region Properties

        private double efficiency;
        private double terminalVelocity;

        /// <summary>
        ///     Gets whether an update has been requested.
        /// </summary>
        public bool UpdateRequested
        {
            get { return this.updateRequested; }
        }

        /// <summary>
        ///     Gets the terminal velocity of the active vessel.
        /// </summary>
        public double TerminalVelocity
        {
            get { return this.terminalVelocity; }
        }

        /// <summary>
        ///     Gets the difference between current velocity and terminal velocity.
        /// </summary>
        public double Efficiency
        {
            get { return this.efficiency; }
        }

        #endregion

        /// <summary>
        ///     Updated the details by recalculating if requested.
        /// </summary>
        public void Update()
        {
            if (!this.updateRequested || FlightGlobals.ActiveVessel.atmDensity == 0)
            {
                return;
            }

            this.updateRequested = false;

            var mass = FlightGlobals.ActiveVessel.parts.Sum(p => p.GetWetMass());
            var drag = FlightGlobals.ActiveVessel.parts.Sum(p => p.GetWetMass() * p.maximum_drag);
            var grav = FlightGlobals.getGeeForceAtPosition(FlightGlobals.ship_position).magnitude;
            var atmo = FlightGlobals.ActiveVessel.atmDensity;
            var coef = FlightGlobals.DragMultiplier;

            this.terminalVelocity = Math.Sqrt((2 * mass * grav) / (atmo * drag * coef));
            this.efficiency = FlightGlobals.ship_srfSpeed / this.terminalVelocity;
        }

        /// <summary>
        ///     Request an update to recalculate the details.
        /// </summary>
        public void RequestUpdate()
        {
            this.updateRequested = true;
        }
    }
}