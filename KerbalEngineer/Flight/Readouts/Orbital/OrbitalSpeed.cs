using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using KerbalEngineer.Extensions;

namespace KerbalEngineer.Flight.Readouts.Orbital
{
    public class OrbitalSpeed : ReadoutModule
    {
        public OrbitalSpeed()
        {
            Name = "Orbital Speed";
            this.Category = ReadoutCategory.Orbital;
            this.HelpString = "Shows the vessel's orbital speed.";
        }

        public override void Draw()
        {
            this.DrawLine(FlightGlobals.ship_obtSpeed.ToSpeed());
        }
    }
}
