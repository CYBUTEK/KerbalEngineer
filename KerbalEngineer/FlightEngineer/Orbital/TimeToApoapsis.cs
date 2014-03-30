// Project:	KerbalEngineer
// Author:	CYBUTEK
// License:	Attribution-NonCommercial-ShareAlike 3.0 Unported

#region Using Directives

using KerbalEngineer.Extensions;

#endregion

namespace KerbalEngineer.FlightEngineer.Orbital
{
    public class TimeToApoapsis : Readout
    {
        protected override void Initialise()
        {
            this.Name = "Time to Apoapsis";
            this.Description = "Shows the time to apoapsis.";
            this.Category = ReadoutCategory.Orbital;
        }

        public override void Draw()
        {
            this.DrawLine(FlightGlobals.ActiveVessel.orbit.timeToAp.ToTime());
        }
    }
}