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

using KerbalEngineer.Extensions;
using KerbalEngineer.Flight.Sections;

#endregion

namespace KerbalEngineer.Flight.Readouts.Vessel
{
    public class DecelerationDistanceVertical : ReadoutModule
    {
        #region Constructors

        public DecelerationDistanceVertical()
        {
            this.Name = "Decel. Burn: Vert. Dist.";
            this.Category = ReadoutCategory.GetCategory("Vessel");
            this.HelpString = "Vertical distance covered during Deceleration Burn.";
            this.IsDefault = false;
        }

        #endregion

        #region Methods: public

        public override void Draw(SectionModule section)
        {
            if (!DecelerationProcessor.ShowDetails)
            {
                return;
            }

            this.DrawLine(DecelerationProcessor.VerticalDistance.ToDistance(), section.IsHud);
        }

        public override void Reset()
        {
            DecelerationProcessor.Reset();
        }

        public override void Update()
        {
            DecelerationProcessor.RequestUpdate();
        }

        #endregion
    }
}