// Name:    Kerbal Engineer Redux
// Author:  CYBUTEK
// License: Attribution-NonCommercial-ShareAlike 3.0 Unported

using KerbalEngineer.Extensions;
using UnityEngine;

namespace KerbalEngineer.FlightEngineer.Surface
{
    public class Longitude : Readout
    {
        protected override void Initialise()
        {
            Name = "Longitude";
            Description = "Shows your angle of longitude.";
            Category = ReadoutCategory.Surface;
        }

        public override void Draw()
        {
            DrawLine(FlightGlobals.ActiveVessel.longitude.ToAngle());
        }
    }
}
