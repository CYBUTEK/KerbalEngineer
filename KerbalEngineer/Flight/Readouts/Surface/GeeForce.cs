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
        }

        public override void Draw()
        {
            if (FlightGlobals.ActiveVessel.geeForce > this.maxGeeForce)
            {
                this.maxGeeForce = FlightGlobals.ActiveVessel.geeForce;
            }
            this.DrawLine(FlightGlobals.ActiveVessel.geeForce.ToString("F3") + " / " + this.maxGeeForce.ToString("F3"));
        }

        public override void Reset()
        {
            this.maxGeeForce = 0;
        }
    }
}