// Name:    Kerbal Engineer Redux
// Author:  CYBUTEK
// License: Attribution-NonCommercial-ShareAlike 3.0 Unported

using System;
using KerbalEngineer.Extensions;
using KerbalEngineer.Simulation;
using UnityEngine;

namespace KerbalEngineer.FlightEngineer.Vessel
{
    public class DeltaVStaged : Readout
    {
        private int _stageCount = 0;

        protected override void Initialise()
        {
            Name = "DeltaV Staged";
            Description = "Shows the deltaV for each stage.";
            Category = ReadoutCategory.Vessel;
        }

        public override void Update()
        {
            SimulationManager.Instance.RequestSimulation();
        }

        public override void Draw()
        {
            int stageCount = 0;

            for (int i = SimulationManager.Instance.Stages.Length - 1; i > -1; i--)
            {
                Stage stage = SimulationManager.Instance.Stages[i];
                if (stage.thrust > 0d)
                {
                    stageCount++;
                    DrawLine("DeltaV (" + stage.Number + ")", stage.DeltaV);
                }
            }

            if (stageCount < _stageCount)
                SectionList.Instance.RequireResize = true;

            if (stageCount != _stageCount)
                _stageCount = stageCount;
        }
    }
}
