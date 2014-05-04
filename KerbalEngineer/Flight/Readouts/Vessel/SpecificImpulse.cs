// Project:	KerbalEngineer
// Author:	CYBUTEK
// License:	Attribution-NonCommercial-ShareAlike 3.0 Unported

#region Using Directives

using KerbalEngineer.Simulation;

#endregion

namespace KerbalEngineer.Flight.Readouts.Vessel
{
    public class SpecificImpulse : ReadoutModule
    {
        public SpecificImpulse()
        {
            this.Name = "Specific Impulse";
            this.Category = ReadoutCategory.Vessel;
        }

        public override void Update()
        {
            SimulationManager.Instance.RequestSimulation();
        }

        public override void Draw()
        {
            this.DrawLine(SimulationManager.Instance.LastStage.isp.ToString("F1") + "s");
        }

        public override void Reset()
        {
            FlightEngineerCore.Instance.AddUpdatable(SimulationManager.Instance);
        }
    }
}