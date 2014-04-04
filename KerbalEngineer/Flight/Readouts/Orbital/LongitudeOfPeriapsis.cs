// Project:	KerbalEngineer
// Author:	CYBUTEK
// License:	Attribution-NonCommercial-ShareAlike 3.0 Unported

#region Using Directives

using KerbalEngineer.Extensions;

#endregion

namespace KerbalEngineer.Flight.Readouts.Orbital
{
    public class LongitudeOfPeriapsis : ReadoutModule
    {
        public LongitudeOfPeriapsis()
        {
            this.Name = "Longitude of Pe";
            this.Category = ReadoutCategory.Orbital;
        }

        public override void Draw()
        {
            this.DrawLine((FlightGlobals.ActiveVessel.orbit.LAN + FlightGlobals.ActiveVessel.orbit.argumentOfPeriapsis).ToAngle());
        }
    }
}