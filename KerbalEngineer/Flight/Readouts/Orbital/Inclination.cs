// Project:	KerbalEngineer
// Author:	CYBUTEK
// License:	Attribution-NonCommercial-ShareAlike 3.0 Unported

#region Using Directives

using KerbalEngineer.Extensions;

#endregion

namespace KerbalEngineer.Flight.Readouts.Orbital
{
    public class Inclination : ReadoutModule
    {
        public Inclination()
        {
            this.Name = "Inclination";
            this.Category = ReadoutCategory.Orbital;
            this.HelpString = "Shows the vessel's orbital inclination.";
            this.IsDefault = true;
        }

        public override void Draw()
        {
            this.DrawLine(FlightGlobals.ship_orbit.inclination.ToAngle());
        }
    }
}