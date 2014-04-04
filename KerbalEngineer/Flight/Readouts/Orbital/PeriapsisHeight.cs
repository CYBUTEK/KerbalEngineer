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
            this.HelpMessage = "Shows the vessel's <b><i>periapsis height</i></b> relative to sea level.";
        }

        public override void Draw()
        {
            this.DrawLine(FlightGlobals.ActiveVessel.orbit.PeA.ToString("N0") + "m");
        }
    }
}