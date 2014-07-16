// Project:	KerbalEngineer
// Author:	CYBUTEK
// License:	Attribution-NonCommercial-ShareAlike 3.0 Unported

#region Using Directives

using KerbalEngineer.Extensions;

#endregion

namespace KerbalEngineer.Flight.Readouts.Orbital
{
    public class SemiMajorAxis : ReadoutModule
    {
        public SemiMajorAxis()
        {
            this.Name = "Semi-Major Axis";
            this.Category = ReadoutCategory.Orbital;
            this.HelpString = "Shows the distance from the centre of an orbit to the farthest edge.";
            this.IsDefault = true;
        }

        public override void Draw()
        {
            this.DrawLine(FlightGlobals.ship_orbit.semiMajorAxis.ToDistance());
        }
    }
}