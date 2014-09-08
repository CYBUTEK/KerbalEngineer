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
using System.Diagnostics;

using KerbalEngineer.Settings;
using KerbalEngineer.VesselSimulator;

using UnityEngine;

#endregion

namespace KerbalEngineer.Editor
{
    [KSPAddon(KSPAddon.Startup.EditorAny, false)]
    public class BuildOverlay : MonoBehaviour
    {
        #region Fields

        private GUIStyle infoStyle;
        private Stage lastStage;

        private GUIStyle titleStyle;
        private bool visible = true;
        private int windowId;
        private Rect windowPosition = new Rect(300.0f, 0, 0, 0);
        private GUIStyle windowStyle;

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the current instance if started or returns null.
        /// </summary>
        public static BuildOverlay Instance { get; private set; }


        /// <summary>
        ///     Gets and sets whether the display is enabled.
        /// </summary>
        public bool Visible
        {
            get { return this.visible; }
            set { this.visible = value; }
        }

        #endregion

        #region Methods: protected

        protected void Awake()
        {
            try
            {
                Instance = this;
                GuiDisplaySize.OnSizeChanged += this.OnSizeChanged;
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }

        /// <summary>
        ///     Saves the settings when this object is destroyed.
        /// </summary>
        protected void OnDestroy()
        {
            try
            {
                this.Save();
                GuiDisplaySize.OnSizeChanged -= this.OnSizeChanged;
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }

        protected void OnGUI()
        {
            try
            {
                if (!this.visible || EditorLogic.fetch == null || EditorLogic.fetch.ship.parts.Count == 0 || EditorLogic.fetch.editorScreen != EditorLogic.EditorScreen.Parts)
                {
                    return;
                }

                if (SimManager.ResultsReady())
                {
                    this.lastStage = SimManager.LastStage;
                }

                SimManager.RequestSimulation();

                if (this.lastStage == null)
                {
                    return;
                }

                GUI.skin = null;
                this.windowPosition = GUILayout.Window(this.windowId, this.windowPosition, this.Window, string.Empty, this.windowStyle);

                // Check and set that the window is at the bottom of the screen.
                if (this.windowPosition.y + this.windowPosition.height != Screen.height - 5.0f)
                {
                    this.windowPosition.y = Screen.height - this.windowPosition.height - 5.0f;
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
                this.windowId = this.GetHashCode();
                this.InitialiseStyles();
                this.Load();
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
                if (!this.visible || BuildAdvanced.Instance == null || EditorLogic.fetch == null || EditorLogic.fetch.ship.parts.Count == 0 || EditorLogic.fetch.editorScreen != EditorLogic.EditorScreen.Parts)
                {
                    return;
                }

                // Configure the simulation parameters based on the selected reference body.
                SimManager.Gravity = CelestialBodies.SelectedBody.Gravity;

                if (BuildAdvanced.Instance.ShowAtmosphericDetails)
                {
                    SimManager.Atmosphere = CelestialBodies.SelectedBody.Atmosphere * 0.01d;
                }
                else
                {
                    SimManager.Atmosphere = 0;
                }

                SimManager.TryStartSimulation();
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }

        #endregion

        #region Methods: private

        private void InitialiseStyles()
        {
            this.windowStyle = new GUIStyle(GUIStyle.none)
            {
                margin = new RectOffset(),
                padding = new RectOffset()
            };

            this.titleStyle = new GUIStyle(HighLogic.Skin.label)
            {
                normal =
                {
                    textColor = Color.white
                },
                margin = new RectOffset(),
                padding = new RectOffset(),
                fontSize = (int)(11 * GuiDisplaySize.Offset),
                fontStyle = FontStyle.Bold,
                stretchWidth = true
            };

            this.infoStyle = new GUIStyle(HighLogic.Skin.label)
            {
                margin = new RectOffset(),
                padding = new RectOffset(),
                fontSize = (int)(11 * GuiDisplaySize.Offset),
                fontStyle = FontStyle.Bold,
                stretchWidth = true
            };

        }

        private void Load()
        {
            var handler = SettingHandler.Load("BuildOverlay.xml");
            handler.Get("visible", ref this.visible);
        }

        private void OnSizeChanged()
        {
            this.InitialiseStyles();
            this.windowPosition.width = 0;
            this.windowPosition.height = 0;
        }

        private void Save()
        {
            var handler = new SettingHandler();
            handler.Set("visible", this.visible);
            handler.Save("BuildOverlay.xml");
        }

        private void Window(int windowId)
        {
            try
            {
                GUILayout.BeginHorizontal();

                // Titles
                GUILayout.BeginVertical(GUILayout.Width(75.0f * GuiDisplaySize.Offset));
                GUILayout.Label("Parts:", this.titleStyle);
                GUILayout.Label("Delta-V:", this.titleStyle);
                GUILayout.Label("TWR:", this.titleStyle);
                GUILayout.EndVertical();

                // Details
                GUILayout.BeginVertical(GUILayout.Width(100.0f * GuiDisplaySize.Offset));
                GUILayout.Label(this.lastStage.partCount.ToString("N0"), this.infoStyle);
                GUILayout.Label(this.lastStage.totalDeltaV.ToString("N0") + " m/s", this.infoStyle);
                GUILayout.Label(this.lastStage.thrustToWeight.ToString("F2"), this.infoStyle);
                GUILayout.EndVertical();

                GUILayout.EndHorizontal();
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }

        #endregion
    }
}