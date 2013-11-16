// Name:    Kerbal Engineer Redux
// Author:  CYBUTEK
// License: Attribution-NonCommercial-ShareAlike 3.0 Unported

using System;
using System.Linq;
using KerbalEngineer.Extensions;
using UnityEngine;

namespace KerbalEngineer.FlightEngineer.Surface
{
    public class AtmosEfficiency : Readout
    {
        private bool _visible = false;

        protected override void Initialise()
        {
            Name = "Atmos. Efficiency";
            Description = "The difference between current and terminal velocity.";
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
                DrawLine(AtmosphericDetails.Instance.Efficiency.ToString("0.00"));
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
