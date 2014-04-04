// Project:	KerbalEngineer
// Author:	CYBUTEK
// License:	Attribution-NonCommercial-ShareAlike 3.0 Unported

#region Using Directives

using KerbalEngineer.Extensions;

#endregion

namespace KerbalEngineer.Flight.Readouts.Surface
{
    public class VerticalSpeed : ReadoutModule
    {
        public VerticalSpeed()
        {
            this.Name = "Vertical Speed";
            this.Category = ReadoutCategory.Surface;
            this.HelpString = "Shows the vessel's vertical speed up and down.";
        }

        public override void Draw()
        {
            this.DrawLine(FlightGlobals.ActiveVessel.verticalSpeed.ToSpeed());
        }
    }
}