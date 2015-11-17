﻿// 
//     Kerbal Engineer Redux
// 
//     Copyright (C) 2015 CYBUTEK
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

namespace KerbalEngineer.Flight.Readouts.Thermal
{
    using Helpers;
    using Sections;

    public class CoolestTemperature : ReadoutModule
    {
        public CoolestTemperature()
        {
            Name = "Coolest Temperature";
            Category = ReadoutCategory.GetCategory("Thermal");
            HelpString = "Lowest internal Temperature on the Vessel.";
            IsDefault = true;
        }

        public override void Draw(SectionModule section)
        {
            if (ThermalProcessor.ShowDetails)
            {
                DrawLine(Units.ToTemperature(ThermalProcessor.CoolestTemperature, ThermalProcessor.CoolestTemperatureMax), section.IsHud);
            }
        }

        public override void Reset()
        {
            FlightEngineerCore.Instance.AddUpdatable(ThermalProcessor.Instance);
        }

        public override void Update()
        {
            ThermalProcessor.RequestUpdate();
        }
    }
}