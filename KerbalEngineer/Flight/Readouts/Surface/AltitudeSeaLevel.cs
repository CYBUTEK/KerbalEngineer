// Project:	KerbalEngineer
// Author:	CYBUTEK
// License:	Attribution-NonCommercial-ShareAlike 3.0 Unported

#region Using Directives

using KerbalEngineer.Extensions;

#endregion

namespace KerbalEngineer.Flight.Readouts.Surface
{
    public class AltitudeSeaLevel : ReadoutModule
    {
        public AltitudeSeaLevel()
        {
            this.Name = "Altitude (Sea Level)";
            this.Category = ReadoutCategory.Surface;
            this.HelpString = "Shows the vessel's altitude above sea level.";
        }

        public override void Draw()
        {
            this.DrawLine(FlightGlobals.ActiveVessel.altitude.ToDistance());
        }
    }
}