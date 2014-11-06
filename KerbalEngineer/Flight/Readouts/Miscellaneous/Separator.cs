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

using KerbalEngineer.Flight.Sections;
using KerbalEngineer.Helpers;

using UnityEngine;

#endregion

namespace KerbalEngineer.Flight.Readouts.Miscellaneous
{
    public class Separator : ReadoutModule
    {
        #region Fields

        private readonly GUIStyle boxStyle;

        #endregion

        #region Constructors

        public Separator()
        {
            this.Name = "Separator";
            this.Category = ReadoutCategory.GetCategory("Miscellaneous");
            this.HelpString = String.Empty;
            this.IsDefault = false;
            this.Cloneable = true;

            this.boxStyle = new GUIStyle
            {
                normal =
                {
                    background = TextureHelper.CreateTextureFromColour(new Color(1.0f, 1.0f, 1.0f, 0.5f))
                },
                fixedHeight = 1.0f,
                stretchWidth = true
            };
        }

        #endregion

        #region Methods: public

        public override void Draw(SectionModule section)
        {
            GUILayout.Box(String.Empty, this.boxStyle);
        }

        #endregion
    }
}