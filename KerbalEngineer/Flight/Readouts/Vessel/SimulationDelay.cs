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

using UnityEngine;

#endregion

namespace KerbalEngineer.Flight.Readouts.Vessel
{
    public class SimulationDelay : ReadoutModule
    {
        public SimulationDelay()
        {
            this.Name = "Minimum Simulation Delay";
            this.Category = ReadoutCategory.GetCategory("Vessel");
            this.HelpString = "Controls the minimum delay between processing vessel simulations.";
            this.IsDefault = true;
        }

        public override void Draw()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Sim Delay", this.NameStyle);
            GUI.skin = HighLogic.Skin;
            SimManager.minSimTime = (long)GUILayout.HorizontalSlider(SimManager.minSimTime, 0, 1000.0f);
            GUI.skin = null;
            GUILayout.EndHorizontal();
        }
    }
}