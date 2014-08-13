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

namespace KerbalEngineer.Extensions
{
    public static class RectExtensions
    {
        /// <summary>
        ///     Clamps the rectangle inside the screen region.
        /// </summary>
        public static Rect ClampInsideScreen(this Rect value)
        {
            value.x = Mathf.Clamp(value.x, 0, Screen.width - value.width);
            value.y = Mathf.Clamp(value.y, 0, Screen.height - value.height);

            return value;
        }

        /// <summary>
        ///     Clamps the rectangle into the screen region by the specified margin.
        /// </summary>
        public static Rect ClampToScreen(this Rect value, float margin = 25.0f)
        {
            value.x = Mathf.Clamp(value.x, -(value.width - margin), Screen.width - margin);
            value.y = Mathf.Clamp(value.y, -(value.height - margin), Screen.height - margin);

            return value;
        }

        /// <summary>
        ///     Returns whether the mouse is within the coordinates of this rectangle.
        /// </summary>
        public static bool MouseIsOver(this Rect value)
        {
            return value.Contains(new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y));
        }

        public static Rect Translate(this Rect value, Rect rectangle)
        {
            value.x += rectangle.x;
            value.y += rectangle.y;

            return value;
        }
    }
}