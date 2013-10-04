// Name:    Kerbal Engineer Redux
// Author:  CYBUTEK
// License: Attribution-NonCommercial-ShareAlike 3.0 Unported

using KerbalEngineer.Extensions;
using UnityEngine;

namespace KerbalEngineer.FlightEngineer.Orbital
{
    public class LongitudeOfPe : Readout
    {
        protected override void Initialise()
        {
            Name = "Longitude of Pe";
            Description = "Shows your longitude of the periapsis.";
            Category = ReadoutCategory.Orbital;
        }

        public override void Draw()
        {
            DrawLine((FlightGlobals.ActiveVessel.orbit.LAN + FlightGlobals.ActiveVessel.orbit.argumentOfPeriapsis).ToAngle());
        }
    }
}
