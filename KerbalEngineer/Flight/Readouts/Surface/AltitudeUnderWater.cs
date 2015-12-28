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

namespace KerbalEngineer.Flight.Readouts.Surface
{
    public class AltitudeUnderWater : ReadoutModule
    {
        #region Constructors

        public AltitudeUnderWater()
        {
            this.Name = "Altitude (Terrain Under Water)";
            this.Category = ReadoutCategory.GetCategory("Surface");
            this.HelpString = "While splashed shows the vessel's altitude to the under water Terrain.";
            this.IsDefault = false;
        }

        #endregion

        #region Methods: public

        public override void Draw(SectionModule section)
        {
			if (ScienceUtil.GetExperimentSituation (FlightGlobals.ActiveVessel) == ExperimentSituations.SrfSplashed) 
			{
				this.DrawLine ((-(FlightGlobals.ActiveVessel.terrainAltitude - FlightGlobals.ship_altitude)).ToDistance (), section.IsHud);
			}
        }

        #endregion
    }
}