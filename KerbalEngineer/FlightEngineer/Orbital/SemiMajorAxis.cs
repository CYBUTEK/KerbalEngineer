// Project:	KerbalEngineer
// Author:	CYBUTEK
// License:	Attribution-NonCommercial-ShareAlike 3.0 Unported

#region Using Directives

using KerbalEngineer.Extensions;

#endregion

namespace KerbalEngineer.FlightEngineer.Orbital
{
    public class SemiMajorAxis : Readout
    {
        protected override void Initialise()
        {
            this.Name = "Semi-major Axis";
            this.Description = "Shows your semi-major axis.";
            this.Category = ReadoutCategory.Orbital;
        }

        public override void Draw()
        {
            this.DrawLine(FlightGlobals.ActiveVessel.orbit.semiMajorAxis.ToDistance());
        }
    }
}