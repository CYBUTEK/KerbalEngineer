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

using UnityEngine;

#endregion

namespace KerbalEngineer.Flight.Readouts.Surface
{
    public class GeeForce : ReadoutModule
    {
        #region Fields

        private double maxGeeForce;

        #endregion

        #region Constructors

        public GeeForce()
        {
            this.Name = "G-Force";
            this.Category = ReadoutCategory.GetCategory("Surface");
            this.HelpString = "Shows the current g-force and maximum g-force experienced.";
            this.IsDefault = true;
        }

        #endregion

        #region Methods: public

        public override void Draw(Unity.Flight.ISectionModule section)
        {
            if (FlightGlobals.ship_geeForce > this.maxGeeForce)
            {
                this.maxGeeForce = FlightGlobals.ship_geeForce;
            }
            this.DrawLine(() =>
            {
                GUILayout.Label(FlightGlobals.ship_geeForce.ToString("F3") + " / " + this.maxGeeForce.ToString("F3"), this.ValueStyle);
                if (GUILayout.Button("R", this.ButtonStyle, GUILayout.Width(this.ButtonStyle.fixedHeight)))
                {
                    this.maxGeeForce = 0.0;
                }
            });
        }

        public override void Reset()
        {
            this.maxGeeForce = 0;
        }

        #endregion
    }
}