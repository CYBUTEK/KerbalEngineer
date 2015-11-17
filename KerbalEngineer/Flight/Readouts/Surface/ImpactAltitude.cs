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

using KerbalEngineer.Extensions;
using KerbalEngineer.Flight.Sections;

#endregion

namespace KerbalEngineer.Flight.Readouts.Surface
{
    public class ImpactAltitude : ReadoutModule
    {
        #region Constructors

        public ImpactAltitude()
        {
            this.Name = "Impact Altitude";
            this.Category = ReadoutCategory.GetCategory("Surface");
            this.HelpString = "Altitude at which the Vessel will impact.";
            this.IsDefault = false;
        }

        #endregion

        #region Methods: public

        public override void Draw(SectionModule section)
        {
            if (ImpactProcessor.ShowDetails)
            {
                this.DrawLine(ImpactProcessor.Altitude.ToDistance(), section.IsHud);
            }
        }

        public override void Reset()
        {
            FlightEngineerCore.Instance.AddUpdatable(ImpactProcessor.Instance);
        }

        public override void Update()
        {
            ImpactProcessor.RequestUpdate();
        }

        #endregion
    }
}