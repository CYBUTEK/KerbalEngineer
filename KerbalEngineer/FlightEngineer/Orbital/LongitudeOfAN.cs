// Project:	KerbalEngineer
// Author:	CYBUTEK
// License:	Attribution-NonCommercial-ShareAlike 3.0 Unported

#region Using Directives

using KerbalEngineer.Extensions;

#endregion

namespace KerbalEngineer.FlightEngineer.Orbital
{
    public class LongitudeOfAN : Readout
    {
        protected override void Initialise()
        {
            this.Name = "Longitude of AN";
            this.Description = "Shows your longitude of the ascending node.";
            this.Category = ReadoutCategory.Orbital;
        }

        public override void Draw()
        {
            this.DrawLine(FlightGlobals.ActiveVessel.orbit.LAN.ToAngle());
        }
    }
}