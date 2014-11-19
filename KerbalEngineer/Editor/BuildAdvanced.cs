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
using System.Linq;

using KerbalEngineer.Extensions;
using KerbalEngineer.Flight;
using KerbalEngineer.Helpers;
using KerbalEngineer.Settings;
using KerbalEngineer.UIControls;
using KerbalEngineer.VesselSimulator;

using UnityEngine;

#endregion

namespace KerbalEngineer.Editor
{
    [KSPAddon(KSPAddon.Startup.EditorAny, false)]
    public class BuildAdvanced : MonoBehaviour
    {
        #region Fields

        private GUIStyle areaSettingStyle;
        private GUIStyle areaStyle;
        private float atmosphericPercentage = 1.0f;
        private float atmosphericVelocity;
        private GUIStyle bodiesButtonActiveStyle;
        private GUIStyle bodiesButtonStyle;
        private DropDown bodiesList;
        private Rect bodiesListPosition;
        private GUIStyle buttonStyle;
        private int compactCheck;
        private bool compactCollapseRight;
        private bool compactMode;
        private float compactRight;
        private bool hasChanged;
        private GUIStyle infoStyle;
        private bool isEditorLocked;
        private int numberOfStages;
        private Rect position = new Rect(265.0f, 45.0f, 0, 0);
        private GUIStyle settingAtmoStyle;
        private GUIStyle settingStyle;
        private bool showAllStages;
        private bool showAtmosphericDetails;
        private bool showSettings;
        private Stage[] stages;
        private GUIStyle titleStyle;
        private bool visible = true;
        private GUIStyle windowStyle;

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the current instance if started or returns null.
        /// </summary>
        public static BuildAdvanced Instance { get; private set; }

        /// <summary>
        ///     Gets and sets whether to show in compact mode.
        /// </summary>
        public bool CompactMode
        {
            get { return this.compactMode; }
            set { this.compactMode = value; }
        }

        /// <summary>
        ///     Gets and sets whether to show all stages.
        /// </summary>
        public bool ShowAllStages
        {
            get { return this.showAllStages; }
            set { this.showAllStages = value; }
        }

        /// <summary>
        ///     Gets and sets whether to use atmospheric details.
        /// </summary>
        public bool ShowAtmosphericDetails
        {
            get { return this.showAtmosphericDetails; }
            set { this.showAtmosphericDetails = value; }
        }

        /// <summary>
        ///     Gets and sets whether to show the settings display.
        /// </summary>
        public bool ShowSettings
        {
            get { return this.showSettings; }
            set { this.showSettings = value; }
        }

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
                this.bodiesList = this.gameObject.AddComponent<DropDown>();
                this.bodiesList.DrawCallback = this.DrawBodiesList;
                this.Load();
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
                var handler = new SettingHandler();
                handler.Set("visible", this.visible);
                handler.Set("windowPositionX", this.position.x);
                handler.Set("windowPositionY", this.position.y);
                handler.Set("compactMode", this.compactMode);
                handler.Set("compactCollapseRight", this.compactCollapseRight);
                handler.Set("showAllStages", this.showAllStages);
                handler.Set("showAtmosphericDetails", this.showAtmosphericDetails);
                handler.Set("showSettings", this.showSettings);
                handler.Set("selectedBodyName", CelestialBodies.SelectedBody.Name);
                handler.Save("BuildAdvanced.xml");
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
                    this.stages = SimManager.Stages;
                }

                SimManager.RequestSimulation();

                if (this.stages == null)
                {
                    return;
                }

                // Change the window title based on whether in compact mode or not.
                var title = !this.compactMode ? "KERBAL ENGINEER REDUX " + EngineerGlobals.AssemblyVersion : "K.E.R. " + EngineerGlobals.AssemblyVersion + (this.showAtmosphericDetails ? " (ATMOS.)" : String.Empty);

