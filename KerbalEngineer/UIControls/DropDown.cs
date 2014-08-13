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

namespace KerbalEngineer.UIControls
{
    public class DropDown : MonoBehaviour
    {
        #region Fields

        private Rect button;
        private Rect position;

        #endregion

        #region Properties

        public bool Resize { get; set; }

        public Callback DrawCallback { get; set; }

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

        private GUIStyle windowStyle;

        private void InitialiseStyles()
        {
            try
            {
                this.windowStyle = new GUIStyle
                {
                    normal =
                    {
                        background = GameDatabase.Instance.GetTexture("KerbalEngineer/Textures/DropDownBackground", false)
                    },
                    border = new RectOffset(8, 8, 1, 8),
                    margin = new RectOffset(),
                    padding = new RectOffset(5, 5, 5, 5)
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
                if ((Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2)) && !this.position.MouseIsOver() && !this.button.MouseIsOver())
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
                if (this.Resize)
                {
                    this.position.height = 0;
                    this.Resize = false;
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
                this.DrawCallback.Invoke();
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }

        #endregion

        #region Public Methods

        public void SetPosition(Rect button)
        {
            try
            {
                this.position.x = button.x;
                this.position.y = button.y + button.height;
                this.position.width = button.width;
                this.button = button;
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }

        #endregion
    }
}