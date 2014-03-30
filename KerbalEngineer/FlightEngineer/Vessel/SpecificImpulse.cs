// Project:	KerbalEngineer
// Author:	CYBUTEK
// License:	Attribution-NonCommercial-ShareAlike 3.0 Unported

#region Using Directives

using KerbalEngineer.Simulation;

#endregion

namespace KerbalEngineer.FlightEngineer.Vessel
{
    public class SpecificImpulse : Readout
    {
        protected override void Initialise()
        {
            this.Name = "Specific Impulse";
            this.Description = "Shows the specific impulse.";
            this.Category = ReadoutCategory.Vessel;
        }

        public override void Update()
        {
            SimulationManager.RequestSimulation();;
        }

        public override void Draw()
        {          
            this.DrawLine(SimulationManager.LastStage.Isp.ToString("F1") + "s");
        }
    }
}