                // Reset the window size when the staging or something else has changed.
                var stageCount = this.stages.Count(stage => this.showAllStages || stage.deltaV > 0);
                if (this.hasChanged || stageCount != this.numberOfStages)
                {
                    this.hasChanged = false;
                    this.numberOfStages = stageCount;

                    this.position.width = 0;
                    this.position.height = 0;
                }

                GUI.skin = null;
                this.position = GUILayout.Window(this.GetInstanceID(), this.position, this.Window, title, this.windowStyle).ClampToScreen();

                if (this.compactCheck > 0 && this.compactCollapseRight)
                {
                    this.position.x = this.compactRight - this.position.width;
                    this.compactCheck--;
                }
                else if (this.compactCheck > 0)
                {
                    this.compactCheck = 0;
                }

                // Check editor lock to manage click-through.
                this.CheckEditorLock();
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
                GuiDisplaySize.OnSizeChanged += this.OnSizeChanged;
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
                if (Input.GetKeyDown(KeyBinder.EditorShowHide))
                {
                    this.visible = !this.visible;
                    if (!this.visible)
                    {
                        this.EditorLock(false);
                    }
                }

                if (!this.visible || EditorLogic.fetch == null || EditorLogic.fetch.ship.parts.Count == 0)
                {
                    this.bodiesList.enabled = false;
                    return;
                }

                // Configure the simulation parameters based on the selected reference body.
                SimManager.Gravity = CelestialBodies.SelectedBody.Gravity;

                if (this.showAtmosphericDetails)
                {
                    SimManager.Atmosphere = CelestialBodies.SelectedBody.Atmosphere * 0.01d * this.atmosphericPercentage;
                }
                else
                {
                    SimManager.Atmosphere = 0;
                }

