// 
//     Kerbal Engineer Redux
// 
//     Copyright (C) 2016 CYBUTEK
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
//     You should have received a copy of the GNU General Public License
//     along with this program.  If not, see <http://www.gnu.org/licenses/>.
//  

namespace KerbalEngineer.Unity.UI
{
    using UnityEngine;
    using UnityEngine.UI;

    public class StyleApplicator : MonoBehaviour
    {
        public enum ElementTypes
        {
            None,
            Window,
            Box,
            Button,
            ButtonToggle
        }

        [SerializeField]
        private ElementTypes m_ElementType = ElementTypes.None;

        /// <summary>
        ///     Gets the UI element type used by the ThemeManager for selecting how to apply the theme.
        /// </summary>
        public ElementTypes ElementType
        {
            get
            {
                return m_ElementType;
            }
        }

        /// <summary>
        ///     Sets a the applicator to apply the selected sprite to an attached image component.
        /// </summary>
        public void SetImage(Sprite sprite, Image.Type type)
        {
            Image image = GetComponent<Image>();
            if (image == null)
            {
                return;
            }

            image.sprite = sprite;
            image.type = type;
        }

        /// <summary>
        ///     Sets the applicator to apply the specified values to an attached selectable component.
        /// </summary>
        public void SetSelectable(TextStyle textStyle, Sprite normal, Sprite highlight, Sprite pressed, Sprite disabled)
        {
            if (textStyle != null)
            {
                Text text = GetComponentInChildren<Text>();
                if (text != null)
                {
                    if (textStyle.Font != null)
                    {
                        text.font = textStyle.Font;
                    }
                    text.fontStyle = textStyle.Style;
                    text.fontSize = textStyle.Size;
                    text.color = textStyle.Colour;
                }
            }

            Selectable selectable = GetComponent<Selectable>();
            if (selectable != null)
            {
                selectable.image.sprite = normal;
                selectable.image.type = Image.Type.Sliced;

                selectable.transition = Selectable.Transition.SpriteSwap;

                SpriteState spriteState = selectable.spriteState;
                spriteState.highlightedSprite = highlight;
                spriteState.pressedSprite = pressed;
                spriteState.disabledSprite = disabled;
                selectable.spriteState = spriteState;
            }
        }

        /// <summary>
        ///     Sets the applicator to apply the specified values to an attached toggle component.
        /// </summary>
        public void SetToggle(TextStyle textStyle, Sprite normal, Sprite highlight, Sprite pressed, Sprite disabled)
        {
            SetSelectable(textStyle, normal, highlight, pressed, disabled);

            Image toggleImage = GetComponent<Toggle>()?.graphic as Image;
            if (toggleImage != null)
            {
                toggleImage.sprite = pressed;
                toggleImage.type = Image.Type.Sliced;
            }
        }
    }
}