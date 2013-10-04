// Name:    Kerbal Engineer Redux
// Author:  CYBUTEK
// License: Attribution-NonCommercial-ShareAlike 3.0 Unported

using KerbalEngineer.Extensions;
using UnityEngine;

namespace KerbalEngineer.FlightEngineer.Orbital
{
    public class PeriapsisHeight : Readout
    {
        protected override void Initialise()
        {
            Name = "Periapsis Height";
            Description = "Shows the periapsis height from sea level.";
            Category = ReadoutCategory.Orbital;
        }

        public override void Draw()
        {
            DrawLine(FlightGlobals.ActiveVessel.orbit.PeA.ToDistance());
        }
    }
}