                SimManager.Velocity = this.atmosphericVelocity;
                SimManager.TryStartSimulation();
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "BuildAdvanced->Update");
            }
        }

        #endregion

        #region Methods: private

        /// <summary>
        ///     Checks whether the editor should be locked to stop click-through.
        /// </summary>
        private void CheckEditorLock()
        {
            if ((this.position.MouseIsOver() || this.bodiesList.Position.MouseIsOver()) && !this.isEditorLocked)
            {
                this.EditorLock(true);
            }
            else if (!this.position.MouseIsOver() && !this.bodiesList.Position.MouseIsOver() && this.isEditorLocked)
            {
                this.EditorLock(false);
            }
        }

        /// <summary>
        ///     Draws the atmospheric settings.
        /// </summary>
        private void DrawAtmosphericDetails()
        {
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();
            GUILayout.Label("Pressure: " + (this.atmosphericPercentage * 100.0f).ToString("F1") + "%", this.settingAtmoStyle, GUILayout.Width(125.0f * GuiDisplaySize.Offset));
            GUI.skin = HighLogic.Skin;
            this.atmosphericPercentage = GUILayout.HorizontalSlider(this.atmosphericPercentage, 0, 1.0f);
            GUI.skin = null;
            GUILayout.EndVertical();

            GUILayout.Space(5.0f);

            GUILayout.BeginVertical();
            GUILayout.Label("Velocity: " + this.atmosphericVelocity.ToString("F1") + "m/s", this.settingAtmoStyle, GUILayout.Width(125.0f * GuiDisplaySize.Offset));
            GUI.skin = HighLogic.Skin;
            this.atmosphericVelocity = GUILayout.HorizontalSlider(this.atmosphericVelocity, 0, 2500f);
            GUI.skin = null;
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }

        private void DrawBodiesList()
        {
            if (CelestialBodies.SystemBody == CelestialBodies.SelectedBody)
            {
                this.DrawBody(CelestialBodies.SystemBody);
            }
            else
            {
                foreach (var body in CelestialBodies.SystemBody.Children)
                {
                    this.DrawBody(body);
                }
            }
        }

        private void DrawBody(CelestialBodies.BodyInfo bodyInfo, int depth = 0)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(20.0f * depth);
            if (GUILayout.Button(bodyInfo.Children.Count > 0 ? bodyInfo.Name + " [" + bodyInfo.Children.Count + "]" : bodyInfo.Name, bodyInfo.Selected && bodyInfo.SelectedDepth == 0 ? this.bodiesButtonActiveStyle : this.bodiesButtonStyle))
            {
                CelestialBodies.SetSelectedBody(bodyInfo.Name);
                this.bodiesList.Resize = true;
            }
            GUILayout.EndHorizontal();

            if (bodyInfo.Selected)
            {
                foreach (var body in bodyInfo.Children)
                {
                    this.DrawBody(body, depth + 1);
                }
            }
        }

        /// <summary>
        ///     Draws the burn time column.
        /// </summary>
        private void DrawBurnTime()
        {
            GUILayout.BeginVertical(GUILayout.Width(75.0f * GuiDisplaySize.Offset));
            GUILayout.Label("BURN", this.titleStyle);
            foreach (var stage in this.stages)
            {
                if (this.showAllStages || stage.deltaV > 0)
                {
                    GUILayout.Label(TimeFormatter.ConvertToString(stage.time), this.infoStyle);
                }
            }
            GUILayout.EndVertical();
        }

        /// <summary>
        ///     Draws the cost column.
        /// </summary>
        private void DrawCost()
        {
            GUILayout.BeginVertical(GUILayout.Width(110.0f * GuiDisplaySize.Offset));
            GUILayout.Label("COST", this.titleStyle);
            foreach (var stage in this.stages)
            {
                if (this.showAllStages || stage.deltaV > 0)
                {
                    GUILayout.Label(Units.Concat(stage.cost, stage.totalCost), this.infoStyle);
                }
            }
            GUILayout.EndVertical();
        }

        /// <summary>
        ///     Draws the deltaV column.
        /// </summary>
        private void DrawDeltaV()
        {
            GUILayout.BeginVertical(GUILayout.Width(100.0f * GuiDisplaySize.Offset));
            GUILayout.Label("DELTA-V", this.titleStyle);
            foreach (var stage in this.stages)
            {
                if (this.showAllStages || stage.deltaV > 0)
                {
                    GUILayout.Label(stage.deltaV.ToString("N0") + " / " + stage.inverseTotalDeltaV.ToString("N0") + "m/s", this.infoStyle);
                }
            }
            GUILayout.EndVertical();
        }

        /// <summary>
        ///     Draws the specific impluse column.
        /// </summary>
        private void DrawIsp()
        {
            GUILayout.BeginVertical(GUILayout.Width(75.0f * GuiDisplaySize.Offset));
            GUILayout.Label("ISP", this.titleStyle);
            foreach (var stage in this.stages)
            {
                if (this.showAllStages || stage.deltaV > 0)
                {
                    GUILayout.Label(stage.isp.ToString("F1") + "s", this.infoStyle);
                }
            }
            GUILayout.EndVertical();
        }

        /// <summary>
        ///     Draws the mass column.
        /// </summary>
        private void DrawMass()
        {
            GUILayout.BeginVertical(GUILayout.Width(110.0f * GuiDisplaySize.Offset));
            GUILayout.Label("MASS", this.titleStyle);
            foreach (var stage in this.stages)
            {
                if (this.showAllStages || stage.deltaV > 0)
                {
                    GUILayout.Label(Units.ToMass(stage.mass, stage.totalMass), this.infoStyle);
                }
            }
            GUILayout.EndVertical();
        }

        /// <summary>
        ///     Draws the part count column.
        /// </summary>
        private void DrawPartCount()
        {
            GUILayout.BeginVertical(GUILayout.Width(50.0f * GuiDisplaySize.Offset));
            GUILayout.Label("PARTS", this.titleStyle);
            foreach (var stage in this.stages)
            {
                if (this.showAllStages || stage.deltaV > 0)
                {
                    GUILayout.Label(stage.partCount + " / " + stage.totalPartCount, this.infoStyle);
                }
            }
            GUILayout.EndVertical();
        }

        /// <summary>
        ///     Draws the settings panel.
        /// </summary>
        private void DrawSettings()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Compact mode collapses to the:", this.settingStyle);
            this.compactCollapseRight = !GUILayout.Toggle(!this.compactCollapseRight, "LEFT", this.buttonStyle, GUILayout.Width(100.0f * GuiDisplaySize.Offset));
            this.compactCollapseRight = GUILayout.Toggle(this.compactCollapseRight, "RIGHT", this.buttonStyle, GUILayout.Width(100.0f * GuiDisplaySize.Offset));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Build Engineer Overlay:", this.settingStyle);
            BuildOverlay.Visible = GUILayout.Toggle(BuildOverlay.Visible, "VISIBLE", this.buttonStyle, GUILayout.Width(100.0f * GuiDisplaySize.Offset));
            BuildOverlayPartInfo.NamesOnly = GUILayout.Toggle(BuildOverlayPartInfo.NamesOnly, "NAMES ONLY", this.buttonStyle, GUILayout.Width(100.0f * GuiDisplaySize.Offset));
            BuildOverlayPartInfo.ClickToOpen = GUILayout.Toggle(BuildOverlayPartInfo.ClickToOpen, "CLICK TO OPEN", this.buttonStyle, GUILayout.Width(100.0f * GuiDisplaySize.Offset));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Flight Engineer activation mode:", this.settingStyle);
            FlightEngineerPartless.IsPartless = GUILayout.Toggle(FlightEngineerPartless.IsPartless, "PARTLESS", this.buttonStyle, GUILayout.Width(100.0f * GuiDisplaySize.Offset));
            FlightEngineerPartless.IsPartless = !GUILayout.Toggle(!FlightEngineerPartless.IsPartless, "MODULE", this.buttonStyle, GUILayout.Width(100.0f * GuiDisplaySize.Offset));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("GUI Size: " + GuiDisplaySize.Increment, this.settingStyle);
            if (GUILayout.Button("<", this.buttonStyle, GUILayout.Width(100.0f * GuiDisplaySize.Offset)))
            {
                GuiDisplaySize.Increment--;
            }
            if (GUILayout.Button(">", this.buttonStyle, GUILayout.Width(100.0f * GuiDisplaySize.Offset)))
            {
                GuiDisplaySize.Increment++;
            }
            GUILayout.EndHorizontal();

            GUILayout.Label("Minimum delay between simulations: " + SimManager.minSimTime + "ms", this.settingStyle);
            GUI.skin = HighLogic.Skin;
            SimManager.minSimTime = (long)GUILayout.HorizontalSlider(SimManager.minSimTime, 0, 2000.0f);
            GUI.skin = null;
        }

        /// <summary>
        ///     Draws the stage number column.
        /// </summary>
        private void DrawStageNumbers()
        {
            GUILayout.BeginVertical(GUILayout.Width(30.0f * GuiDisplaySize.Offset));
            GUILayout.Label(string.Empty, this.titleStyle);
            foreach (var stage in this.stages)
            {
                if (this.showAllStages || stage.deltaV > 0)
                {
                    GUILayout.Label("S" + stage.number, this.titleStyle);
                }
            }
            GUILayout.EndVertical();
        }

        /// <summary>
        ///     Draws the thrust column.
        /// </summary>
        private void DrawThrust()
        {
            GUILayout.BeginVertical(GUILayout.Width(75.0f * GuiDisplaySize.Offset));
            GUILayout.Label("THRUST", this.titleStyle);
            foreach (var stage in this.stages)
            {
                if (this.showAllStages || stage.deltaV > 0)
                {
                    GUILayout.Label(stage.thrust.ToForce(), this.infoStyle);
                }
            }
            GUILayout.EndVertical();
        }

        /// <summary>
        ///     Draws the torque column.
        /// </summary>
        private void DrawTorque()
        {
            GUILayout.BeginVertical(GUILayout.Width(75.0f * GuiDisplaySize.Offset));
            GUILayout.Label("TORQUE", this.titleStyle);
            foreach (var stage in this.stages)
            {
                if (this.showAllStages || stage.deltaV > 0)
                {
                    GUILayout.Label(stage.maxThrustTorque.ToTorque(), this.infoStyle);
                }
            }
            GUILayout.EndVertical();
        }

        /// <summary>
        ///     Drwas the thrust to weight ratio column.
        /// </summary>
        private void DrawTwr()
        {
            GUILayout.BeginVertical(GUILayout.Width(100.0f * GuiDisplaySize.Offset));
            GUILayout.Label("TWR (MAX)", this.titleStyle);
            foreach (var stage in this.stages)
            {
                if (this.showAllStages || stage.deltaV > 0)
                {
                    GUILayout.Label(stage.thrustToWeight.ToString("F2") + " (" + stage.maxThrustToWeight.ToString("F2") + ")", this.infoStyle);
                }
            }
            GUILayout.EndVertical();
        }

        private void EditorLock(bool state)
        {
            if (state)
            {
                EditorLogic.fetch.Lock(true, true, true, "KER_BuildAdvanced");
                BuildOverlayPartInfo.Hidden = true;
                this.isEditorLocked = true;
            }
            else
            {
                EditorLogic.fetch.Unlock("KER_BuildAdvanced");
                BuildOverlayPartInfo.Hidden = false;
                this.isEditorLocked = false;
            }
        }

        /// <summary>
        ///     Initialises all the styles that are required.
        /// </summary>
        private void InitialiseStyles()
        {
            this.windowStyle = new GUIStyle(HighLogic.Skin.window)
            {
                alignment = TextAnchor.UpperLeft
            };

            this.areaStyle = new GUIStyle(HighLogic.Skin.box)
            {
                padding = new RectOffset(0, 0, 9, 0)
            };

            this.areaSettingStyle = new GUIStyle(HighLogic.Skin.box)
            {
                padding = new RectOffset(10, 10, 10, 10)
            };

            this.buttonStyle = new GUIStyle(HighLogic.Skin.button)
            {
                normal =
                {
                    textColor = Color.white
                },
                fontSize = (int)(11 * GuiDisplaySize.Offset),
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
            };

            this.titleStyle = new GUIStyle(HighLogic.Skin.label)
            {
                normal =
                {
                    textColor = Color.white
                },
                fontSize = (int)(11 * GuiDisplaySize.Offset),
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
                stretchWidth = true,
            };

            this.infoStyle = new GUIStyle(HighLogic.Skin.label)
            {
                fontSize = (int)(11 * GuiDisplaySize.Offset),
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
                stretchWidth = true
            };

            this.settingStyle = new GUIStyle(this.titleStyle)
            {
                alignment = TextAnchor.MiddleLeft,
                stretchWidth = true,
                stretchHeight = true
            };

            this.settingAtmoStyle = new GUIStyle(this.titleStyle)
            {
                margin = new RectOffset(),
                padding = new RectOffset(),
                alignment = TextAnchor.UpperLeft
            };

            this.bodiesButtonStyle = new GUIStyle(HighLogic.Skin.button)
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

            this.bodiesButtonActiveStyle = new GUIStyle(this.bodiesButtonStyle)
            {
                normal = this.bodiesButtonStyle.onNormal,
                hover = this.bodiesButtonStyle.onHover
            };
        }

        /// <summary>
        ///     Loads the settings when this object is created.
        /// </summary>
        private void Load()
        {
            try
            {
                var handler = SettingHandler.Load("BuildAdvanced.xml");
                handler.Get("visible", ref this.visible);
                this.position.x = handler.Get("windowPositionX", this.position.x);
                this.position.y = handler.Get("windowPositionY", this.position.y);
                handler.Get("compactMode", ref this.compactMode);
                handler.Get("compactCollapseRight", ref this.compactCollapseRight);
                handler.Get("showAllStages", ref this.showAllStages);
                handler.Get("showAtmosphericDetails", ref this.showAtmosphericDetails);
                handler.Get("showSettings", ref this.showSettings);
                CelestialBodies.SetSelectedBody(handler.Get("selectedBodyName", CelestialBodies.SelectedBody.Name));
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "BuildAdvanced->Load");
            }
        }

        private void OnSizeChanged()
        {
            this.InitialiseStyles();
            this.hasChanged = true;
        }

        /// <summary>
        ///     Draws the OnGUI window.
        /// </summary>
        private void Window(int windowId)
        {
            try
            {
                // Draw the compact mode toggle.
                if (GUI.Toggle(new Rect(this.position.width - 70.0f * GuiDisplaySize.Offset, 5.0f, 65.0f * GuiDisplaySize.Offset, 20.0f), this.compactMode, "COMPACT", this.buttonStyle) != this.compactMode)
                {
                    this.hasChanged = true;
                    this.compactCheck = 2;
                    this.compactRight = this.position.xMax;
                    this.compactMode = !this.compactMode;
                }

                // When not in compact mode draw the 'All Stages' and 'Atmospheric' toggles.
                if (!this.compactMode)
                {
                    if (GUI.Toggle(new Rect(this.position.width - 143.0f * GuiDisplaySize.Offset, 5.0f, 70.0f * GuiDisplaySize.Offset, 20.0f), this.showSettings, "SETTINGS", this.buttonStyle) != this.showSettings)
                    {
                        this.hasChanged = true;
                        this.showSettings = !this.showSettings;
                    }

                    if (GUI.Toggle(new Rect(this.position.width - 226.0f * GuiDisplaySize.Offset, 5.0f, 80.0f * GuiDisplaySize.Offset, 20.0f), this.showAllStages, "ALL STAGES", this.buttonStyle) != this.showAllStages)
                    {
                        this.hasChanged = true;
                        this.showAllStages = !this.showAllStages;
                    }

                    if (GUI.Toggle(new Rect(this.position.width - 324.0f * GuiDisplaySize.Offset, 5.0f, 95.0f * GuiDisplaySize.Offset, 20.0f), this.showAtmosphericDetails, "ATMOSPHERIC", this.buttonStyle) != this.showAtmosphericDetails)
                    {
                        this.hasChanged = true;
                        this.showAtmosphericDetails = !this.showAtmosphericDetails;
                    }

                    this.bodiesListPosition = new Rect(this.position.width - 452.0f * GuiDisplaySize.Offset, 5.0f, 125.0f * GuiDisplaySize.Offset, 20.0f);
                    this.bodiesList.enabled = GUI.Toggle(this.bodiesListPosition, this.bodiesList.enabled, "BODY: " + CelestialBodies.SelectedBody.Name.ToUpper(), this.buttonStyle);
                    this.bodiesList.SetPosition(this.bodiesListPosition.Translate(this.position));
                }

                // Draw the main informational display box.
                if (!this.compactMode)
                {
                    GUILayout.BeginHorizontal(this.areaStyle);
                    this.DrawStageNumbers();
                    this.DrawPartCount();
                    this.DrawCost();
                    this.DrawMass();
                    this.DrawIsp();
                    this.DrawThrust();
                    this.DrawTorque();
                    this.DrawTwr();
                    this.DrawDeltaV();
                    this.DrawBurnTime();
                    GUILayout.EndHorizontal();

                    if (this.showAtmosphericDetails)
                    {
                        GUILayout.BeginVertical(this.areaSettingStyle);
                        this.DrawAtmosphericDetails();
                        GUILayout.EndVertical();
                    }

                    if (this.showSettings)
                    {
                        GUILayout.BeginVertical(this.areaSettingStyle);
                        this.DrawSettings();
                        GUILayout.EndVertical();
                    }
                }
                else // Draw only a few details when in compact mode.
                {
                    GUILayout.BeginHorizontal(this.areaStyle);
                    this.DrawStageNumbers();
                    this.DrawTwr();
                    this.DrawDeltaV();
                    GUILayout.EndHorizontal();
                }

                GUI.DragWindow();
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }

        #endregion
    }
}