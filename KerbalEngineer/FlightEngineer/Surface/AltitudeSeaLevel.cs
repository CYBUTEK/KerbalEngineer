// Project:	KerbalEngineer
// Author:	CYBUTEK
// License:	Attribution-NonCommercial-ShareAlike 3.0 Unported

#region Using Directives

using KerbalEngineer.Extensions;

#endregion

namespace KerbalEngineer.FlightEngineer.Surface
{
    public class AltitudeSeaLevel : Readout
    {
        protected override void Initialise()
        {
            this.Name = "Altitude (Sea Level)";
            this.Description = "Shows your altitude relative to sea level.";
            this.Category = ReadoutCategory.Surface;
        }

        public override void Draw()
        {
            this.DrawLine(FlightGlobals.ActiveVessel.altitude.ToDistance());
        }
    }
}