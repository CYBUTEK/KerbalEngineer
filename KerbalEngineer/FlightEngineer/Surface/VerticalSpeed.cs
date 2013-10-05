// Name:    Kerbal Engineer Redux
// Author:  CYBUTEK
// License: Attribution-NonCommercial-ShareAlike 3.0 Unported

using KerbalEngineer.Extensions;
using UnityEngine;

namespace KerbalEngineer.FlightEngineer.Surface
{
    public class VerticalSpeed : Readout
    {
        protected override void Initialise()
        {
            Name = "Vertical Speed";
            Description = "Shows your vertical speed.";
            Category = ReadoutCategory.Surface;
        }

        public override void Draw()
        {
            DrawLine(FlightGlobals.ActiveVessel.verticalSpeed.ToSpeed());
        }
    }
}
