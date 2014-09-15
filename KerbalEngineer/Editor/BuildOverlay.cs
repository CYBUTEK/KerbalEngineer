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

using KerbalEngineer.Helpers;
using KerbalEngineer.Settings;

using UnityEngine;

#endregion

namespace KerbalEngineer.Editor
{
    [KSPAddon(KSPAddon.Startup.EditorAny, false)]
    public class BuildOverlay : MonoBehaviour
    {
        #region Fields

        private static BuildOverlay instance;

        private static float minimumWidth = 200.0f;
        private static GUIStyle nameStyle;
        private static float tabSpeed = 5.0f;
        private static GUIStyle tabStyle;
        private static GUIStyle titleStyle;
        private static GUIStyle valueStyle;
        private static bool visible = true;
        private static GUIStyle windowStyle;

        private BuildOverlayPartInfo buildOverlayPartInfo;
        private BuildOverlayResources buildOverlayResources;
        private BuildOverlayVessel buildOverlayVessel;

        #endregion

        #region Properties

        public static BuildOverlayPartInfo BuildOverlayPartInfo
        {
            get { return instance.buildOverlayPartInfo; }
        }

        public static BuildOverlayResources BuildOverlayResources
        {
            get { return instance.buildOverlayResources; }
        }

        public static BuildOverlayVessel BuildOverlayVessel
        {
            get { return instance.buildOverlayVessel; }
        }

        public static float MinimumWidth
        {
            get { return minimumWidth; }
            set { minimumWidth = value; }
        }

        public static GUIStyle NameStyle
        {
            get
            {
                return nameStyle ?? (nameStyle = new GUIStyle
                {
                    normal =
                    {
                        textColor = Color.white
                    },
                    fontSize = 11,
                    fontStyle = FontStyle.Bold,
                    alignment = TextAnchor.UpperLeft,
                    stretchWidth = true
                });
            }
        }

        public static float TabSpeed
        {
            get { return tabSpeed; }
            set { tabSpeed = value; }
        }

        public static GUIStyle TabStyle
        {
            get
            {
                return tabStyle ?? (tabStyle = new GUIStyle
                {
                    normal =
                    {
                        background = TextureHelper.CreateTextureFromColour(new Color(0.0f, 0.0f, 0.0f, 0.5f)),
                        textColor = Color.yellow
                    },
                    hover =
                    {
                        background = TextureHelper.CreateTextureFromColour(new Color(0.0f, 0.0f, 0.0f, 0.75f)),
                        textColor = Color.yellow
                    },
                    onNormal =
                    {
                        background = TextureHelper.CreateTextureFromColour(new Color(0.0f, 0.0f, 0.0f, 0.5f)),
                        textColor = Color.yellow
                    },
                    onHover =
                    {
                        background = TextureHelper.CreateTextureFromColour(new Color(0.0f, 0.0f, 0.0f, 0.75f)),
                        textColor = Color.yellow
                    },
                    padding = new RectOffset(20, 20, 0, 0),
                    fontSize = 11,
                    fontStyle = FontStyle.Bold,
                    alignment = TextAnchor.MiddleCenter,
                    fixedHeight = 15.0f,
                    stretchWidth = true
                });
            }
        }

        public static GUIStyle TitleStyle
        {
            get
            {
                return titleStyle ?? (titleStyle = new GUIStyle
                {
                    normal =
                    {
                        textColor = Color.yellow
                    },
                    fontSize = 11,
                    fontStyle = FontStyle.Bold,
                    stretchWidth = true
                });
            }
        }

        public static GUIStyle ValueStyle
        {
            get
            {
                return valueStyle ?? (valueStyle = new GUIStyle
                {
                    normal =
                    {
                        textColor = Color.white
                    },
                    fontSize = 11,
                    fontStyle = FontStyle.Normal,
                    alignment = TextAnchor.UpperRight,
                    stretchWidth = true
                });
            }
        }

        public static bool Visible
        {
            get { return visible; }
            set { visible = value; }
        }

        public static GUIStyle WindowStyle
        {
            get
            {
                return windowStyle ?? (windowStyle = new GUIStyle
                {
                    normal =
                    {
                        background = TextureHelper.CreateTextureFromColour(new Color(0.0f, 0.0f, 0.0f, 0.5f))
                    },
                    padding = new RectOffset(5, 5, 3, 3),
                });
            }
        }

        #endregion

        #region Methods: public

        public static void Load()
        {
            var handler = SettingHandler.Load("BuildOverlay.xml");
            handler.GetSet("visible", visible);
            instance.buildOverlayPartInfo.NamesOnly = handler.GetSet("namesOnly", instance.buildOverlayPartInfo.NamesOnly);
            instance.buildOverlayPartInfo.ClickToOpen = handler.GetSet("clickToOpen", instance.buildOverlayPartInfo.ClickToOpen);
            instance.buildOverlayVessel.Open = handler.GetSet("vesselOpen", instance.buildOverlayVessel.Open);
            instance.buildOverlayResources.Open = handler.GetSet("resourcesOpen", instance.buildOverlayResources.Open);
            handler.Save("BuildOverlay.xml");
        }

        public static void Save()
        {
            var handler = SettingHandler.Load("BuildOverlay.xml");
            handler.Set("visible", visible);
            handler.Set("namesOnly", instance.buildOverlayPartInfo.NamesOnly);
            handler.Set("clickToOpen", instance.buildOverlayPartInfo.ClickToOpen);
            handler.Set("vesselOpen", instance.buildOverlayVessel.Open);
            handler.Set("resourcesOpen", instance.buildOverlayResources.Open);
            handler.Save("BuildOverlay.xml");
        }

        #endregion

        #region Methods: protected

        protected void Awake()
        {
            try
            {
                if (instance != null)
                {
                    Destroy(this);
                    return;
                }
                instance = this;
                this.buildOverlayPartInfo = this.gameObject.AddComponent<BuildOverlayPartInfo>();
                this.buildOverlayVessel = this.gameObject.AddComponent<BuildOverlayVessel>();
                this.buildOverlayResources = this.gameObject.AddComponent<BuildOverlayResources>();
                Load();
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }

        protected void OnDestroy()
        {
            try
            {
                Save();
                if (this.buildOverlayPartInfo != null)
                {
                    Destroy(this.buildOverlayPartInfo);
                }
                if (this.buildOverlayVessel != null)
                {
                    Destroy(this.buildOverlayVessel);
                }
                if (this.buildOverlayResources != null)
                {
                    Destroy(this.buildOverlayResources);
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }

        #endregion
    }
}