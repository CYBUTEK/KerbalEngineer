// Project:	KerbalEngineer
// Author:	CYBUTEK
// License:	Attribution-NonCommercial-ShareAlike 3.0 Unported

#region Using Directives

using KerbalEngineer.Extensions;

#endregion

namespace KerbalEngineer.Flight.Readouts.Surface
{
    public class Longitude : ReadoutModule
    {
        public Longitude()
        {
            this.Name = "Longitude";
            this.Category = ReadoutCategory.Surface;
        }

        public override void Draw()
        {
            this.DrawLine(FlightGlobals.ActiveVessel.longitude.ToAngle());
        }
    }
}