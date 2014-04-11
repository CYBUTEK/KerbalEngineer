// Project:	KerbalEngineer
// Author:	CYBUTEK
// License:	Attribution-NonCommercial-ShareAlike 3.0 Unported

#region Using Directives

using KerbalEngineer.Extensions;

#endregion

namespace KerbalEngineer.Flight.Readouts.Orbital
{
    public class Eccentricity : ReadoutModule
    {
        public Eccentricity()
        {
            this.Name = "Eccentricity";
            this.Category = ReadoutCategory.Orbital;
            this.HelpString = "Shows the vessel's orbital eccentricity.";
        }

        public override void Draw()
        {
            this.DrawLine(FlightGlobals.ship_orbit.eccentricity.ToAngle());
        }
    }
}