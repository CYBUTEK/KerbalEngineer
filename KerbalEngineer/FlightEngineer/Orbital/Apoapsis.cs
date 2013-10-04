// Name:    Kerbal Engineer Redux
// Author:  CYBUTEK
// License: Attribution-NonCommercial-ShareAlike 3.0 Unported

using KerbalEngineer.Extensions;
using UnityEngine;

namespace KerbalEngineer.FlightEngineer.Orbital
{
    public class Apoapsis : Readout
    {
        #region Instance

        private static Readout _instance;
        /// <summary>
        /// Gets the current instance of this readout.
        /// </summary>
        public static Readout Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Apoapsis();

                return _instance;
            }
        }

        #endregion

        protected override void Initialise()
        {
            Name = "Apoapsis Height";
            Description = "Shows the apoapsis height from sea level.";
            Category = ReadoutCategory.Orbital;
        }

        public override void Draw()
        {
            DrawLine(FlightGlobals.ActiveVessel.orbit.ApA.ToDistance());
        }
    }
}
