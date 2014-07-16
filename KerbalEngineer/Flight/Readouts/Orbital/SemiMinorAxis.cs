// Project:	KerbalEngineer
// Author:	CYBUTEK
// License:	Attribution-NonCommercial-ShareAlike 3.0 Unported

#region Using Directives

using KerbalEngineer.Extensions;

#endregion

namespace KerbalEngineer.Flight.Readouts.Orbital
{
    public class SemiMinorAxis : ReadoutModule
    {
        public SemiMinorAxis()
        {
            this.Name = "Semi-Minor Axis";
            this.Category = ReadoutCategory.Orbital;
            this.HelpString = "Shows the distance from the centre of an orbit to the nearest edge.";
            this.IsDefault = true;
        }

        public override void Draw()
        {
            this.DrawLine(FlightGlobals.ship_orbit.semiMinorAxis.ToDistance());
        }
    }
}