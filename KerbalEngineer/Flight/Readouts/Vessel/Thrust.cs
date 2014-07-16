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
            this.HelpString = string.Empty;
            this.IsDefault = true;
        }

        public override void Update()
        {
            SimManager.RequestUpdate();
        }

        public override void Draw()
        {
            this.DrawLine(SimManager.LastStage.actualThrust.ToForce(false) + " / " + SimManager.LastStage.thrust.ToForce());
        }

        public override void Reset()
        {
            FlightEngineerCore.Instance.AddUpdatable(SimManager.Instance);
        }
    }
}