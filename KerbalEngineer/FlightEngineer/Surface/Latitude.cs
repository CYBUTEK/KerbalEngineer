// Name:    Kerbal Engineer Redux
// Author:  CYBUTEK
// License: Attribution-NonCommercial-ShareAlike 3.0 Unported

using KerbalEngineer.Extensions;
using UnityEngine;

namespace KerbalEngineer.FlightEngineer.Surface
{
    public class Latitude : Readout
    {
        protected override void Initialise()
        {
            Name = "Latitude";
            Description = "Shows your angle of latitude.";
            Category = ReadoutCategory.Surface;
        }

        public override void Draw()
        {
            DrawLine(FlightGlobals.ActiveVessel.latitude.ToAngle());
        }
    }
}
