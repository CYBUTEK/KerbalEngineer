// Name:    Kerbal Engineer Redux
// Author:  CYBUTEK
// License: Attribution-NonCommercial-ShareAlike 3.0 Unported

using KerbalEngineer.Extensions;
using UnityEngine;

namespace KerbalEngineer.FlightEngineer.Orbital
{
    public class SemiMajorAxis : Readout
    {
        protected override void Initialise()
        {
            Name = "Semi-major Axis";
            Description = "Shows your semi-major axis.";
            Category = ReadoutCategory.Orbital;
        }

        public override void Draw()
        {
            DrawLine(FlightGlobals.ActiveVessel.orbit.semiMajorAxis.ToDistance());
        }
    }
}
