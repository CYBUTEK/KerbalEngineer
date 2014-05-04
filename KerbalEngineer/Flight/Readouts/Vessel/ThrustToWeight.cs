// Project:	KerbalEngineer
// Author:	CYBUTEK
// License:	Attribution-NonCommercial-ShareAlike 3.0 Unported

#region Using Directives

using KerbalEngineer.Simulation;

#endregion

namespace KerbalEngineer.Flight.Readouts.Vessel
{
    public class ThrustToWeight : ReadoutModule
    {
        private string actual = string.Empty;
        private string total = string.Empty;

        public ThrustToWeight()
        {
            this.Name = "Thrust to Weight Ratio";
            this.Category = ReadoutCategory.Vessel;
            this.HelpString = "Shows the vessel's actual and total thrust to weight ratio.";
        }

        public override void Update()
        {
            SimulationManager.Instance.RequestSimulation();
        }

        public override void Draw()
        {
            this.actual = (SimulationManager.Instance.LastStage.actualThrust / (SimulationManager.Instance.LastStage.totalMass * FlightGlobals.getGeeForceAtPosition(FlightGlobals.ship_position).magnitude)).ToString("F2");
            this.total = (SimulationManager.Instance.LastStage.thrust / (SimulationManager.Instance.LastStage.totalMass * FlightGlobals.getGeeForceAtPosition(FlightGlobals.ship_position).magnitude)).ToString("F2");
            this.DrawLine("TWR", this.actual + " / " + this.total);
        }

        public override void Reset()
        {
            FlightEngineerCore.Instance.AddUpdatable(SimulationManager.Instance);
        }
    }
}