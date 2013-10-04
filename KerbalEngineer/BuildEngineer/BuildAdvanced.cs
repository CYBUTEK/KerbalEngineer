// Name:    Kerbal Engineer Redux
// Author:  CYBUTEK
// License: Attribution-NonCommercial-ShareAlike 3.0 Unported

using System;
using KerbalEngineer.Extensions;
using KerbalEngineer.Settings;
using KerbalEngineer.Simulation;
using UnityEngine;

namespace KerbalEngineer.BuildEngineer
{
    [KSPAddon(KSPAddon.Startup.EditorAny, false)]
    public class BuildAdvanced : MonoBehaviour
    {
        #region Instance

        /// <summary>
        /// Gets the current instance if started or returns null.
        /// </summary>
        public static BuildAdvanced Instance { get; private set; }

        #endregion

        #region Fields

        private Rect _windowPosition = new Rect(265f, 45f, 0f, 0f);
        private GUIStyle _windowStyle, _areaStyle, _areaBodiesStyle, _buttonStyle, _titleStyle, _infoStyle;
        private int _windowID = EngineerGlobals.GetNextWindowID();
        private bool _hasInitStyles = false;
        private bool _hasChanged = false;
        private bool _isEditorLocked = false;

        #endregion

        #region Properties

        private bool _visible = false;
        /// <summary>
        /// Gets and sets whether the display is enabled.
        /// </summary>
        public bool Visible
        {
            get { return _visible; }
            set { _visible = value; }
        }

        private bool _compactMode = false;
        /// <summary>
        /// Gets and sets whether to show in compact mode.
        /// </summary>
        public bool CompactMode
        {
            get { return _compactMode; }
            set { _compactMode = value; }
        }

        private bool _showAllStages = false;
        /// <summary>
        /// Gets and sets whether to show all stages.
        /// </summary>
        public bool ShowAllStages
        {
            get { return _showAllStages; }
            set { _showAllStages = value; }
        }

        private bool _useAtmosphericDetails = false;
        /// <summary>
        /// Gets and sets whether to use atmospheric details.
        /// </summary>
        public bool UseAtmosphericDetails
        {
            get { return _useAtmosphericDetails; }
            set { _useAtmosphericDetails = value; }
        }

        private bool _showReferenceBodies = false;
        /// <summary>
        /// Gets and sets whether to show the reference body selection.
        /// </summary>
        public bool ShowReferenceBodies
        {
            get { return _showReferenceBodies; }
            set { _showReferenceBodies = value; }
        }

        #endregion

        #region Initialisation

        public void Start()
        {
            // Set the instance to this object.
            Instance = this;

            RenderingManager.AddToPostDrawQueue(0, OnDraw);
        }

        // Initialises all of the GUI styles that are required.
        private void InitialiseStyles()
        {
            _hasInitStyles = true;

            _windowStyle = new GUIStyle(HighLogic.Skin.window);
            _windowStyle.alignment = TextAnchor.UpperLeft;

            _areaStyle = new GUIStyle(HighLogic.Skin.box);
            _areaStyle.padding = new RectOffset(0, 0, 9, 0);

            _areaBodiesStyle = new GUIStyle(HighLogic.Skin.box);

            _buttonStyle = new GUIStyle(HighLogic.Skin.button);
            _buttonStyle.normal.textColor = Color.white;
            _buttonStyle.fontSize = 11;
            _buttonStyle.fontStyle = FontStyle.Bold;
            _buttonStyle.alignment = TextAnchor.MiddleCenter;

            _titleStyle = new GUIStyle(HighLogic.Skin.label);
            _titleStyle.normal.textColor = Color.white;
            _titleStyle.fontSize = 11;
            _titleStyle.fontStyle = FontStyle.Bold;
            _titleStyle.alignment = TextAnchor.MiddleCenter;
            _titleStyle.stretchWidth = true;

            _infoStyle = new GUIStyle(HighLogic.Skin.label);
            _infoStyle.fontSize = 11;
            _infoStyle.fontStyle = FontStyle.Bold;
            _infoStyle.alignment = TextAnchor.MiddleCenter;
            _infoStyle.stretchWidth = true;
        }

