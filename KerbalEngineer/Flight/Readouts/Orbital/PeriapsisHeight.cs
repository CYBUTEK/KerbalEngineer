// Project:	KerbalEngineer
// Author:	CYBUTEK
// License:	Attribution-NonCommercial-ShareAlike 3.0 Unported

namespace KerbalEngineer.Flight.Readouts.Orbital
{
    public class PeriapsisHeight : ReadoutModule
    {
        public PeriapsisHeight()
        {
            this.Name = "Periapsis Height";
            this.Category = ReadoutCategory.Orbital;
            this.HelpString = "Shows the vessel's periapsis height relative to sea level. (Periapsis is the lowest point of an orbit.";
            this.IsDefault = true;
        }

        public override void Draw()
        {
            this.DrawLine(FlightGlobals.ship_orbit.PeA.ToString("N0") + "m");
        }
    }
}