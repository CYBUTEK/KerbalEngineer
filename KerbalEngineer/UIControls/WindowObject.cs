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

using UnityEngine;

#endregion

namespace KerbalEngineer.UIControls
{
    public class WindowObject : MonoBehaviour
    {
        #region Fields

        private Rect position;
        private bool shouldCentre;

        #endregion

        #region Properties

        public bool Draggable { get; set; }

        public Callback DrawCallback { get; set; }

        public float Height
        {
            get { return this.position.height; }
            set { this.position.height = value; }
        }

        public Rect Position
        {
            get { return this.position; }
            set { this.position = value; }
        }

        public string Title { get; set; }

        public Callback UpdateCallback { get; set; }

        public bool Visible { get; set; }

        public float Width
        {
            get { return this.position.width; }
            set { this.position.width = value; }
        }

        public float X
        {
            get { return this.position.x; }
            set { this.position.x = value; }
        }

        public float Y
        {
            get { return this.position.y; }
            set { this.position.y = value; }
        }

        #endregion

        #region Methods: public

        public void Centre()
        {
            this.shouldCentre = true;
        }

        #endregion

        #region Methods: protected

        protected void OnGUI()
        {
            try
            {
                if (!this.Visible)
                {
                    return;
                }
                this.position = GUILayout.Window(this.GetInstanceID(), this.position, this.Window, this.Title, HighLogic.Skin.window);
                this.CentreTheWindow();
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
                if (!this.Visible || this.UpdateCallback == null)
                {
                    return;
                }
                this.UpdateCallback();
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }

        #endregion

        #region Methods: private

        private void CentreTheWindow()
        {
            if (this.shouldCentre && this.position.width > 0.0f && this.position.height > 0.0f)
            {
                this.position.center = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
                this.shouldCentre = false;
            }
        }

        private void Window(int windowId)
        {
            try
            {
                if (this.DrawCallback != null)
                {
                    this.DrawCallback();
                }
                else
                {
                    GUILayout.FlexibleSpace();
                }

                if (this.Draggable)
                {
                    GUI.DragWindow();
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