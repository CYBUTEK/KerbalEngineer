// 
//     Kerbal Engineer Redux
// 
//     Copyright (C) 2014 CYBUTEK
// 
//     This program is free software: you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     This program is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General Public License for more details.
// 
//     You should have received a copy of the GNU General Public License
//     along with this program.  If not, see <http://www.gnu.org/licenses/>.
// 

#region Using Directives

using KerbalEngineer.VesselSimulator;

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
            this.IsDefault = true;
        }

        public override void Update()
        {
            SimManager.RequestUpdate();
        }

        public override void Draw()
        {
            if (SimManager.LastStage == null)
            {
                return;
            }

            this.actual = (SimManager.LastStage.actualThrust / (SimManager.LastStage.totalMass * FlightGlobals.getGeeForceAtPosition(FlightGlobals.ship_position).magnitude)).ToString("F2");
            this.total = (SimManager.LastStage.thrust / (SimManager.LastStage.totalMass * FlightGlobals.getGeeForceAtPosition(FlightGlobals.ship_position).magnitude)).ToString("F2");
            this.DrawLine("TWR", this.actual + " / " + this.total);
        }

        public override void Reset()
        {
            FlightEngineerCore.Instance.AddUpdatable(SimManager.Instance);
        }
    }
}