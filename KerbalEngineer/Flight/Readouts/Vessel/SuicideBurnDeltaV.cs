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

using System;

using KerbalEngineer.Extensions;
using KerbalEngineer.Flight.Sections;

#endregion

namespace KerbalEngineer.Flight.Readouts.Vessel
{
    public class SuicideBurnDeltaV : ReadoutModule
    {
        #region Constructors

        public SuicideBurnDeltaV()
        {
            this.Name = "Suicide Burn (dV)";
            this.Category = ReadoutCategory.GetCategory("Vessel");
            this.HelpString = "Shows the DeltaV of a suicide burn.";
            this.IsDefault = true;
        }

        #endregion

        #region Methods: public

        public override void Draw(SectionModule section)
        {
            if (!SuicideBurnProcessor.ShowDetails)
            {
                return;
            }

            this.DrawLine(SuicideBurnProcessor.DeltaV.ToString("N1") + "m/s", section.IsHud);
        }

        public override void Reset()
        {
            SuicideBurnProcessor.Reset();
        }

        public override void Update()
        {
            SuicideBurnProcessor.RequestUpdate();
        }

        #endregion
    }
}