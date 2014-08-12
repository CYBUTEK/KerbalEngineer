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

using KerbalEngineer.Extensions;

using UnityEngine;

#endregion

namespace KerbalEngineer.Editor
{
    public class BuildBodiesList : MonoBehaviour
    {
        #region Fields

        private Rect position;
        private bool resize;

        #endregion

        #region Properties

        public Rect Position
        {
            get { return this.position; }
        }

        #endregion

        #region Initialisation

        private void Awake()
        {
            try
            {
                this.enabled = false;
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }

        private void Start()
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

        #endregion

        #region Styles

        private GUIStyle buttonActiveStyle;
        private GUIStyle buttonStyle;
        private GUIStyle windowStyle;

        private void InitialiseStyles()
        {
            try
            {
                this.windowStyle = new GUIStyle
                {
                    normal =
                    {
                        background = GameDatabase.Instance.GetTexture("KerbalEngineer/Textures/BodyListBackground", false)
                    },
                    border = new RectOffset(8, 8, 0, 8),
                    margin = new RectOffset(),
                    padding = new RectOffset(5, 5, 5, 5)
                };

                this.buttonStyle = new GUIStyle(HighLogic.Skin.button)
                {
                    margin = new RectOffset(0, 0, 2, 0),
                    padding = new RectOffset(5, 5, 5, 5),
                    normal =
                    {
                        textColor = Color.white
                    },
                    active =
                    {
                        textColor = Color.white
                    },
                    fontSize = (int)(11 * GuiDisplaySize.Offset),
                    fontStyle = FontStyle.Bold,
                    alignment = TextAnchor.MiddleCenter,
                    fixedHeight = 20.0f
                };

                this.buttonActiveStyle = new GUIStyle(this.buttonStyle)
                {
                    normal = this.buttonStyle.active
                };
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }

        #endregion

        #region Updating

        private void Update()
        {
            if ((Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2)) && !this.position.MouseIsOver())
            {
                this.enabled = false;
            }
        }

        #endregion

        #region Drawing

        private void OnGUI()
        {
            try
            {
                if (this.resize)
                {
                    this.position.height = 0;
                    this.resize = false;
                }
                GUI.skin = null;
                this.position = GUILayout.Window(this.GetInstanceID(), this.position, this.Window, string.Empty, this.windowStyle);
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }

        private void Window(int windowId)
        {
            try
            {
                GUI.BringWindowToFront(windowId);
                if (CelestialBodies.SystemBody == CelestialBodies.SelectedBody)
                {
                    this.DrawBodies(CelestialBodies.SystemBody);
                }
                else
                {
                    foreach (var body in CelestialBodies.SystemBody.Children)
                    {
                        this.DrawBodies(body);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }

        private void DrawBodies(CelestialBodies.BodyInfo bodyInfo, int depth = 0)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(20.0f * depth);
            if (GUILayout.Button(bodyInfo.Children.Count > 0 ? bodyInfo.Name + " [" + bodyInfo.Children.Count + "]" : bodyInfo.Name, bodyInfo.Selected && bodyInfo.SelectedDepth == 0 ? this.buttonActiveStyle : this.buttonStyle))
            {
                CelestialBodies.SetSelectedBody(bodyInfo.Name);
                this.resize = true;
            }
            GUILayout.EndHorizontal();

            if (bodyInfo.Selected)
            {
                foreach (var body in bodyInfo.Children)
                {
                    this.DrawBodies(body, depth + 1);
                }
            }
        }

        #endregion

        #region Public Methods

        public void SetPosition(float x, float y, float width)
        {
            try
            {
                this.position.x = x;
                this.position.y = y;
                this.position.width = width;
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }

        #endregion
    }
}