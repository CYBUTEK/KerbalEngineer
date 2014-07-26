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
using KerbalEngineer.Settings;
using KerbalEngineer.VesselSimulator;

using UnityEngine;

#endregion

namespace KerbalEngineer.Editor
{
    [KSPAddon(KSPAddon.Startup.EditorAny, false)]
    public class BuildAdvanced : MonoBehaviour
    {
        #region Instance

        /// <summary>
        ///     Gets the current instance if started or returns null.
        /// </summary>
        public static BuildAdvanced Instance { get; private set; }

        #endregion

        #region Fields

        private float atmosphericPercentage = 1.0f;
        private float atmosphericVelocity;
        private int compactCheck;
        private bool compactCollapseRight;
        private float compactRight;
        private bool hasChanged;
        private bool isEditorLocked;
        private int numberOfStages;
        private int windowId;
        private Rect windowPosition = new Rect(265.0f, 45.0f, 0, 0);

        #region Styles

        private GUIStyle areaBodiesStyle;
        private GUIStyle areaStyle;
        private GUIStyle buttonStyle;
        private GUIStyle infoStyle;
        private GUIStyle settingStyle;
        private GUIStyle titleStyle;
        private GUIStyle windowStyle;

        #endregion

        #endregion

        #region Properties

        private bool compactMode;
        private bool showAllStages;
        private bool showAtmosphericDetails;
        private bool showReferenceBodies;
        private bool showSettings;
        private bool visible = true;

        /// <summary>
        ///     Gets and sets whether the display is enabled.
        /// </summary>
        public bool Visible
        {
            get { return this.visible; }
            set
            {
                this.visible = value;
                Logger.Log("BuildAdvanced->Visible = " + value);
            }
        }

        /// <summary>
        ///     Gets and sets whether to show in compact mode.
        /// </summary>
        public bool CompactMode
        {
            get { return this.compactMode; }
            set
            {
                this.compactMode = value;
                Logger.Log("BuildAdvanced->CompactMode = " + value);
            }
        }

        /// <summary>
        ///     Gets and sets whether to show all stages.
        /// </summary>
        public bool ShowAllStages
        {
            get { return this.showAllStages; }
            set
            {
                this.showAllStages = value;
                Logger.Log("BuildAdvanced->ShowAllStages = " + value);
            }
        }

        /// <summary>
        ///     Gets and sets whether to use atmospheric details.
        /// </summary>
        public bool ShowAtmosphericDetails
        {
            get { return this.showAtmosphericDetails; }
            set
            {
                this.showAtmosphericDetails = value;
                Logger.Log("BuildAdvanced->ShowAtmosphericDetails = " + value);
            }
        }

        /// <summary>
        ///     Gets and sets whether to show the reference body selection.
        /// </summary>
        public bool ShowReferenceBodies
        {
            get { return this.showReferenceBodies; }
            set
            {
                this.showReferenceBodies = value;
                Logger.Log("BuildAdvanced->ShowReferenceBodies = " + value);
            }
        }

        public bool ShowSettings
        {
            get { return this.showSettings; }
            set
            {
                this.showSettings = value;
                Logger.Log("BuildAdvanced->ShowSettings = " + value);
            }
        }

        #endregion

        #region Initialisation

        private void Awake()
        {
            Instance = this;
            this.Load();
        }

        private void Start()
        {
            this.windowId = this.GetHashCode();
            this.InitialiseStyles();
            RenderingManager.AddToPostDrawQueue(0, this.OnDraw);
        }

