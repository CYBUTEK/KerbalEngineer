// Project:	KerbalEngineer
// Author:	CYBUTEK
// License:	Attribution-NonCommercial-ShareAlike 3.0 Unported

#region Using Directives

using KerbalEngineer.Extensions;
using KerbalEngineer.Simulation;

#endregion

namespace KerbalEngineer.Flight.Readouts.Vessel
{
    public class Thrust : ReadoutModule
    {
        public Thrust()
        {
            this.Name = "Thrust";
            this.Category = ReadoutCategory.Vessel;
        }

        public override void Update()
        {
            SimulationManager.Instance.RequestSimulation();
        }

        public override void Draw()
        {
            this.DrawLine(SimulationManager.Instance.LastStage.ActualThrust.ToForce(false) + " / " + SimulationManager.Instance.LastStage.Thrust.ToForce());
        }

        public override void Reset()
        {
            FlightEngineerCore.Instance.AddUpdatable(SimulationManager.Instance);
        }
    }
}