// Project:	KerbalEngineer
// Author:	CYBUTEK
// License:	Attribution-NonCommercial-ShareAlike 3.0 Unported

namespace KerbalEngineer.FlightEngineer.Surface
{
    public class AtmosEfficiency : Readout
    {
        private bool visible;

        protected override void Initialise()
        {
            this.Name = "Atmos. Efficiency";
            this.Description = "The difference between current and terminal velocity.";
            this.Category = ReadoutCategory.Surface;
        }

        public override void Update()
        {
            if (FlightGlobals.ActiveVessel.atmDensity > 0)
            {
                AtmosphericDetails.Instance.RequestUpdate();
            }
        }

        public override void Draw()
        {
            if (FlightGlobals.ActiveVessel.atmDensity > 0)
            {
                if (!this.visible)
                {
                    this.visible = true;
                }

                this.DrawLine(AtmosphericDetails.Instance.Efficiency.ToString("0.00"));
            }
            else
            {
                if (this.visible)
                {
                    this.visible = false;
                    SectionList.Instance.RequestResize();
                }
            }
        }
    }
}