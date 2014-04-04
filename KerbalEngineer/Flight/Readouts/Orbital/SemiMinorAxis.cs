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
        }

        public override void Draw()
        {
            this.DrawLine(FlightGlobals.ActiveVessel.orbit.semiMinorAxis.ToDistance());
        }
    }
}