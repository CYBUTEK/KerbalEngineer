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
            this.HelpString = "Shows the vessel's apoapsis height relative to sea level.  (Apoapsis is the highest point of an orbit.)";
            this.IsDefault = true;
        }

        public override void Draw()
        {
            this.DrawLine(FlightGlobals.ship_orbit.ApA.ToString("N0") + "m");
        }
    }
}