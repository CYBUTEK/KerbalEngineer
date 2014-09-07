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
using System.Collections.Generic;

using KerbalEngineer.Extensions;

using UnityEngine;

#endregion

namespace KerbalEngineer.Editor
{
    [KSPAddon(KSPAddon.Startup.EditorAny, false)]
    public class BuildPartInfo : MonoBehaviour
    {
        #region Fields

        private readonly List<InfoItem> infoItems = new List<InfoItem>();
        private GUIStyle nameStyle;
        private Rect position;
        private Part selectedPart;
        private GUIStyle titleStyle;
        private GUIStyle valueStyle;
        private GUIStyle windowStyle;

        #endregion

        #region Methods: protected

        protected void OnGUI()
        {
            try
            {
                if (this.selectedPart == null)
                {
                    return;
                }

                this.position = GUILayout.Window(this.GetInstanceID(), this.position, this.Window, String.Empty, this.windowStyle);
            }
            catch (Exception ex)

            {
                Logger.Exception(ex);
            }
        }

        protected void Start()
        {
            try
            {
                this.InitialiseStyles();
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }

        protected void Update()
        {
            try
            {
                this.position.x = Mathf.Clamp(Input.mousePosition.x + 16.0f, 0.0f, Screen.width - this.position.width);
                this.position.y = this.position.x < Input.mousePosition.x + 16.0f ? Screen.height - Input.mousePosition.y + 16.0f : Mathf.Clamp(Screen.height - Input.mousePosition.y, 0.0f, Screen.height - this.position.height);

                this.infoItems.Clear();
                var part = EditorLogic.SortedShipList.Find(p => p.stackIcon.highlightIcon) ?? EditorLogic.SelectedPart;
                if (part != null)
                {
                    if (!part.Equals(this.selectedPart))
                    {
                        this.selectedPart = part;
                        this.position.width = 0;
                        this.position.height = 0;
                    }
                    this.SetMassItems();
                }
                else
                {
                    this.selectedPart = null;
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }

        #endregion

        #region Methods: private

        private Texture2D CreateTextureFromColour(Color colour)
        {
            var texture = new Texture2D(1, 1, TextureFormat.ARGB32, false);
            texture.SetPixel(1, 1, colour);
            texture.Apply();
            return texture;
        }

        private void InitialiseStyles()
        {
            this.windowStyle = new GUIStyle
            {
                normal =
                {
                    background = this.CreateTextureFromColour(new Color(0.0f, 0.0f, 0.0f, 0.5f))
                },
                padding = new RectOffset(5, 5, 3, 3),
            };

            this.titleStyle = new GUIStyle
            {
                normal =
                {
                    textColor = Color.yellow
                },
                fontSize = (int)(11 * GuiDisplaySize.Offset),
                fontStyle = FontStyle.Bold,
                stretchWidth = true
            };

            this.nameStyle = new GUIStyle(this.titleStyle)
            {
                normal =
                {
                    textColor = Color.white
                },
                padding = new RectOffset(0, 0, 2, 0),
                fontStyle = FontStyle.Normal
            };

            this.valueStyle = new GUIStyle(this.nameStyle)
            {
                alignment = TextAnchor.UpperRight
            };
        }

        private void SetMassItems()
        {
            if (this.selectedPart.physicalSignificance == Part.PhysicalSignificance.FULL)
            {
                this.infoItems.Add(new InfoItem("Dry Mass", this.selectedPart.GetDryMass().ToMass()));
            }
            if (this.selectedPart.ContainsResources())
            {
                this.infoItems.Add(new InfoItem("Wet Mass", this.selectedPart.GetWetMass().ToMass()));
                this.SetResourceItems();
            }
        }

        private void SetResourceItems()
        {
            foreach (var resource in this.selectedPart.Resources.list)
            {
                if (resource.GetDensity() > 0)
                {
                    this.infoItems.Add(new InfoItem(resource.info.name, "(" + resource.amount.ToString("F1") + ") " + resource.GetMass().ToMass()));
                }
                else
                {
                    this.infoItems.Add(new InfoItem(resource.info.name, resource.amount.ToString("F1")));
                }
            }
        }

        private void Window(int windowId)
        {
            try
            {
                GUILayout.Label(this.selectedPart.partInfo.title, this.titleStyle);
                if (this.infoItems.Count > 0)
                {
                    GUILayout.Space(5.0f);
                }
                foreach (var item in this.infoItems)
                {
                    
                    GUILayout.BeginHorizontal();
                    if (item.Value != null)
                    {
                        GUILayout.Label(item.Name + ":", this.nameStyle);
                        GUILayout.Space(10.0f);
                        GUILayout.Label(item.Value, this.valueStyle);
                    }
                    else
                    {
                        GUILayout.Label(item.Name, this.nameStyle);
                    }
                    GUILayout.EndHorizontal();
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }

        #endregion

        #region Nested Type: InfoItem

        public class InfoItem
        {
            #region Constructors

            public InfoItem(string name)
            {
                this.Name = name;
            }

            public InfoItem(string name, string value)
            {
                this.Name = name;
                this.Value = value;
            }

            #endregion

            #region Properties

            public string Name { get; set; }

            public string Value { get; set; }

            #endregion
        }

        #endregion
    }
}