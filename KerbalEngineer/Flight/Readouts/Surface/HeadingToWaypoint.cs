﻿//
//     Kerbal Engineer Redux
//
//     Copyright (C) 2017 fat-lobyte
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
using KerbalEngineer.Flight.Readouts.Surface;
using KerbalEngineer.Flight.Sections;

#endregion

namespace KerbalEngineer.Flight.Readouts.Rendezvous
{
    public class HeadingToWaypoint : ReadoutModule
    {
        #region Constructors

        public HeadingToWaypoint()
        {
            this.Name = "Heading to Waypoint";
            this.Category = ReadoutCategory.GetCategory("Surface");
            this.HelpString = "Heading to the waypoint along the surface";
            this.IsDefault = false;
        }

        #endregion

        #region Methods: public

        public override void Draw(SectionModule section)
        {
            if (SurfaceDistanceProcessor.ShowDetails)
            {
                this.DrawLine(SurfaceDistanceProcessor.SurfaceHeadingToTarget.ToAngle(), section.IsHud);
            }
        }

        public override void Reset()
        {
            FlightEngineerCore.Instance.AddUpdatable(SurfaceDistanceProcessor.Instance);
        }

        public override void Update()
        {
            SurfaceDistanceProcessor.RequestUpdate();
        }

        #endregion
    }
}