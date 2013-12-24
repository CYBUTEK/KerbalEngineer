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
        /// Clamps the rectangle inside the screen region.
        /// </summary>
        public static Rect ClampInsideScreen(this Rect value)
        {
            value.x = Mathf.Clamp(value.x, 0f, Screen.width - value.width);
            value.y = Mathf.Clamp(value.y, 0f, Screen.height - value.height);

            return value;
        }

        /// <summary>
        /// Clamps the rectangle into the screen region by the specified margin.
        /// </summary>
        public static Rect ClampToScreen(this Rect value, float margin = 25f)
        {
            value.x = Mathf.Clamp(value.x, -(value.width - margin), Screen.width - margin);
            value.y = Mathf.Clamp(value.y, -(value.height - margin), Screen.height - margin);

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
