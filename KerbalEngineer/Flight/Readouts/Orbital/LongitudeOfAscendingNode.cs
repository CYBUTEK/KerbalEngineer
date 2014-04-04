// Project:	KerbalEngineer
// Author:	CYBUTEK
// License:	Attribution-NonCommercial-ShareAlike 3.0 Unported

#region Using Directives

using KerbalEngineer.Extensions;

#endregion

namespace KerbalEngineer.Flight.Readouts.Orbital
{
    public class LongitudeOfAscendingNode : ReadoutModule
    {
        public LongitudeOfAscendingNode()
        {
            this.Name = "Longitude of AN";
            this.Category = ReadoutCategory.Orbital;
            this.HelpString = "Shows the vessel's longitude of the ascending node.";
        }

        public override void Draw()
        {
            this.DrawLine(FlightGlobals.ActiveVessel.orbit.LAN.ToAngle());
        }
    }
}