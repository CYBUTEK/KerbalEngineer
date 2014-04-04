// Project:	KerbalEngineer
// Author:	CYBUTEK
// License:	Attribution-NonCommercial-ShareAlike 3.0 Unported

#region Using Directives

using KerbalEngineer.Extensions;

#endregion

namespace KerbalEngineer.Flight.Readouts.Surface
{
    public class HorizontalSpeed : ReadoutModule
    {
        public HorizontalSpeed()
        {
            this.Name = "Horizontal Speed";
            this.Category = ReadoutCategory.Surface;
            this.HelpString = "Shows the vessel's horizontal speed across a celestial bodies surface.";
        }

        public override void Draw()
        {
            this.DrawLine(FlightGlobals.ActiveVessel.horizontalSrfSpeed.ToSpeed());
        }
    }
}