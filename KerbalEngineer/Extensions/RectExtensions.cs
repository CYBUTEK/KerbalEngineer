// Name:    Kerbal Engineer Redux
// Author:  CYBUTEK
// License: Attribution-NonCommercial-ShareAlike 3.0 Unported

using KerbalEngineer.Settings;
using UnityEngine;

namespace KerbalEngineer.Extensions
{
    public static class RectExtensions
    {
        /// <summary>
        /// Clamps the rectangle into the screen by the specified margin.
        /// </summary>
        public static Rect ClampToScreen(this Rect value, float margin = 25f)
        {
            if (value.x + value.width < margin) value.x = margin - value.width;
            if (value.x > Screen.width - margin) value.x = Screen.width - margin;
            if (value.y + value.height < margin) value.y = margin - value.height;
            if (value.y > Screen.height - margin) value.y = Screen.height - margin;

            return value;
        }

        /// <summary>
        /// Returns whether the mouse is within the coordinates of this rectangle.
        /// </summary>
        public static bool MouseIsOver(this Rect value)
        {
            return value.Contains(new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y));
        }
    }
}
