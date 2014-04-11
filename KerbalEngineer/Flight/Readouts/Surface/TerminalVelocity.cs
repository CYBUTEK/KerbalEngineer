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
            if (FlightGlobals.ActiveVessel.atmDensity > 0)
            {
                this.DrawLine(AtmosphericDetails.Instance.TerminalVelocity.ToSpeed());
            }
        }

        public override void Reset()
        {
            FlightEngineerCore.Instance.AddUpdatable(AtmosphericDetails.Instance);
        }
    }
}