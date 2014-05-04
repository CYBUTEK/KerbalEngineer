// Project:	KerbalEngineer
// Author:	CYBUTEK
// License:	Attribution-NonCommercial-ShareAlike 3.0 Unported

#region Using Directives

using KerbalEngineer.Extensions;

#endregion

namespace KerbalEngineer.Flight.Readouts.Surface
{
    public class TerminalVelocity : ReadoutModule
    {
        private bool showing;

        public TerminalVelocity()
        {
            this.Name = "Terminal Velocity";
            this.Category = ReadoutCategory.Surface;
            this.HelpString = "Shows the velocity where the efforts of thrust and drag are equalled out.";
        }

        public override void Update()
        {
            AtmosphericDetails.Instance.RequestUpdate();
        }

        public override void Draw()
        {
            var tempShowing = this.showing;
            this.showing = false;

            if (FlightGlobals.ActiveVessel.atmDensity > 0)
            {
                this.showing = true;
                this.DrawLine(AtmosphericDetails.Instance.TerminalVelocity.ToSpeed());
            }

            if (this.showing != tempShowing)
            {
                this.ResizeRequested = true;
            }
        }

        public override void Reset()
        {
            FlightEngineerCore.Instance.AddUpdatable(AtmosphericDetails.Instance);
        }
    }
}