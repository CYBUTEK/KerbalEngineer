﻿// 
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

using KerbalEngineer.Flight.Sections;

#endregion

namespace KerbalEngineer.Flight.Readouts.Vessel
{
    public class IntakeAirSupply : ReadoutModule
    {
        #region Fields

        private double supply;

        #endregion

        #region Constructors

        public IntakeAirSupply()
        {
            this.Name = "Intake Air (Supply)";
            this.Category = ReadoutCategory.GetCategory("Vessel");
            this.HelpString = "Displays the available intake air.";
            this.IsDefault = false;
        }

        #endregion

        #region Methods: public

        public override void Draw(SectionModule section)
        {
            this.DrawLine(this.supply.ToString("F4"), section.IsHud);
        }

        public override void Update()
        {
            this.supply = IntakeAirDemandSupply.GetSupply();
        }

        #endregion
    }
}