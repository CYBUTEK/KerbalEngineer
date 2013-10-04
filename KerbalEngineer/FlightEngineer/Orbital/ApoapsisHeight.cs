// Name:    Kerbal Engineer Redux
// Author:  CYBUTEK
// License: Attribution-NonCommercial-ShareAlike 3.0 Unported

using KerbalEngineer.Extensions;
using UnityEngine;

namespace KerbalEngineer.FlightEngineer.Orbital
{
    public class ApoapsisHeight : Readout
    {
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
