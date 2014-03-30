// Project:	KerbalEngineer
// Author:	CYBUTEK
// License:	Attribution-NonCommercial-ShareAlike 3.0 Unported

#region Using Directives

using KerbalEngineer.Extensions;

#endregion

namespace KerbalEngineer.FlightEngineer.Orbital
{
    public class Inclination : Readout
    {
        protected override void Initialise()
        {
            this.Name = "Inclination";
            this.Description = "Shows your current inclination.";
            this.Category = ReadoutCategory.Orbital;
        }

        public override void Draw()
        {
            this.DrawLine(FlightGlobals.ActiveVessel.orbit.inclination.ToAngle());
        }
    }
}