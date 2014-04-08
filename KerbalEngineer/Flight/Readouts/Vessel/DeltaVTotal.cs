// Project:	KerbalEngineer
// Author:	CYBUTEK
// License:	Attribution-NonCommercial-ShareAlike 3.0 Unported

#region Using Directives

using KerbalEngineer.Simulation;

#endregion

namespace KerbalEngineer.Flight.Readouts.Vessel
{
    public class DeltaVTotal : ReadoutModule
    {
        public DeltaVTotal()
        {
            this.Name = "DeltaV Total";
            this.Category = ReadoutCategory.Vessel;
            this.HelpString = "Shows the vessel's total delta velocity.";
        }

        public override void Update()
        {
            SimulationManager.Instance.RequestSimulation();
        }

        public override void Draw()
        {
            this.DrawLine(SimulationManager.Instance.LastStage.TotalDeltaV.ToString("N0") + "m/s");
        }

        public override void Reset()
        {
            FlightEngineerCore.Instance.AddUpdatable(SimulationManager.Instance);
        }
    }
}