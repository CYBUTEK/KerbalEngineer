// Project:	KerbalEngineer
// Author:	CYBUTEK
// License:	Attribution-NonCommercial-ShareAlike 3.0 Unported

#region Using Directives

using KerbalEngineer.Extensions;

#endregion

namespace KerbalEngineer.FlightEngineer.Surface
{
    public class Longitude : Readout
    {
        protected override void Initialise()
        {
            this.Name = "Longitude";
            this.Description = "Shows your angle of longitude.";
            this.Category = ReadoutCategory.Surface;
        }

        public override void Draw()
        {
            this.DrawLine(FlightGlobals.ActiveVessel.longitude.ToAngle());
        }
    }
}