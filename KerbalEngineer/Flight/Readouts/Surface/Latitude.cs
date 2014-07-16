// Project:	KerbalEngineer
// Author:	CYBUTEK
// License:	Attribution-NonCommercial-ShareAlike 3.0 Unported

#region Using Directives

using KerbalEngineer.Extensions;

#endregion

namespace KerbalEngineer.Flight.Readouts.Surface
{
    public class Latitude : ReadoutModule
    {
        public Latitude()
        {
            this.Name = "Latitude";
            this.Category = ReadoutCategory.Surface;
            this.HelpString = "Shows the vessel's latitude position around the celestial body.  Latitude is the angle from the equator to poles.";
            this.IsDefault = true;
        }

        public override void Draw()
        {
            this.DrawLine(FlightGlobals.ship_latitude.ToAngle());
        }
    }
}