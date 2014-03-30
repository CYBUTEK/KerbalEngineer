// Project:	KerbalEngineer
// Author:	CYBUTEK
// License:	Attribution-NonCommercial-ShareAlike 3.0 Unported

#region Using Directives

using KerbalEngineer.Extensions;

#endregion

namespace KerbalEngineer.FlightEngineer.Surface
{
    public class TerminalVelocity : Readout
    {
        private bool visible;

        protected override void Initialise()
        {
            this.Name = "Terminal Velocity";
            this.Description = "Shows your terminal velocity in atmosphere.";
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

                this.DrawLine(AtmosphericDetails.Instance.TerminalVelocity.ToSpeed());
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