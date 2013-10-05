// Name:    Kerbal Engineer Redux
// Author:  CYBUTEK
// License: Attribution-NonCommercial-ShareAlike 3.0 Unported

using System;
using System.Linq;
using KerbalEngineer.Extensions;
using UnityEngine;

namespace KerbalEngineer.FlightEngineer.Surface
{
    public class AtmosphericDetails
    {
        #region Instance

        private static AtmosphericDetails _instance;
        /// <summary>
        /// Gets the current instance of atmospheric details.
        /// </summary>
        public static AtmosphericDetails Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new AtmosphericDetails();

                return _instance;
            }
        }

        #endregion

        #region Properties

        private bool _requestUpdate = false;
        /// <summary>
        /// Gets and sets whether an update has been requested.
        /// </summary>
        public bool RequestUpdate
        {
            get { return _requestUpdate; }
            set { _requestUpdate = value; }
        }

        private double _terminalVelocity = 0d;
        /// <summary>
        /// Gets the terminal velocity of the active vessel.
        /// </summary>
        public double TerminalVelocity
        {
            get { return _terminalVelocity; }
        }

        private double _efficiency = 0d;
        /// <summary>
        /// Gets the difference between current velocity and terminal velocity.
        /// </summary>
        public double Efficiency
        {
            get { return _efficiency; }
        }

        #endregion

        // Updates the details by recalculating if requested.
        public void Update()
        {
            if (_requestUpdate)
            {
                _requestUpdate = false;

                double mass = FlightGlobals.ActiveVessel.parts.Sum(p => p.GetWetMass());
                double drag = FlightGlobals.ActiveVessel.parts.Sum(p => p.GetWetMass() * p.maximum_drag);
                double grav = FlightGlobals.getGeeForceAtPosition(FlightGlobals.ActiveVessel.CoM).magnitude;
                double atmo = FlightGlobals.ActiveVessel.atmDensity;
                double coef = FlightGlobals.DragMultiplier;

                _terminalVelocity = Math.Sqrt((2 * mass * grav) / (atmo * drag * coef));
                _efficiency = FlightGlobals.ActiveVessel.srf_velocity.magnitude / _terminalVelocity;
            }
        }
    }
}