        /// <summary>
        ///     Initialises all the styles that are required.
        /// </summary>
        private void InitialiseStyles()
        {
            try
            {
                this.areaBodiesStyle = new GUIStyle(HighLogic.Skin.box);

                this.windowStyle = new GUIStyle(HighLogic.Skin.window)
                {
                    alignment = TextAnchor.UpperLeft
                };

                this.areaStyle = new GUIStyle(HighLogic.Skin.box)
                {
                    padding = new RectOffset(0, 0, 9, 0)
                };

                this.buttonStyle = new GUIStyle(HighLogic.Skin.button)
                {
                    normal =
                    {
                        textColor = Color.white
                    },
                    fontSize = 11,
                    fontStyle = FontStyle.Bold,
                    alignment = TextAnchor.MiddleCenter
                };

                this.titleStyle = new GUIStyle(HighLogic.Skin.label)
                {
                    normal =
                    {
                        textColor = Color.white
                    },
                    fontSize = 11,
                    fontStyle = FontStyle.Bold,
                    alignment = TextAnchor.MiddleCenter,
                    stretchWidth = true,
                };

                this.infoStyle = new GUIStyle(HighLogic.Skin.label)
                {
                    fontSize = 11,
                    fontStyle = FontStyle.Bold,
                    alignment = TextAnchor.MiddleCenter,
                    stretchWidth = true
                };

                this.settingStyle = new GUIStyle(this.titleStyle)
                {
                    stretchHeight = true
                };
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "BuildAdvanced->InitialiseStyles");
            }
        }

        #endregion

        #region Update and Drawing