        #endregion

        #region Update and Drawing

        public void Update()
        {
            try
            {
                if (EditorLogic.fetch != null && EditorLogic.SortedShipList.Count > 0)
                {
                    // Configure the simulation parameters based on the selected reference body.
                    SimulationManager.Instance.Gravity = CelestialBodies.Instance.SelectedBodyInfo.Gravity;
                    if (_useAtmosphericDetails)
                        SimulationManager.Instance.Atmosphere = CelestialBodies.Instance.SelectedBodyInfo.Atmosphere;
                    else
                        SimulationManager.Instance.Atmosphere = 0d;

                    SimulationManager.Instance.TryStartSimulation();

                    // Reset the window size when the staging or something else has changed.
                    if (_hasChanged || SimulationManager.Instance.StagingChanged)
                    {
                        _hasChanged = false;

                        _windowPosition.width = 0f;
                        _windowPosition.height = 0f;
                    }
                }
            }
            catch { /* A null reference exception is thrown when checking if EditorLogic.fetch != null??? */ }
        }

        private void OnDraw()
        {
            try
            {
                if (_visible && EditorLogic.fetch != null && EditorLogic.SortedShipList.Count > 0 && EditorLogic.fetch.editorScreen == EditorLogic.EditorScreen.Parts)
                {
                    SimulationManager.Instance.RequestSimulation();

                    // Initialise the GUI styles, but only once as needed.
                    if (!_hasInitStyles) InitialiseStyles();

                    // Change the window title based on whether in compact mode or not.
                    string title;
                    if (!_compactMode)
                        title = "KERBAL ENGINEER REDUX " + EngineerGlobals.AssemblyVersion;
                    else
                        title = "K.E.R. " + EngineerGlobals.AssemblyVersion;

                    _windowPosition = GUILayout.Window(_windowID, _windowPosition, Window, title, _windowStyle).ClampToScreen();

                    // Check editor lock to manage click-through.
                    CheckEditorLock();
                }
            }
            catch { /* A null reference exception is thrown when checking if EditorLogic.fetch != null??? */ }
        }

        // Checks whether the editor should be looked to stop click-through.
        private void CheckEditorLock()
        {
            if (_windowPosition.MouseIsOver() && !EditorLogic.editorLocked)
            {
                EditorLogic.fetch.Lock(true, true, true);
                _isEditorLocked = true;
            }
            else if (_isEditorLocked && !_windowPosition.MouseIsOver() && EditorLogic.editorLocked)
            {
                EditorLogic.fetch.Unlock();
            }

            if (!EditorLogic.editorLocked) _isEditorLocked = false;
        }

