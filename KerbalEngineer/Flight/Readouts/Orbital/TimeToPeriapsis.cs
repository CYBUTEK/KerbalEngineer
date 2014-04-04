// Project:	KerbalEngineer
// Author:	CYBUTEK
// License:	Attribution-NonCommercial-ShareAlike 3.0 Unported

#region Using Directives

using KerbalEngineer.Extensions;

#endregion

namespace KerbalEngineer.Flight.Readouts.Orbital
{
    public class TimeToPeriapsis : ReadoutModule
    {
        public TimeToPeriapsis()
        {
            this.Name = "Time to Periapsis";
            this.Category = ReadoutCategory.Orbital;
        }

        public override void Draw()
        {
            this.DrawLine(FlightGlobals.ActiveVessel.orbit.timeToPe.ToTime());
        }
    }
}