        private void Update()
        {
            try
            {
                if (!this.visible || EditorLogic.fetch == null || EditorLogic.fetch.ship.parts.Count == 0)
                {
                    return;
                }

                // Configure the simulation parameters based on the selected reference body.
                SimManager.Gravity = CelestialBodies.Instance.SelectedBodyInfo.Gravity;

                if (this.showAtmosphericDetails)
                {
                    SimManager.Atmosphere = CelestialBodies.Instance.SelectedBodyInfo.Atmosphere * 0.01d * this.atmosphericPercentage;
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

        private void OnDraw()
        {
            try
            {
                if (!this.visible || EditorLogic.fetch == null || EditorLogic.fetch.ship.parts.Count == 0)
                {
                    return;
                }

                SimManager.RequestSimulation();

                // Change the window title based on whether in compact mode or not.
                var title = !this.compactMode ? "KERBAL ENGINEER REDUX " + EngineerGlobals.AssemblyVersion : "K.E.R. " + EngineerGlobals.AssemblyVersion;

                if (SimManager.Stages != null)
                {
                    // Reset the window size when the staging or something else has changed.
                    var stageCount = SimManager.Stages.Count(stage => this.showAllStages || stage.deltaV > 0);
                    if (this.hasChanged || stageCount != this.numberOfStages)
                    {
                        this.hasChanged = false;
                        this.numberOfStages = stageCount;

                        this.windowPosition.width = 0;
                        this.windowPosition.height = 0;
                    }

                    this.windowPosition = GUILayout.Window(this.windowId, this.windowPosition, this.Window, title, this.windowStyle).ClampToScreen();

                    if (this.compactCheck > 0 && this.compactCollapseRight)
                    {
                        this.windowPosition.x = this.compactRight - this.windowPosition.width;
                        this.compactCheck--;
                    }
                    else if (this.compactCheck > 0)
                    {
                        this.compactCheck = 0;
                    }

                    // Check editor lock to manage click-through.
                    this.CheckEditorLock();
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "BuildAdvanced->OnDraw");
            }
        }

        /// <summary>
        ///     Checks whether the editor should be locked to stop click-through.
        /// </summary>
        private void CheckEditorLock()
        {
            try
            {
                if (this.windowPosition.MouseIsOver())
                {
                    EditorLogic.fetch.State = EditorLogic.EditorState.GUI_SELECTED;
                    this.isEditorLocked = true;
                }
                else if (!this.windowPosition.MouseIsOver() && this.isEditorLocked)
                {
                    EditorLogic.fetch.State = EditorLogic.EditorState.PAD_UNSELECTED;
                    this.isEditorLocked = false;
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "BuildAdvanced->CheckEditorLock");
            }
        }

        /// <summary>
        ///     Draws the OnGUI window.
        /// </summary>
        private void Window(int windowId)
        {
            try
            {
                // Draw the compact mode toggle.
                if (GUI.Toggle(new Rect(this.windowPosition.width - 70.0f, 5.0f, 65.0f, 20.0f), this.compactMode, "COMPACT", this.buttonStyle) != this.compactMode)
                {
                    this.hasChanged = true;
                    this.compactCheck = 2;
                    this.compactRight = this.windowPosition.xMax;
                    this.compactMode = !this.compactMode;
                }

                // When not in compact mode draw the 'All Stages' and 'Atmospheric' toggles.
                if (!this.compactMode)
                {
                    if (GUI.Toggle(new Rect(this.windowPosition.width - 143.0f, 5.0f, 70.0f, 20.0f), this.showSettings, "SETTINGS", this.buttonStyle) != this.showSettings)
                    {
                        this.hasChanged = true;
                        this.showSettings = !this.showSettings;
                    }

                    if (GUI.Toggle(new Rect(this.windowPosition.width - 226.0f, 5.0f, 80.0f, 20.0f), this.showAllStages, "ALL STAGES", this.buttonStyle) != this.showAllStages)
                    {
                        this.hasChanged = true;
                        this.showAllStages = !this.showAllStages;
                    }

                    if (GUI.Toggle(new Rect(this.windowPosition.width - 324.0f, 5.0f, 95.0f, 20.0f), this.showAtmosphericDetails, "ATMOSPHERIC", this.buttonStyle) != this.showAtmosphericDetails)
                    {
                        this.hasChanged = true;
                        this.showAtmosphericDetails = !this.showAtmosphericDetails;
                    }

                    if (GUI.Toggle(new Rect(this.windowPosition.width - 452.0f, 5.0f, 125.0f, 20.0f), this.showReferenceBodies, "REFERENCE BODIES", this.buttonStyle) != this.showReferenceBodies)
                    {
                        this.hasChanged = true;
                        this.showReferenceBodies = !this.showReferenceBodies;
                    }
                }

                // Draw the main informational display box.

                if (!this.compactMode)
                {
                    GUILayout.BeginHorizontal(this.areaStyle);
                    this.DrawStageNumbers();
                    //this.DrawPartCount();
                    this.DrawCost();
                    this.DrawMass();
                    this.DrawIsp();
                    this.DrawThrust();
                    this.DrawTwr();
                    this.DrawDeltaV();
                    this.DrawBurnTime();
                    GUILayout.EndHorizontal();

                    if (this.showAtmosphericDetails)
                    {
                        GUILayout.BeginVertical(this.areaStyle);
                        this.DrawAtmosphericDetails();
                        GUILayout.EndVertical();
                    }

                    if (this.showReferenceBodies)
                    {
                        GUILayout.BeginVertical(this.areaBodiesStyle);
                        this.DrawReferenceBodies();
                        GUILayout.EndVertical();
                    }

                    if (this.showSettings)
                    {
                        GUILayout.BeginVertical(this.areaStyle);
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
                Logger.Exception(ex, "BuildAdvanced->Window");
            }
        }

        /// <summary>
        ///     Draws the atmospheric settings.
        /// </summary>
        private void DrawAtmosphericDetails()
        {
            try
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("Pressure " + (this.atmosphericPercentage * 100.0f).ToString("F1") + "%", this.settingStyle, GUILayout.Width(125.0f));
                GUI.skin = HighLogic.Skin;
                this.atmosphericPercentage = GUILayout.HorizontalSlider(this.atmosphericPercentage, 0, 1.0f);
                GUI.skin = null;
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                GUILayout.Label("Velocity " + this.atmosphericVelocity.ToString("F1") + "m/s", this.settingStyle, GUILayout.Width(125.0f));
                GUI.skin = HighLogic.Skin;
                this.atmosphericVelocity = GUILayout.HorizontalSlider(this.atmosphericVelocity, 0, 2500f);
                GUI.skin = null;
                GUILayout.EndHorizontal();
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "BuildAdvanced->DrawAtmosphericDetails");
            }
        }

        /// <summary>
        ///     Draws all the reference bodies.
        /// </summary>
        private void DrawReferenceBodies()
        {
            try
            {
                var index = 0;

                foreach (var bodyName in CelestialBodies.Instance.BodyList.Keys)
                {
                    if (index % 8 == 0)
                    {
                        if (index > 0)
                        {
                            GUILayout.EndHorizontal();
                        }
                        GUILayout.BeginHorizontal();
                    }
                    if (GUILayout.Toggle(CelestialBodies.Instance.SelectedBodyName == bodyName, bodyName, this.buttonStyle))
                    {
                        CelestialBodies.Instance.SelectedBodyName = bodyName;
                    }
                    index++;
                }
                GUILayout.EndHorizontal();
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "BuildAdvanced->DrawReferenceBodies");
            }
        }

        private void DrawSettings()
        {
            try
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("Compact mode collapses to the:", this.settingStyle);
                this.compactCollapseRight = !GUILayout.Toggle(!this.compactCollapseRight, "LEFT", this.buttonStyle, GUILayout.Width(100.0f));
                this.compactCollapseRight = GUILayout.Toggle(this.compactCollapseRight, "RIGHT", this.buttonStyle, GUILayout.Width(100.0f));
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Flight Engineer activation mode:", this.settingStyle);
                FlightEngineerPartless.IsPartless = GUILayout.Toggle(FlightEngineerPartless.IsPartless, "PARTLESS", this.buttonStyle, GUILayout.Width(100.0f));
                FlightEngineerPartless.IsPartless = !GUILayout.Toggle(!FlightEngineerPartless.IsPartless, "MODULE", this.buttonStyle, GUILayout.Width(100.0f));
                GUILayout.EndHorizontal();
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "BuildAdvanced->DrawSettings");
            }
        }

