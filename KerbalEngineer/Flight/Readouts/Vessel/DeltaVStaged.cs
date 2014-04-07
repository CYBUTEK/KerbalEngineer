// Project:	KerbalEngineer
// Author:	CYBUTEK
// License:	Attribution-NonCommercial-ShareAlike 3.0 Unported

#region Using Directives

using System.Linq;

using KerbalEngineer.Simulation;

#endregion

namespace KerbalEngineer.Flight.Readouts.Vessel
{
    public class DeltaVStaged : ReadoutModule
    {
        private int numberOfStages;

        public DeltaVStaged()
        {
            this.Name = "DeltaV Staged";
            this.Category = ReadoutCategory.Vessel;
            this.HelpString = "Shows the vessel's delta velocity for each stage.";
        }

        public override void Update()
        {
            SimulationManager.Instance.RequestSimulation();
        }

        public override void Draw()
        {
            var newNumberOfStages = 0;

            foreach (var stage in SimulationManager.Instance.Stages)
            {
                if (stage.DeltaV > 0 || stage.Number == Staging.CurrentStage)
                {
                    this.DrawLine("DeltaV (S" + stage.Number + ")", stage.DeltaV.ToString("N0") + "m/s");
                    newNumberOfStages++;
                }
            }

            if (newNumberOfStages != this.numberOfStages)
            {
                this.numberOfStages = newNumberOfStages;
                this.ResizeRequested = true;
            }
        }

        public override void Reset()
        {
            FlightEngineerCore.Instance.AddUpdatable(SimulationManager.Instance);
        }
    }
}