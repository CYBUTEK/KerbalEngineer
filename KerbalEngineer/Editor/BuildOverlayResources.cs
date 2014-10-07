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
using System.Linq;

using KerbalEngineer.Extensions;

using UnityEngine;

#endregion

namespace KerbalEngineer.Editor
{
    public class BuildOverlayResources : MonoBehaviour
    {
        #region Fields

        private static bool visible = true;

        private readonly Dictionary<int, ResourceInfoItem> resources = new Dictionary<int, ResourceInfoItem>();

        private bool open = true;
        private float openPercent;
        private GUIContent tabContent;
        private Rect tabPosition;
        private Vector2 tabSize;
        private Rect windowPosition = new Rect(0.0f, 0.0f, BuildOverlay.MinimumWidth, 0.0f);

        #endregion

        #region Properties

        public static bool Visible
        {
            get { return visible; }
            set { visible = value; }
        }

        public bool Open
        {
            get { return this.open; }
            set { this.open = value; }
        }

        #endregion

        #region Methods: protected

        protected void OnGUI()
        {
            try
            {
                if (!BuildOverlay.Visible || this.resources.Count == 0 || EditorLogic.fetch.editorScreen != EditorLogic.EditorScreen.Parts)
                {
                    return;
                }

                this.open = GUI.Toggle(this.tabPosition, this.open, this.tabContent, BuildOverlay.TabStyle);
                if (this.openPercent > 0.0)
                {
                    this.windowPosition = GUILayout.Window(this.GetInstanceID(), this.windowPosition, this.Window, String.Empty, BuildOverlay.WindowStyle);
                }
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
                this.tabContent = new GUIContent("RESOURCES");
                this.tabSize = BuildOverlay.TabStyle.CalcSize(this.tabContent);
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
                if (!BuildOverlay.Visible)
                {
                    return;
                }

                this.SetResources();
                this.SetSlidePosition();
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }

        #endregion

        #region Methods: private

        private void SetResources()
        {
            var previousCount = this.resources.Count;
            this.resources.Clear();
            foreach (var resource in EditorLogic.fetch.ship.parts.SelectMany(p => p.Resources.list).Where(r => r.amount > 0.0))
            {
                if (this.resources.ContainsKey(resource.info.id))
                {
                    this.resources[resource.info.id].Amount += resource.amount;
                }
                else
                {
                    this.resources.Add(resource.info.id, new ResourceInfoItem(resource));
                }
            }

            if (this.resources.Count < previousCount)
            {
                this.windowPosition.height = 0;
            }
        }

        private void SetSlidePosition()
        {
            if (this.open && this.openPercent < 1.0f)
            {
                this.openPercent = Mathf.Clamp(this.openPercent + Time.deltaTime * BuildOverlay.TabSpeed, 0.0f, 1.0f);
            }
            else if (!this.open && this.openPercent > 0.0f)
            {
                this.openPercent = Mathf.Clamp(this.openPercent - Time.deltaTime * BuildOverlay.TabSpeed, 0.0f, 1.0f);
            }

            this.windowPosition.x = BuildOverlay.BuildOverlayVessel.WindowPosition.xMax + 5.0f;
            this.windowPosition.y = Mathf.Lerp(Screen.height, Screen.height - this.windowPosition.height, this.openPercent);
            this.tabPosition.width = this.tabSize.x;
            this.tabPosition.height = this.tabSize.y;
            this.tabPosition.x = this.windowPosition.x;
            this.tabPosition.y = this.windowPosition.y - this.tabPosition.height;
        }

        private void Window(int windowId)
        {
            try
            {
                var firstItem = true;
                foreach (var resource in this.resources)
                {
                    if (!firstItem)
                    {
                        GUILayout.Space(2.0f);
                    }
                    firstItem = false;

                    GUILayout.BeginHorizontal();

                    GUILayout.Label(resource.Value.Name + ":", BuildOverlay.NameStyle);
                    GUILayout.Space(50.0f);
                    if (resource.Value.Mass > 0.0)
                    {
                        GUILayout.Label("(" + resource.Value.Mass.ToMass() + ") " + resource.Value.Amount.ToString("N1"), BuildOverlay.ValueStyle);
                    }
                    else
                    {
                        GUILayout.Label(resource.Value.Amount.ToString("N1"), BuildOverlay.ValueStyle);
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
    }
}