// Project:	KerbalEngineer
// Author:	CYBUTEK
// License:	Attribution-NonCommercial-ShareAlike 3.0 Unported

#region Using Directives

using KerbalEngineer.Simulation;

#endregion

namespace KerbalEngineer.FlightEngineer.Vessel
{
    public class DeltaVStaged : Readout
    {
        private int stageCount;

        protected override void Initialise()
        {
            this.Name = "DeltaV Staged";
            this.Description = "Shows the deltaV for each stage.";
            this.Category = ReadoutCategory.Vessel;
        }

        public override void Update()
        {
            SimulationManager.Instance.RequestSimulation();
        }

        public override void Draw()
        {
            var stageCount = 0;

            for (var i = SimulationManager.Instance.Stages.Length - 1; i > -1; i--)
            {
                var stage = SimulationManager.Instance.Stages[i];
                if (stage.thrust > 0)
                {
                    stageCount++;
                    this.DrawLine("DeltaV (" + stage.Number + ")", stage.DeltaV);
                }
            }

            if (stageCount < this.stageCount)
            {
                SectionList.Instance.RequestResize();
            }

            this.stageCount = stageCount;
        }
    }
}