// Project:	KerbalEngineer
// Author:	CYBUTEK
// License:	Attribution-NonCommercial-ShareAlike 3.0 Unported

#region Using Directives

using KerbalEngineer.Extensions;

#endregion

namespace KerbalEngineer.FlightEngineer.Surface
{
    public class HorizontalSpeed : Readout
    {
        protected override void Initialise()
        {
            this.Name = "Horizontal Speed";
            this.Description = "Shows your horizontal speed.";
            this.Category = ReadoutCategory.Surface;
        }

        public override void Draw()
        {
            this.DrawLine(FlightGlobals.ActiveVessel.horizontalSrfSpeed.ToSpeed());
        }
    }
}