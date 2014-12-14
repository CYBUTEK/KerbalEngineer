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

#endregion

namespace KerbalEngineer.Flight.Readouts.Surface
{
    #region Using Directives

    using Extensions;
    using Sections;

    #endregion

    public class AltitudeTerrain : ReadoutModule
    {
        #region Constructors

        public AltitudeTerrain()
        {
            this.Name = "Altitude (Terrain)";
            this.Category = ReadoutCategory.GetCategory("Surface");
            this.HelpString = "Shows the vessel's altitude above the terrain.";
            this.IsDefault = true;
        }

        #endregion

        #region Methods

        public override void Draw(SectionModule section)
        {
            if (FlightGlobals.ActiveVessel.terrainAltitude > 0.0)
            {
                this.DrawLine((FlightGlobals.ship_altitude - FlightGlobals.ActiveVessel.terrainAltitude).ToDistance(), section.IsHud);
            }
            else
            {
                this.DrawLine((FlightGlobals.ship_altitude).ToDistance(), section.IsHud);
            }
        }

        #endregion
    }
}