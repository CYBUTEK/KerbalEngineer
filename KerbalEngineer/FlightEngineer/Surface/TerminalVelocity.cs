// Name:    Kerbal Engineer Redux
// Author:  CYBUTEK
// License: Attribution-NonCommercial-ShareAlike 3.0 Unported

using System;
using System.Linq;
using KerbalEngineer.Extensions;
using UnityEngine;

namespace KerbalEngineer.FlightEngineer.Surface
{
    public class TerminalVelocity : Readout
    {
        private bool _visible = false;

        protected override void Initialise()
        {
            Name = "Terminal Velocity";
            Description = "Shows your terminal velocity in atmosphere.";
            Category = ReadoutCategory.Surface;
        }

        public override void Update()
        {
            if (FlightGlobals.ActiveVessel.atmDensity > 0d)
                AtmosphericDetails.Instance.RequestUpdate = true;
        }

        public override void Draw()
        {
            if (FlightGlobals.ActiveVessel.atmDensity > 0d)
            {
                if (!_visible) _visible = true;
                DrawLine(AtmosphericDetails.Instance.TerminalVelocity.ToSpeed());
            }
            else
            {
                if (_visible)
                {
                    _visible = false;
                    SectionList.Instance.RequireResize = true;
                }
            }
        }
    }
}
