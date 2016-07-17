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

namespace KerbalEngineer
{
    using Unity;
    using Unity.UI;
    using UnityEngine;
    using UnityEngine.UI;

    public static class StyleManager
    {
        private static GameObject s_WindowPrefab;
        private static GameObject s_SettingPrefab;

        /// <summary>
        ///     Creates a setting on the supplied window.
        /// </summary>
        public static Setting CreateSetting(string label, Window window)
        {
            Setting setting = null;

            GameObject settingPrefab = GetSettingPrefab();

            if (settingPrefab != null && window != null)
            {
                GameObject settingObject = Object.Instantiate(settingPrefab);

                if (settingObject != null)
                {
                    setting = settingObject.GetComponent<Setting>();
                    if (setting != null)
                    {
                        setting.SetLabel(label);
                        window.AddToContent(settingObject);
                    }
                }
            }

            return setting;
        }

        /// <summary>
        ///     Creates and returns a new window object.
        /// </summary>
        public static Window CreateWindow(string title, float width)
        {
            GameObject windowPrefab = GetWindowPrefab();
            if (windowPrefab == null)
            {
                return null;
            }

            GameObject windowObject = Object.Instantiate(windowPrefab);
            if (windowObject == null)
            {
                return null;
            }

            // process style applicators
            Process(windowObject);

            // assign game object to be a child of the main canvas
            windowObject.transform.SetParent(MainCanvasUtil.MainCanvas.transform, false);

            // set window values
            Window window = windowObject.GetComponent<Window>();
            if (window != null)
            {
                window.SetTitle(title);
                window.SetWidth(width);
            }

            return window;
        }

        /// <summary>
        ///     Processes all of the style applicators on the supplied game object.
        /// </summary>
        public static void Process(GameObject gameObject)
        {
            if (gameObject == null)
            {
                return;
            }

            StyleApplicator[] applicators = gameObject.GetComponentsInChildren<StyleApplicator>();

            if (applicators != null)
            {
                for (int i = 0; i < applicators.Length; i++)
                {
                    Process(applicators[i]);
                }
            }
        }

        /// <summary>
        ///     Processes all the style applicators on the supplied component's game object.
        /// </summary>
        public static void Process(Component component)
        {
            if (component != null)
            {
                Process(component.gameObject);
            }
        }

        /// <summary>
        ///     Gets a setting prefab object.
        /// </summary>
        private static GameObject GetSettingPrefab()
        {
            if (s_SettingPrefab == null)
            {
                s_SettingPrefab = AssetBundleLoader.Prefabs.LoadAsset<GameObject>("Setting");
            }

            return s_SettingPrefab;
        }

        /// <summary>
        ///     Gets a new ThemeTextStyle created from KSP UIStyle and UIStyleState objects.
        /// </summary>
        private static TextStyle GetTextStyle(UIStyle style, UIStyleState styleState)
        {
            TextStyle textStyle = new TextStyle();

            if (style != null)
            {
                textStyle.Font = style.font;
                textStyle.Style = style.fontStyle;
                textStyle.Size = style.fontSize;
            }

            if (styleState != null)
            {
                textStyle.Colour = styleState.textColor;
            }

            return textStyle;
        }

        /// <summary>
        ///     Gets a window prefab object.
        /// </summary>
        private static GameObject GetWindowPrefab()
        {
            if (s_WindowPrefab == null)
            {
                s_WindowPrefab = AssetBundleLoader.Prefabs.LoadAsset<GameObject>("Window");
            }

            return s_WindowPrefab;
        }

        /// <summary>
        ///     Processes a theme on the supplied applicator.
        /// </summary>
        private static void Process(StyleApplicator applicator)
        {
            if (applicator == null)
            {
                return;
            }

            // get the default skin
            UISkinDef skin = UISkinManager.defaultSkin;
            if (skin == null)
            {
                return;
            }

            // apply selected theme type
            switch (applicator.ElementType)
            {
                case StyleApplicator.ElementTypes.Window:
                    applicator.SetImage(skin.window.normal.background, Image.Type.Sliced);
                    break;

                case StyleApplicator.ElementTypes.Box:
                    applicator.SetImage(skin.box.normal.background, Image.Type.Sliced);
                    break;

                case StyleApplicator.ElementTypes.Button:
                    applicator.SetSelectable(null, skin.button.normal.background,
                        skin.button.highlight.background,
                        skin.button.active.background,
                        skin.button.disabled.background);
                    break;

                case StyleApplicator.ElementTypes.ButtonToggle:
                    applicator.SetToggle(null, skin.button.normal.background,
                        skin.button.highlight.background,
                        skin.button.active.background,
                        skin.button.disabled.background);
                    break;

                case StyleApplicator.ElementTypes.Label:
                    applicator.SetText(GetTextStyle(skin.label, skin.label.normal));
                    break;
            }
        }
    }
}