// Project:	KerbalEngineer
// Author:	CYBUTEK
// License:	Attribution-NonCommercial-ShareAlike 3.0 Unported

#region Using Directives

using KerbalEngineer.Extensions;

#endregion

namespace KerbalEngineer.Flight.Readouts.Orbital
{
    public class TimeToApoapsis : ReadoutModule
    {
        public TimeToApoapsis()
        {
            this.Name = "Time to Apoapsis";
            this.Category = ReadoutCategory.Orbital;
        }

        public override void Draw()
        {
            this.DrawLine(FlightGlobals.ActiveVessel.orbit.timeToAp.ToTime());
        }
    }
}