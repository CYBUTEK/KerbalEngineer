// Project:	KerbalEngineer
// Author:	CYBUTEK
// License:	Attribution-NonCommercial-ShareAlike 3.0 Unported

#region Using Directives

using KerbalEngineer.Extensions;

#endregion

namespace KerbalEngineer.FlightEngineer.Orbital
{
    public class PeriapsisHeight : Readout
    {
        protected override void Initialise()
        {
            this.Name = "Periapsis Height";
            this.Description = "Shows the periapsis height from sea level.";
            this.Category = ReadoutCategory.Orbital;
        }

        public override void Draw()
        {
            this.DrawLine(FlightGlobals.ActiveVessel.orbit.PeA.ToDistance());
        }
    }
}