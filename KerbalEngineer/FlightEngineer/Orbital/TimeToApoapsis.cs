// Name:    Kerbal Engineer Redux
// Author:  CYBUTEK
// License: Attribution-NonCommercial-ShareAlike 3.0 Unported

using KerbalEngineer.Extensions;
using UnityEngine;

namespace KerbalEngineer.FlightEngineer.Orbital
{
    public class TimeToApoapsis : Readout
    {
        protected override void Initialise()
        {
            Name = "Time to Apoapsis";
            Description = "Shows the time to apoapsis.";
            Category = ReadoutCategory.Orbital;
        }

        public override void Draw()
        {
            DrawLine(FlightGlobals.ActiveVessel.orbit.timeToAp.ToTime());
        }
    }
}
