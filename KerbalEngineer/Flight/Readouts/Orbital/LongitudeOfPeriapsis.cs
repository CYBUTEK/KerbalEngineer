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
            this.HelpString = "Shows the vessel's longitude of periapsis.";
            this.IsDefault = true;
        }

        public override void Draw()
        {
            this.DrawLine((FlightGlobals.ship_orbit.LAN + FlightGlobals.ship_orbit.argumentOfPeriapsis).ToAngle());
        }
    }
}