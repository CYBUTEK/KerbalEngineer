// Name:    Kerbal Engineer Redux
// Author:  CYBUTEK
// License: Attribution-NonCommercial-ShareAlike 3.0 Unported

using System;
using KerbalEngineer.Extensions;
using KerbalEngineer.Simulation;
using UnityEngine;

namespace KerbalEngineer.FlightEngineer.Vessel
{
    public class ThrustTotal : Readout
    {
        protected override void Initialise()
        {
            Name = "Thrust (Total)";
            Description = "Shows the vessel thrust.";
            Category = ReadoutCategory.Vessel;
        }

        public override void Update()
        {
            SimulationManager.Instance.RequestSimulation();
        }

        public override void Draw()
        {
            DrawLine(SimulationManager.Instance.LastStage.Thrust);
        }
    }
}
