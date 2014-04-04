// Project:	KerbalEngineer
// Author:	CYBUTEK
// License:	Attribution-NonCommercial-ShareAlike 3.0 Unported

#region Using Directives

using KerbalEngineer.Extensions;

#endregion

namespace KerbalEngineer.Flight.Readouts.Orbital
{
    public class OrbitalPeriod : ReadoutModule
    {
        public OrbitalPeriod()
        {
            this.Name = "Orbital Period";
            this.Category = ReadoutCategory.Orbital;
            this.HelpString = "Shows the amount of time it will take to complete a full orbit.";
        }

        public override void Draw()
        {
            this.DrawLine(FlightGlobals.ActiveVessel.orbit.period.ToTime());
        }
    }
}