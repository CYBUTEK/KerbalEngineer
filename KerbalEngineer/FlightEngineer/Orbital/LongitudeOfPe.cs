// Project:	KerbalEngineer
// Author:	CYBUTEK
// License:	Attribution-NonCommercial-ShareAlike 3.0 Unported

#region Using Directives

using KerbalEngineer.Extensions;

#endregion

namespace KerbalEngineer.FlightEngineer.Orbital
{
    public class LongitudeOfPe : Readout
    {
        protected override void Initialise()
        {
            this.Name = "Longitude of Pe";
            this.Description = "Shows your longitude of the periapsis.";
            this.Category = ReadoutCategory.Orbital;
        }

        public override void Draw()
        {
            this.DrawLine((FlightGlobals.ActiveVessel.orbit.LAN + FlightGlobals.ActiveVessel.orbit.argumentOfPeriapsis).ToAngle());
        }
    }
}