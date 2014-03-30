// Project:	KerbalEngineer
// Author:	CYBUTEK
// License:	Attribution-NonCommercial-ShareAlike 3.0 Unported

#region Using Directives

using KerbalEngineer.Extensions;

#endregion

namespace KerbalEngineer.FlightEngineer.Orbital
{
    public class ApoapsisHeight : Readout
    {
        protected override void Initialise()
        {
            this.Name = "Apoapsis Height";
            this.Description = "Shows the apoapsis height from sea level.";
            this.Category = ReadoutCategory.Orbital;
        }

        public override void Draw()
        {
            this.DrawLine(FlightGlobals.ActiveVessel.orbit.ApA.ToDistance());
        }
    }
}