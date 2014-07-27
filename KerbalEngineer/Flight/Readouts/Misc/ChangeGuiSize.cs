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

using UnityEngine;

namespace KerbalEngineer.Flight.Readouts.Misc
{
    public class ChangeGuiSize : ReadoutModule
    {
        public ChangeGuiSize()
        {
            this.Name = "Change GUI Size";
            this.Category = ReadoutCategory.Misc;
            this.HelpString = "Shows a control that will allow you to change the GUI size.";
            this.IsDefault = false;
        }

        public override void Draw()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("GUI Size: " + GuiDisplaySize.Increment, this.NameStyle);
            if (GUILayout.Button("<", this.ButtonStyle))
            {
                GuiDisplaySize.Increment--;
            }
            if (GUILayout.Button(">", this.ButtonStyle))
            {
                GuiDisplaySize.Increment++;
            }
            GUILayout.EndHorizontal();
        }
    }
}