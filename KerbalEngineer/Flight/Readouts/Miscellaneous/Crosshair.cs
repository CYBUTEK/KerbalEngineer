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
    public class Crosshair : ReadoutModule
    {
        #region Fields

        private GUIStyle boxStyle;
        private GUIStyle boxStyleHud;
        #endregion

        #region Constructors

        public Crosshair()
        {
            this.Name = "Crosshair";
            this.Category = ReadoutCategory.GetCategory("Miscellaneous");
            this.HelpString = "Creates a cross that can be used as a placeable crosshair.";
            this.IsDefault = false;
            this.Cloneable = true;

            this.InitialiseStyles();

            GuiDisplaySize.OnSizeChanged += this.InitialiseStyles;
        }

        #endregion

        #region Methods: public

        public override void Draw(Unity.Flight.ISectionModule section)
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Box(tex, section.IsHud ? this.boxStyleHud : this.boxStyle);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        #endregion

        #region Methods: private

        private static readonly Texture2D tex = TextureHelper.createCrosshair(new Color(1.0f, 1.0f, 1.0f, 0.75f));
        private static readonly Texture2D bg = TextureHelper.CreateTextureFromColour(new Color(0.0f, 0.0f, 0.0f, 0.0f));

        private void InitialiseStyles()
        {
            this.boxStyle = new GUIStyle
            {
                normal =
                {
                  background  = bg,
                  textColor = new Color(1,1,1,0f)
                },
                active =
                {
                    background  = bg
                },
                border = new RectOffset(0, 0, 0, 0),
                fixedHeight = 0.0f,
                imagePosition = ImagePosition.ImageOnly
            };

            this.boxStyleHud = new GUIStyle(this.boxStyle)
            {
                margin = new RectOffset(0, 0, (int)(8 * GuiDisplaySize.Offset), 0)
            };
        }

        #endregion
    }
}