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
using KerbalEngineer.Flight.Readouts;

using UnityEngine;

#endregion

namespace KerbalEngineer.Flight.Sections
{
    public class SectionEditorCategoryList : MonoBehaviour
    {
        #region Fields

        private Rect position;
        private bool resize;

        #endregion

        #region properties

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
                    border = new RectOffset(8, 8, 1, 8),
                    margin = new RectOffset(),
                    padding = new RectOffset(5, 5, 5, 5)
                };

                this.buttonStyle = new GUIStyle(HighLogic.Skin.button)
                {
                    normal =
                    {
                        textColor = Color.white
                    },
                    alignment = TextAnchor.MiddleCenter,
                    fontSize = 12,
                    fontStyle = FontStyle.Bold,
                    fixedHeight = 30.0f,
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
            try
            {
                if ((Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2)) && !this.position.MouseIsOver())
                {
                    this.enabled = false;
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
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
                foreach (var category in ReadoutCategory.Categories)
                {
                    if (GUILayout.Button(category.Name, category == ReadoutCategory.Selected ? this.buttonActiveStyle : this.buttonStyle))
                    {
                        ReadoutCategory.Selected = category;
                        this.enabled = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }

        #endregion

        #region Public Methods

        public void SetPosition(Rect position)
        {
            try
            {
                this.position.x = position.x;
                this.position.y = position.y + position.height;
                this.position.width = position.width;
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }

        #endregion
    }
}