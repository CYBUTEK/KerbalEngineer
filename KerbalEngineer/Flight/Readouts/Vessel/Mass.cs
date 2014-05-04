// Project:	KerbalEngineer
// Author:	CYBUTEK
// License:	Attribution-NonCommercial-ShareAlike 3.0 Unported

#region Using Directives

using KerbalEngineer.Extensions;
using KerbalEngineer.Simulation;

#endregion

namespace KerbalEngineer.Flight.Readouts.Vessel
{
    public class Mass : ReadoutModule
    {
        public Mass()
        {
            this.Name = "Mass";
            this.Category = ReadoutCategory.Vessel;
        }

        public override void Update()
        {
            SimulationManager.Instance.RequestSimulation();
        }

        public override void Draw()
        {
            this.DrawLine(SimulationManager.Instance.LastStage.mass.ToMass(false) + " / " + SimulationManager.Instance.LastStage.totalMass.ToMass());
        }

        public override void Reset()
        {
            FlightEngineerCore.Instance.AddUpdatable(SimulationManager.Instance);
        }
    }
}