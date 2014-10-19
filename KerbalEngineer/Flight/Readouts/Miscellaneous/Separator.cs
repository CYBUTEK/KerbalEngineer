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

using KerbalEngineer.Flight.Sections;

using UnityEngine;

#endregion

namespace KerbalEngineer.Flight.Readouts.Miscellaneous
{
    public class Separator : ReadoutModule
    {
        #region Fields

        private readonly Texture2D texture;

        #endregion

        #region Constructors

        public Separator()
        {
            this.Name = "Separator";
            this.Category = ReadoutCategory.GetCategory("Miscellaneous");
            this.HelpString = string.Empty;
            this.IsDefault = false;
            this.Cloneable = true;

            this.texture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
            this.texture.SetPixel(0, 0, new Color(1.0f, 1.0f, 1.0f, 0.5f));
            this.texture.Apply();
        }

        #endregion

        #region Methods: public

        public override void Draw(SectionModule section)
        {
            GUILayout.Box(string.Empty, GUIStyle.none, new[] {GUILayout.Width(this.ContentWidth), GUILayout.Height(1.0f)});
            GUI.DrawTexture(GUILayoutUtility.GetLastRect(), this.texture, ScaleMode.StretchToFill);
        }

        #endregion
    }
}