        /// <summary>
        ///     Draws the stage number column.
        /// </summary>
        private void DrawStageNumbers()
        {
            try
            {
                GUILayout.BeginVertical(GUILayout.Width(30.0f));
                GUILayout.Label(string.Empty, this.titleStyle);
                foreach (var stage in SimManager.Stages)
                {
                    if (this.showAllStages || stage.deltaV > 0)
                    {
                        GUILayout.Label("S" + stage.number, this.titleStyle);
                    }
                }
                GUILayout.EndVertical();
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "BuildAdvanced->DrawStageNumbers");
            }
        }

        /// <summary>
        ///     Draws the part count column.
        /// </summary>
        private void DrawPartCount()
        {
            try
            {
                GUILayout.BeginVertical(GUILayout.Width(50.0f));
                GUILayout.Label("PARTS", this.titleStyle);
                foreach (var stage in SimManager.Stages)
                {
                    if (this.showAllStages || stage.deltaV > 0)
                    {
                        //GUILayout.Label(stage.PartCount.ToString("N0"), this.infoStyle);
                    }
                }
                GUILayout.EndVertical();
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "BuildAdvanced->DrawPartCount");
            }
        }

        /// <summary>
        ///     Draws the cost column.
        /// </summary>
        private void DrawCost()
        {
            try
            {
                GUILayout.BeginVertical(GUILayout.Width(110.0f));
                GUILayout.Label("COST", this.titleStyle);
                foreach (var stage in SimManager.Stages)
                {
                    if (this.showAllStages || stage.deltaV > 0)
                    {
                        GUILayout.Label(stage.cost.ToString("N0") + " / " + stage.totalCost.ToString("N0"), this.infoStyle);
                    }
                }
                GUILayout.EndVertical();
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "BuildAdvanced->DrawCost");
            }
        }

        /// <summary>
        ///     Draws the mass column.
        /// </summary>
        private void DrawMass()
        {
            try
            {
                GUILayout.BeginVertical(GUILayout.Width(110.0f));
                GUILayout.Label("MASS", this.titleStyle);
                foreach (var stage in SimManager.Stages)
                {
                    if (this.showAllStages || stage.deltaV > 0)
                    {
                        GUILayout.Label(stage.mass.ToMass(false) + " / " + stage.totalMass.ToMass(), this.infoStyle);
                    }
                }
                GUILayout.EndVertical();
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "BuildAdvanced->DrawMass");
            }
        }

        /// <summary>
        ///     Draws the specific impluse column.
        /// </summary>
        private void DrawIsp()
        {
            try
            {
                GUILayout.BeginVertical(GUILayout.Width(75.0f));
                GUILayout.Label("ISP", this.titleStyle);
                foreach (var stage in SimManager.Stages)
                {
                    if (this.showAllStages || stage.deltaV > 0)
                    {
                        GUILayout.Label(stage.isp.ToString("F1") + "s", this.infoStyle);
                    }
                }
                GUILayout.EndVertical();
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "BuildAdvanced->DrawIsp");
            }
        }

        /// <summary>
        ///     Draws the thrust column.
        /// </summary>
        private void DrawThrust()
        {
            try
            {
                GUILayout.BeginVertical(GUILayout.Width(75.0f));
                GUILayout.Label("THRUST", this.titleStyle);
                foreach (var stage in SimManager.Stages)
                {
                    if (this.showAllStages || stage.deltaV > 0)
                    {
                        GUILayout.Label(stage.thrust.ToForce(), this.infoStyle);
                    }
                }
                GUILayout.EndVertical();
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "BuildAdvanced->DrawThrust");
            }
        }

        /// <summary>
        ///     Drwas the thrust to weight ratio column.
        /// </summary>
        private void DrawTwr()
        {
            try
            {
                GUILayout.BeginVertical(GUILayout.Width(100.0f));
                GUILayout.Label("TWR (MAX)", this.titleStyle);
                foreach (var stage in SimManager.Stages)
                {
                    if (this.showAllStages || stage.deltaV > 0)
                    {
                        GUILayout.Label(stage.thrustToWeight.ToString("F2") + " (" + stage.maxThrustToWeight.ToString("F2") + ")", this.infoStyle);
                    }
                }
                GUILayout.EndVertical();
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "BuildAdvanced->DrawTwr");
            }
        }

        /// <summary>
        ///     Draws the deltaV column.
        /// </summary>
        private void DrawDeltaV()
        {
            try
            {
                GUILayout.BeginVertical(GUILayout.Width(100.0f));
                GUILayout.Label("DELTA-V", this.titleStyle);
                foreach (var stage in SimManager.Stages)
                {
                    if (this.showAllStages || stage.deltaV > 0)
                    {
                        GUILayout.Label(stage.deltaV.ToString("N0") + " / " + stage.inverseTotalDeltaV.ToString("N0") + "m/s", this.infoStyle);
                    }
                }
                GUILayout.EndVertical();
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "BuildAdvanced->DrawDeltaV");
            }
        }

        /// <summary>
        ///     Draws the burn time column.
        /// </summary>
        private void DrawBurnTime()
        {
            try
            {
                GUILayout.BeginVertical(GUILayout.Width(75.0f));
                GUILayout.Label("BURN", this.titleStyle);
                foreach (var stage in SimManager.Stages)
                {
                    if (this.showAllStages || stage.deltaV > 0)
                    {
                        GUILayout.Label(stage.time.ToTime(), this.infoStyle);
                    }
                }
                GUILayout.EndVertical();
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "BuildAdvanced->DrawBurnTime");
            }
        }

        #endregion

        #region Save and Load

        /// <summary>
        ///     Saves the settings when this object is destroyed.
        /// </summary>
        private void OnDestroy()
        {
            try
            {
                var handler = new SettingHandler();
                handler.Set("visible", this.visible);
                handler.Set("windowPositionX", this.windowPosition.x);
                handler.Set("windowPositionY", this.windowPosition.y);
                handler.Set("compactMode", this.compactMode);
                handler.Set("showAllStages", this.showAllStages);
                handler.Set("showAtmosphericDetails", this.showAtmosphericDetails);
                handler.Set("showReferenceBodies", this.showReferenceBodies);
                handler.Set("showSettings", this.showSettings);
                handler.Set("selectedBodyName", CelestialBodies.Instance.SelectedBodyName);
                handler.Save("BuildAdvanced.xml");
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "BuildAdvanced->OnDestroy");
            }
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
                this.windowPosition.x = handler.Get("windowPositionX", this.windowPosition.x);
                this.windowPosition.y = handler.Get("windowPositionY", this.windowPosition.y);
                handler.Get("compactMode", ref this.compactMode);
                handler.Get("showAllStages", ref this.showAllStages);
                handler.Get("showAtmosphericDetails", ref this.showAtmosphericDetails);
                handler.Get("showSettings", ref this.showSettings);
                CelestialBodies.Instance.SelectedBodyName = handler.Get("selectedBodyName", CelestialBodies.Instance.SelectedBodyName);
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "BuildAdvanced->Load");
            }
        }

        #endregion
    }
}