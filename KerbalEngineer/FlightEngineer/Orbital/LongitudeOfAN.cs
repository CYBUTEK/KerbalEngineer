// Name:    Kerbal Engineer Redux
// Author:  CYBUTEK
// License: Attribution-NonCommercial-ShareAlike 3.0 Unported

using KerbalEngineer.Extensions;
using UnityEngine;

namespace KerbalEngineer.FlightEngineer.Orbital
{
    public class LongitudeOfAN : Readout
    {
        protected override void Initialise()
        {
            Name = "Longitude of AN";
            Description = "Shows your longitude of the ascending node.";
            Category = ReadoutCategory.Orbital;
        }

        public override void Draw()
        {
            DrawLine(FlightGlobals.ActiveVessel.orbit.LAN.ToAngle());
        }
    }
}
