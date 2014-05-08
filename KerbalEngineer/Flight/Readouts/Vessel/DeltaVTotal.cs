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
            SimManager.RequestUpdate();
        }

        public override void Draw()
        {
            this.DrawLine(SimManager.LastStage.totalDeltaV.ToString("N0") + "m/s");
        }

        public override void Reset()
        {
            FlightEngineerCore.Instance.AddUpdatable(SimManager.Instance);
        }
    }
}