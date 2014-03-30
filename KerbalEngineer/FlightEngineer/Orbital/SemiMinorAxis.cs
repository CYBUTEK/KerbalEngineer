// Project:	KerbalEngineer
// Author:	CYBUTEK
// License:	Attribution-NonCommercial-ShareAlike 3.0 Unported

#region Using Directives

using KerbalEngineer.Extensions;

#endregion

namespace KerbalEngineer.FlightEngineer.Orbital
{
    public class SemiMinorAxis : Readout
    {
        protected override void Initialise()
        {
            this.Name = "Semi-minor Axis";
            this.Description = "Shows your semi-minor axis.";
            this.Category = ReadoutCategory.Orbital;
        }

        public override void Draw()
        {
            this.DrawLine(FlightGlobals.ActiveVessel.orbit.semiMinorAxis.ToDistance());
        }
    }
}