// Project:	KerbalEngineer
// Author:	CYBUTEK
// License:	Attribution-NonCommercial-ShareAlike 3.0 Unported

namespace KerbalEngineer.Flight.Readouts.Surface
{
    public class AtmosphericEfficiency : ReadoutModule
    {
        public AtmosphericEfficiency()
        {
            this.Name = "Atmos. Efficiency";
            this.Category = ReadoutCategory.Surface;
            this.HelpString = "Shows you vessel's efficiency as a ratio of the current velocity and terminal velocity.  Less than 1 means that you are losing efficiency due to gravity and greater than 1 is due to drag.";
        }

        public override void Update()
        {
            AtmosphericDetails.Instance.RequestUpdate();
        }

        public override void Draw()
        {
            if (FlightGlobals.ActiveVessel.atmDensity > 0)
            {
                this.DrawLine(AtmosphericDetails.Instance.Efficiency.ToString("F2"));
            }
        }

        public override void Reset()
        {
            FlightEngineerCore.Instance.AddUpdatable(AtmosphericDetails.Instance);
        }
    }
}