// Project:	KerbalEngineer
// Author:	CYBUTEK
// License:	Attribution-NonCommercial-ShareAlike 3.0 Unported

#region Using Directives

using KerbalEngineer.Extensions;

#endregion

namespace KerbalEngineer.Flight.Readouts.Surface
{
    public class Longitude : ReadoutModule
    {
        public Longitude()
        {
            this.Name = "Longitude";
            this.Category = ReadoutCategory.Surface;
            this.HelpString = "Shows the vessel's longitude around a celestial body.  Longitude is the angle from the bodies prime meridian.";
        }

        public override void Draw()
        {
            this.DrawLine(FlightGlobals.ship_longitude.ToAngle());
        }
    }
}