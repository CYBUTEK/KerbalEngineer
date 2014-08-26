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

using System.Linq;

#endregion

namespace KerbalEngineer.Flight.Readouts.Vessel
{
    public class IntakeAirSupplyDemand : ReadoutModule
    {
        private double supply;
        private double demand;

        public IntakeAirSupplyDemand()
        {
            this.Name = "Intake Air (S/D)";
            this.Category = ReadoutCategory.GetCategory("Vessel");
            this.HelpString = string.Empty;
            this.IsDefault = true;
        }

        public override void Update()
        {
            foreach (var part in FlightGlobals.ActiveVessel.Parts.Where(part => part.Modules.Contains("ModuleEngines") && (part.Modules["ModuleEngines"] as ModuleEngines).propellants.Exists(prop => prop.name == "IntakeAir")))
            {
                if (part.inverseStage == Staging.CurrentStage)
                {
                    var engine = (part.Modules["ModuleEngines"] as ModuleEngines);
                    foreach (var propellant in engine.propellants)
                    {
                        if (propellant.name == "IntakeAir")
                        {
                            this.supply = propellant.currentAmount;
                            this.demand = propellant.currentRequirement;
                        }
                    }
                }
            }
        }

        public override void Draw()
        {
            this.DrawLine(this.supply.ToString("F3") + " / " + this.demand.ToString("F3"));
        }
    }
}