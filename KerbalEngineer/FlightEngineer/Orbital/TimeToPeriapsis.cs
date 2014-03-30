// Project:	KerbalEngineer
// Author:	CYBUTEK
// License:	Attribution-NonCommercial-ShareAlike 3.0 Unported

#region Using Directives

using KerbalEngineer.Extensions;

#endregion

namespace KerbalEngineer.FlightEngineer.Orbital
{
    public class TimeToPeriapsis : Readout
    {
        protected override void Initialise()
        {
            this.Name = "Time to Periapsis";
            this.Description = "Shows the time to periapsis.";
            this.Category = ReadoutCategory.Orbital;
        }

        public override void Draw()
        {
            this.DrawLine(FlightGlobals.ActiveVessel.orbit.timeToPe.ToTime());
        }
    }
}