        private void Window(int windowID)
        {
            // Draw the compact mode toggle.
            if (GUI.Toggle(new Rect(_windowPosition.width - 70f, 5f, 65f, 20f), _compactMode, "COMPACT", _buttonStyle) != _compactMode)
            {
                _hasChanged = true;
                _compactMode = !_compactMode;
            }

            // When not in compact mode draw the 'All Stages' and 'Atmospheric' toggles.
            if (!_compactMode)
            {
                if (GUI.Toggle(new Rect(_windowPosition.width - 153f, 5f, 80f, 20f), _showAllStages, "ALL STAGES", _buttonStyle) != _showAllStages)
                {
                    _hasChanged = true;
                    _showAllStages = !_showAllStages;
                }

                _useAtmosphericDetails = GUI.Toggle(new Rect(_windowPosition.width - 251f, 5f, 95f, 20f), _useAtmosphericDetails, "ATMOSPHERIC", _buttonStyle);
                if (GUI.Toggle(new Rect(_windowPosition.width - 379f, 5f, 125f, 20f), _showReferenceBodies, "REFERENCE BODIES", _buttonStyle) != _showReferenceBodies)
                {
                    _hasChanged = true;
                    _showReferenceBodies = !_showReferenceBodies;
                }
            }

            // Draw the main informational display box.
            
            if (!_compactMode)
            {
                GUILayout.BeginHorizontal(_areaStyle);
                DrawStageNumbers();
                DrawPartCount();
                DrawCost();
                DrawMass();
                DrawIsp();
                DrawThrust();
                DrawTWR();
                DrawDeltaV();
                DrawBurnTime();
                GUILayout.EndHorizontal();

                if (_showReferenceBodies)
                {
                    GUILayout.BeginVertical(_areaBodiesStyle);
                    DrawReferenceBodies();
                    GUILayout.EndVertical();
                }
            }
            else // Draw only a few details when in compact mode.
            {
                GUILayout.BeginHorizontal(_areaStyle);
                DrawStageNumbers();
                DrawTWR();
                DrawDeltaV();
                GUILayout.EndHorizontal();
            }
            
            GUI.DragWindow();
        }

        // Draws all the reference bodies.
        private void DrawReferenceBodies()
        {
            int index = 0;
            foreach (string bodyName in CelestialBodies.Instance.BodyList.Keys)
            {
                if (index % 8 == 0)
                {
                    if (index > 0) GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                }
                if (GUILayout.Toggle(CelestialBodies.Instance.SelectedBodyName == bodyName, bodyName, _buttonStyle))
                {
                    CelestialBodies.Instance.SelectedBodyName = bodyName;
                }
                index++;
            }
            GUILayout.EndHorizontal();
        }

        // Draws the stage number column.
        private void DrawStageNumbers()
        {
            GUILayout.BeginVertical(GUILayout.Width(30f));
            GUILayout.Label("", _titleStyle);
            foreach (Stage stage in SimulationManager.Instance.Stages)
            {
                if (_showAllStages || stage.deltaV > 0d)
                    GUILayout.Label(stage.Number, _titleStyle);
            }
            GUILayout.EndVertical();
        }

        // Draws the part count column.
        private void DrawPartCount()
        {
            GUILayout.BeginVertical(GUILayout.Width(50f));
            GUILayout.Label("PARTS", _titleStyle);
            foreach (Stage stage in SimulationManager.Instance.Stages)
            {
                if (_showAllStages || stage.deltaV > 0d)
                    GUILayout.Label(stage.Parts, _infoStyle);
            }
            GUILayout.EndVertical();
        }

        // Draws the cost column.
        private void DrawCost()
        {
            GUILayout.BeginVertical(GUILayout.Width(100f));
            GUILayout.Label("COST", _titleStyle);
            foreach (Stage stage in SimulationManager.Instance.Stages)
            {
                if (_showAllStages || stage.deltaV > 0d)
                    GUILayout.Label(stage.Cost, _infoStyle);
            }
            GUILayout.EndVertical();
        }

        // Draws the mass column.
        private void DrawMass()
        {
            GUILayout.BeginVertical(GUILayout.Width(100f));
            GUILayout.Label("MASS", _titleStyle);
            foreach (Stage stage in SimulationManager.Instance.Stages)
            {
                if (_showAllStages || stage.deltaV > 0d)
                    GUILayout.Label(stage.Mass, _infoStyle);
            }
            GUILayout.EndVertical();
        }

        // Draws the specific impulse column.
        private void DrawIsp()
        {
            GUILayout.BeginVertical(GUILayout.Width(50f));
            GUILayout.Label("ISP", _titleStyle);
            foreach (Stage stage in SimulationManager.Instance.Stages)
            {
                if (_showAllStages || stage.deltaV > 0d)
                    GUILayout.Label(stage.Isp, _infoStyle);
            }
            GUILayout.EndVertical();
        }

