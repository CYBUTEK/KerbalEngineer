// Project:	KerbalEngineer
// Author:	CYBUTEK
// License:	Attribution-NonCommercial-ShareAlike 3.0 Unported

#region Using Directives

using KerbalEngineer.Extensions;

#endregion

namespace KerbalEngineer.FlightEngineer.Orbital
{
    public class Eccentricity : Readout
    {
        protected override void Initialise()
        {
            this.Name = "Eccentricity";
            this.Description = "Shows your current eccentricity.";
            this.Category = ReadoutCategory.Orbital;
        }

        public override void Draw()
        {
            this.DrawLine(FlightGlobals.ActiveVessel.orbit.eccentricity.ToAngle());
        }
    }
}