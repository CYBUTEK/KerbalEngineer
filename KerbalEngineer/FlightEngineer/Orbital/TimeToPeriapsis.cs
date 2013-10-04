// Name:    Kerbal Engineer Redux
// Author:  CYBUTEK
// License: Attribution-NonCommercial-ShareAlike 3.0 Unported

using KerbalEngineer.Extensions;
using UnityEngine;

namespace KerbalEngineer.FlightEngineer.Orbital
{
    public class TimeToPeriapsis : Readout
    {
        protected override void Initialise()
        {
            Name = "Time to Periapsis";
            Description = "Shows the time to periapsis.";
            Category = ReadoutCategory.Orbital;
        }

        public override void Draw()
        {
            DrawLine(FlightGlobals.ActiveVessel.orbit.timeToPe.ToTime());
        }
    }
}
