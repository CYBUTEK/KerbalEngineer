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
            SimManager.RequestUpdate();
        }

        public override void Draw()
        {
            this.DrawLine(SimManager.LastStage.mass.ToMass(false) + " / " + SimManager.LastStage.totalMass.ToMass());
        }

        public override void Reset()
        {
            FlightEngineerCore.Instance.AddUpdatable(SimManager.Instance);
        }
    }
}