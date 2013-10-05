// Name:    Kerbal Engineer Redux
// Author:  CYBUTEK
// License: Attribution-NonCommercial-ShareAlike 3.0 Unported

using KerbalEngineer.Extensions;
using UnityEngine;

namespace KerbalEngineer.FlightEngineer.Surface
{
    public class HorizontalSpeed : Readout
    {
        protected override void Initialise()
        {
            Name = "Horizontal Speed";
            Description = "Shows your horizontal speed.";
            Category = ReadoutCategory.Surface;
        }

        public override void Draw()
        {
            DrawLine(FlightGlobals.ActiveVessel.horizontalSrfSpeed.ToSpeed());
        }
    }
}
