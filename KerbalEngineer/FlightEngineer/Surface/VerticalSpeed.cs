// Project:	KerbalEngineer
// Author:	CYBUTEK
// License:	Attribution-NonCommercial-ShareAlike 3.0 Unported

#region Using Directives

using KerbalEngineer.Extensions;

#endregion

namespace KerbalEngineer.FlightEngineer.Surface
{
    public class VerticalSpeed : Readout
    {
        protected override void Initialise()
        {
            this.Name = "Vertical Speed";
            this.Description = "Shows your vertical speed.";
            this.Category = ReadoutCategory.Surface;
        }

        public override void Draw()
        {
            this.DrawLine(FlightGlobals.ActiveVessel.verticalSpeed.ToSpeed());
        }
    }
}