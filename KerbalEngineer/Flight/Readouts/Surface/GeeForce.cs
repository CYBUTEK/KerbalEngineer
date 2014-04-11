// Project:	KerbalEngineer
// Author:	CYBUTEK
// License:	Attribution-NonCommercial-ShareAlike 3.0 Unported

namespace KerbalEngineer.Flight.Readouts.Surface
{
    public class GeeForce : ReadoutModule
    {
        private double maxGeeForce;

        public GeeForce()
        {
            this.Name = "G-Force";
            this.Category = ReadoutCategory.Surface;
            this.HelpString = "Shows the current g-force and maximum g-force experienced.";
        }

        public override void Draw()
        {
            if (FlightGlobals.ship_geeForce > this.maxGeeForce)
            {
                this.maxGeeForce = FlightGlobals.ship_geeForce;
            }
            this.DrawLine(FlightGlobals.ship_geeForce.ToString("F3") + " / " + this.maxGeeForce.ToString("F3"));
        }

        public override void Reset()
        {
            this.maxGeeForce = 0;
        }
    }
}