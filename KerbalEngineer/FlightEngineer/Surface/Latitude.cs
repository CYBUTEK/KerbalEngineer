// Project:	KerbalEngineer
// Author:	CYBUTEK
// License:	Attribution-NonCommercial-ShareAlike 3.0 Unported

#region Using Directives

using KerbalEngineer.Extensions;

#endregion

namespace KerbalEngineer.FlightEngineer.Surface
{
    public class Latitude : Readout
    {
        protected override void Initialise()
        {
            this.Name = "Latitude";
            this.Description = "Shows your angle of latitude.";
            this.Category = ReadoutCategory.Surface;
        }

        public override void Draw()
        {
            this.DrawLine(FlightGlobals.ActiveVessel.latitude.ToAngle());
        }
    }
}