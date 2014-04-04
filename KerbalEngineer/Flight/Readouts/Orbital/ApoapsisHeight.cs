// Project:	KerbalEngineer
// Author:	CYBUTEK
// License:	Attribution-NonCommercial-ShareAlike 3.0 Unported

namespace KerbalEngineer.Flight.Readouts.Orbital
{
    public class ApoapsisHeight : ReadoutModule
    {
        public ApoapsisHeight()
        {
            this.Name = "Apoapsis Height";
            this.Category = ReadoutCategory.Orbital;
            this.HelpMessage = "Shows the vessel's <b><i>apoapsis height</i></b> relative to sea level.";
        }

        public override void Draw()
        {
            this.DrawLine(FlightGlobals.ActiveVessel.orbit.ApA.ToString("N0") + "m");
        }
    }
}