        // Draws the thrust column.
        private void DrawThrust()
        {
            GUILayout.BeginVertical(GUILayout.Width(50f));
            GUILayout.Label("THRUST", _titleStyle);
            foreach (Stage stage in SimulationManager.Instance.Stages)
            {
                if (_showAllStages || stage.deltaV > 0d)
                    GUILayout.Label(stage.Thrust, _infoStyle);
            }
            GUILayout.EndVertical();
        }

        // Draws the thrust to weight ratio column.
        private void DrawTWR()
        {
            GUILayout.BeginVertical(GUILayout.Width(50f));
            GUILayout.Label("TWR", _titleStyle);
            foreach (Stage stage in SimulationManager.Instance.Stages)
            {
                if (_showAllStages || stage.deltaV > 0d)
                    GUILayout.Label(stage.TWR, _infoStyle);
            }
            GUILayout.EndVertical();
        }

        // Draws the deltaV column.
        private void DrawDeltaV()
        {
            GUILayout.BeginVertical(GUILayout.Width(100f));
            GUILayout.Label("DELTA-V", _titleStyle);
            foreach (Stage stage in SimulationManager.Instance.Stages)
            {
                if (_showAllStages || stage.deltaV > 0d)
                    GUILayout.Label(stage.DeltaV, _infoStyle);
            }
            GUILayout.EndVertical();
        }

        // Draws the burn time column.
        private void DrawBurnTime()
        {
            GUILayout.BeginVertical(GUILayout.Width(75f));
            GUILayout.Label("BURN", _titleStyle);
            foreach (Stage stage in SimulationManager.Instance.Stages)
            {
                if (_showAllStages || stage.deltaV > 0d)
                    GUILayout.Label(stage.Time, _infoStyle);
            }
            GUILayout.EndVertical();
        }

        #endregion

        #region Save and Load

        // Saves the settings when this object is destroyed.
        public void OnDestroy()
        {
            try
            {
                SettingsList list = new SettingsList();
                list.AddSetting("visible", _visible);
                list.AddSetting("x", _windowPosition.x);
                list.AddSetting("y", _windowPosition.y);
                list.AddSetting("compact", _compactMode);
                list.AddSetting("all_stages", _showAllStages);
                list.AddSetting("atmosphere", _useAtmosphericDetails);
                list.AddSetting("bodies", _showReferenceBodies);
                list.AddSetting("selected_body", CelestialBodies.Instance.SelectedBodyName);
                SettingsList.SaveToFile(EngineerGlobals.AssemblyPath + "Settings/BuildAdvanced", list);

                print("[KerbalEngineer/BuildAdvanced]: Successfully saved settings.");
            }
            catch { print("[KerbalEngineer/BuildAdvanced]: Failed to save settings."); }
        }

        // Loads the settings when this object is created.
        public void Awake()
        {
            try
            {
                SettingsList list = SettingsList.CreateFromFile(EngineerGlobals.AssemblyPath + "Settings/BuildAdvanced");
                _visible = (bool)list.GetSetting("visible", _visible);
                _windowPosition.x = (float)list.GetSetting("x", _windowPosition.x);
                _windowPosition.y = (float)list.GetSetting("y", _windowPosition.y);
                _compactMode = (bool)list.GetSetting("compact", _compactMode);
                _showAllStages = (bool)list.GetSetting("all_stages", _showAllStages);
                _useAtmosphericDetails = (bool)list.GetSetting("atmosphere", _useAtmosphericDetails);
                _showReferenceBodies = (bool)list.GetSetting("bodies", _showReferenceBodies);
                CelestialBodies.Instance.SelectedBodyName = (string)list.GetSetting("selected_body", "Kerbin");

                print("[KerbalEngineer/BuildAdvanced]: Successfully loaded settings.");
            }
            catch { print("[KerbalEngineer/BuildAdvanced]: Failed to load settings."); }
        }

        #endregion
    }
}
