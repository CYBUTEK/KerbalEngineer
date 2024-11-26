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

using UnityEngine;

#endregion

namespace KerbalEngineer.Helpers
{
    public static class TextureHelper
    {
        #region Methods: public

        public static Texture2D CreateTextureFromColour(Color colour)
        {
            var texture = new Texture2D(1, 1, TextureFormat.ARGB32, false);
            texture.SetPixel(1, 1, colour);
            texture.Apply();
            return texture;
        }
        public static Texture2D createCrosshair(Color colour)
        {
            var texture = new Texture2D(17, 17, TextureFormat.ARGB32, false);
       
            for (int i = 0; i < 17; i++)
            {
                for (int j = 0; j < 17; j++)
                {
                    texture.SetPixel(i, j, new Color(0,0,0,0));
                }
            }

            for (int i = 0; i < 17; i++)
            {
                texture.SetPixel(8, i, colour);
            }
            for (int i = 0; i < 17; i++)
            {
                texture.SetPixel(i, 8, colour);
            }
            texture.Apply();
            return texture;
        }
        #endregion
    }
}