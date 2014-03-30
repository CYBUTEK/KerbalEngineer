// Project:	KerbalEngineer
// Author:	CYBUTEK
// License:	Attribution-NonCommercial-ShareAlike 3.0 Unported

#region Using Directives

using KerbalEngineer.Extensions;
using KerbalEngineer.Settings;
using KerbalEngineer.Simulation;

using UnityEngine;

#endregion

namespace KerbalEngineer.BuildEngineer
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

        private readonly int windowId = EngineerGlobals.GetNextWindowId();

        private bool hasChanged;
        private bool isEditorLocked;
        private Rect windowPosition = new Rect(265.0f, 45.0f, 0, 0);

        #region Styles

        private GUIStyle areaBodiesStyle;
        private GUIStyle areaStyle;
        private GUIStyle buttonStyle;
        private GUIStyle infoStyle;
        private GUIStyle titleStyle;
        private GUIStyle windowStyle;

        #endregion

        #endregion

        #region Properties

        private bool compactMode;
        private bool showAllStages;
        private bool showReferenceBodies;
        private bool useAtmosphericDetails;
        private bool visible;

        /// <summary>
        ///     Gets and sets whether the display is enabled.
        /// </summary>
        public bool Visible
        {
            get { return this.visible; }
            set { this.visible = value; }
        }

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
        public bool UseAtmosphericDetails
        {
            get { return this.useAtmosphericDetails; }
            set { this.useAtmosphericDetails = value; }
        }

        /// <summary>
        ///     Gets and sets whether to show the reference body selection.
        /// </summary>
        public bool ShowReferenceBodies
        {
            get { return this.showReferenceBodies; }
            set { this.showReferenceBodies = value; }
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
            this.InitialiseStyles();
            RenderingManager.AddToPostDrawQueue(0, this.OnDraw);
        }

        /// <summary>
        ///     Initialises all the styles that are required.
        /// </summary>
        private void InitialiseStyles()
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
                stretchWidth = true
            };

            this.infoStyle = new GUIStyle(HighLogic.Skin.label)
            {
                fontSize = 11,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
                stretchWidth = true
            };
        }

        #endregion

        #region Update and Drawing

        private void Update()
        {
            try
            {
                if (!this.visible || EditorLogic.fetch == null || EditorLogic.fetch.ship.Count <= 0)
                {
                    return;
                }

                // Configure the simulation parameters based on the selected reference body.
                SimulationManager.Gravity = CelestialBodies.Instance.SelectedBodyInfo.Gravity;
                if (this.useAtmosphericDetails)
                {
                    SimulationManager.Atmosphere = CelestialBodies.Instance.SelectedBodyInfo.Atmosphere *
                                                            0.01d;
                }
                else
                {
                    SimulationManager.Atmosphere = 0;
                }

                SimulationManager.TryStartSimulation();

                // Reset the window size when the staging or something else has changed.
                if (this.hasChanged || SimulationManager.StagingChanged)
                {
                    this.hasChanged = false;

                    this.windowPosition.width = 0;
                    this.windowPosition.height = 0;
                }
            }
            catch
            {
                /* A null reference exception is thrown when checking if EditorLogic.fetch != null??? */
            }
        }

        private void OnDraw()
        {
            try
            {
                if (!this.visible || EditorLogic.fetch == null || EditorLogic.fetch.ship.Count <= 0)
                {
                    return;
                }

                SimulationManager.RequestSimulation();

                // Change the window title based on whether in compact mode or not.
                string title;
                if (!this.compactMode)
                {
                    title = "KERBAL ENGINEER REDUX " + EngineerGlobals.AssemblyVersion;
                }
                else
                {
                    title = "K.E.R. " + EngineerGlobals.AssemblyVersion;
                }

                this.windowPosition = GUILayout.Window(this.windowId, this.windowPosition, this.Window, title, this.windowStyle).ClampToScreen();

                // Check editor lock to manage click-through.
                this.CheckEditorLock();
            }
            catch
            {
                /* A null reference exception is thrown when checking if EditorLogic.fetch != null??? */
            }
        }

        /// <summary>
        ///     Checks whether the editor should be locked to stop click-through.
        /// </summary>
        private void CheckEditorLock()
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

        /// <summary>
        ///     Draws the OnGUI window.
        /// </summary>
        private void Window(int windowId)
        {
            // Draw the compact mode toggle.
            if (GUI.Toggle(new Rect(this.windowPosition.width - 70.0f, 5.0f, 65.0f, 20.0f), this.compactMode, "COMPACT", this.buttonStyle) != this.compactMode)
            {
                this.hasChanged = true;
                this.compactMode = !this.compactMode;
            }

            // When not in compact mode draw the 'All Stages' and 'Atmospheric' toggles.
            if (!this.compactMode)
            {
                if (GUI.Toggle(new Rect(this.windowPosition.width - 153.0f, 5.0f, 80.0f, 20.0f), this.showAllStages, "ALL STAGES", this.buttonStyle) != this.showAllStages)
                {
                    this.hasChanged = true;
                    this.showAllStages = !this.showAllStages;
                }

                this.useAtmosphericDetails = GUI.Toggle(new Rect(this.windowPosition.width - 251.0f, 5.0f, 95.0f, 20.0f), this.useAtmosphericDetails, "ATMOSPHERIC", this.buttonStyle);

                if (GUI.Toggle(new Rect(this.windowPosition.width - 379.0f, 5.0f, 125.0f, 20.0f), this.showReferenceBodies, "REFERENCE BODIES", this.buttonStyle) != this.showReferenceBodies)
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
                this.DrawPartCount();
                this.DrawCost();
                this.DrawMass();
                this.DrawIsp();
                this.DrawThrust();
                this.DrawTwr();
                this.DrawDeltaV();
                this.DrawBurnTime();
                GUILayout.EndHorizontal();

                if (this.showReferenceBodies)
                {
                    GUILayout.BeginVertical(this.areaBodiesStyle);
                    this.DrawReferenceBodies();
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

        /// <summary>
        ///     Draws all the reference bodies.
        /// </summary>
        private void DrawReferenceBodies()
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

        /// <summary>
        ///     Draws the stage number column.
        /// </summary>
        private void DrawStageNumbers()
        {
            GUILayout.BeginVertical(GUILayout.Width(30.0f));
            GUILayout.Label("", this.titleStyle);
            foreach (var stage in SimulationManager.Stages)
            {
                if (this.showAllStages || stage.DeltaV > 0)
                {
                    GUILayout.Label(stage.Number.ToString(), this.titleStyle);
                }
            }
            GUILayout.EndVertical();
        }

        /// <summary>
        ///     Draws the part count column.
        /// </summary>
        private void DrawPartCount()
        {
            GUILayout.BeginVertical(GUILayout.Width(50.0f));
            GUILayout.Label("PARTS", this.titleStyle);
            foreach (var stage in SimulationManager.Stages)
            {
                if (this.showAllStages || stage.DeltaV > 0)
                {
                    //GUILayout.Label(stage.Parts, this.infoStyle);
                }
            }
            GUILayout.EndVertical();
        }

        /// <summary>
        ///     Draws the cost column.
        /// </summary>
        private void DrawCost()
        {
            GUILayout.BeginVertical(GUILayout.Width(100.0f));
            GUILayout.Label("COST", this.titleStyle);
            foreach (var stage in SimulationManager.Stages)
            {
                if (this.showAllStages || stage.DeltaV > 0)
                {
                    GUILayout.Label(stage.Cost.ToString(), this.infoStyle);
                }
            }
            GUILayout.EndVertical();
        }

        /// <summary>
        ///     Draws the mass column.
        /// </summary>
        private void DrawMass()
        {
            GUILayout.BeginVertical(GUILayout.Width(100.0f));
            GUILayout.Label("MASS", this.titleStyle);
            foreach (var stage in SimulationManager.Stages)
            {
                if (this.showAllStages || stage.DeltaV > 0)
                {
                    GUILayout.Label(stage.Mass.ToMass(), this.infoStyle);
                }
            }
            GUILayout.EndVertical();
        }

        /// <summary>
        ///     Draws the specific impluse column.
        /// </summary>
        private void DrawIsp()
        {
            GUILayout.BeginVertical(GUILayout.Width(50.0f));
            GUILayout.Label("ISP", this.titleStyle);
            foreach (var stage in SimulationManager.Stages)
            {
                if (this.showAllStages || stage.DeltaV > 0)
                {
                    GUILayout.Label(stage.Isp.ToString("0."), this.infoStyle);
                }
            }
            GUILayout.EndVertical();
        }

        /// <summary>
        ///     Draws the thrust column.
        /// </summary>
        private void DrawThrust()
        {
            GUILayout.BeginVertical(GUILayout.Width(75.0f));
            GUILayout.Label("THRUST", this.titleStyle);
            foreach (var stage in SimulationManager.Stages)
            {
                if (this.showAllStages || stage.DeltaV > 0)
                {
                    GUILayout.Label(stage.Thrust.ToForce(), this.infoStyle);
                }
            }
            GUILayout.EndVertical();
        }

        /// <summary>
        ///     Drwas the thrust to weight ratio column.
        /// </summary>
        private void DrawTwr()
        {
            GUILayout.BeginVertical(GUILayout.Width(50.0f));
            GUILayout.Label("TWR", this.titleStyle);
            foreach (var stage in SimulationManager.Stages)
            {
                if (this.showAllStages || stage.DeltaV > 0)
                {
                    GUILayout.Label(stage.ThrustToWeight.ToString("0.00"), this.infoStyle);
                }
            }
            GUILayout.EndVertical();
        }

        /// <summary>
        ///     Draws the deltaV column.
        /// </summary>
        private void DrawDeltaV()
        {
            GUILayout.BeginVertical(GUILayout.Width(100.0f));
            GUILayout.Label("DELTA-V", this.titleStyle);
            foreach (var stage in SimulationManager.Stages)
            {
                if (this.showAllStages || stage.DeltaV > 0)
                {
                    GUILayout.Label(stage.DeltaV.ToSpeed(), this.infoStyle);
                }
            }
            GUILayout.EndVertical();
        }

        /// <summary>
        ///     Draws the burn time column.
        /// </summary>
        private void DrawBurnTime()
        {
            GUILayout.BeginVertical(GUILayout.Width(75.0f));
            GUILayout.Label("BURN", this.titleStyle);
            foreach (var stage in SimulationManager.Stages)
            {
                if (this.showAllStages || stage.DeltaV > 0)
                {
                    GUILayout.Label(stage.Time.ToTime(), this.infoStyle);
                }
            }
            GUILayout.EndVertical();
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
                var list = new SettingList();
                list.AddSetting("visible", this.visible);
                list.AddSetting("x", this.windowPosition.x);
                list.AddSetting("y", this.windowPosition.y);
                list.AddSetting("compact", this.compactMode);
                list.AddSetting("all_stages", this.showAllStages);
                list.AddSetting("atmosphere", this.useAtmosphericDetails);
                list.AddSetting("bodies", this.showReferenceBodies);
                list.AddSetting("selected_body", CelestialBodies.Instance.SelectedBodyName);
                SettingList.SaveToFile(EngineerGlobals.AssemblyPath + "Settings/BuildAdvanced", list);

                print("[KerbalEngineer/BuildAdvanced]: Successfully saved settings.");
            }
            catch
            {
                print("[KerbalEngineer/BuildAdvanced]: Failed to save settings.");
            }
        }

        /// <summary>
        ///     Loads the settings when this object is created.
        /// </summary>
        private void Load()
        {
            try
            {
                var list = SettingList.CreateFromFile(EngineerGlobals.AssemblyPath + "Settings/BuildAdvanced");
                this.visible = (bool)list.GetSetting("visible", this.visible);
                this.windowPosition.x = (float)list.GetSetting("x", this.windowPosition.x);
                this.windowPosition.y = (float)list.GetSetting("y", this.windowPosition.y);
                this.compactMode = (bool)list.GetSetting("compact", this.compactMode);
                this.showAllStages = (bool)list.GetSetting("all_stages", this.showAllStages);
                this.useAtmosphericDetails = (bool)list.GetSetting("atmosphere", this.useAtmosphericDetails);
                this.showReferenceBodies = (bool)list.GetSetting("bodies", this.showReferenceBodies);
                CelestialBodies.Instance.SelectedBodyName = (string)list.GetSetting("selected_body", "Kerbin");

                print("[KerbalEngineer/BuildAdvanced]: Successfully loaded settings.");
            }
            catch
            {
                print("[KerbalEngineer/BuildAdvanced]: Failed to load settings.");
            }
        }

        #endregion
    }
}