// Name:    Kerbal Engineer Redux
// Author:  CYBUTEK
// License: Attribution-NonCommercial-ShareAlike 3.0 Unported

using KerbalEngineer.Extensions;
using KerbalEngineer.Settings;
using KerbalEngineer.Simulation;
using UnityEngine;

namespace KerbalEngineer.BuildEngineer
{
    [KSPAddon(KSPAddon.Startup.EditorAny, false)]
    public class BuildOverlay : MonoBehaviour
    {
        #region Instance

        /// <summary>
        /// Gets the current instance if started or returns null.
        /// </summary>
        public static BuildOverlay Instance { get; private set; }

        #endregion

        #region Fields

        private Rect _position = new Rect(265f, 0f, 0f, 0f);
        private GUIStyle _windowStyle, _infoStyle, _tooltipStyle;
        private int _windowID = EngineerGlobals.GetNextWindowID();
        private bool _hasInitStyles = false;

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

        #endregion

        #region Initialisation

        public void Start()
        {
            // Set the instance to this object.
            Instance = this;

            RenderingManager.AddToPostDrawQueue(0, OnDraw);
        }

        private void InitialiseStyles()
        {
            _windowStyle = new GUIStyle(GUIStyle.none);
            _windowStyle.margin = new RectOffset();
            _windowStyle.padding = new RectOffset();

            _infoStyle = new GUIStyle(HighLogic.Skin.label);
            _infoStyle.margin = new RectOffset();
            _infoStyle.padding = new RectOffset();
            _infoStyle.fontSize = 11;
            _infoStyle.fontStyle = FontStyle.Bold;
            _infoStyle.stretchWidth = true;

            _tooltipStyle = new GUIStyle(HighLogic.Skin.label);
            _tooltipStyle.fontSize = 11;
            _tooltipStyle.fontStyle = FontStyle.Bold;
            _tooltipStyle.stretchWidth = true;
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

                    if (BuildAdvanced.Instance.UseAtmosphericDetails)
                        SimulationManager.Instance.Atmosphere = CelestialBodies.Instance.SelectedBodyInfo.Atmosphere;
                    else
                        SimulationManager.Instance.Atmosphere = 0d;

                    SimulationManager.Instance.TryStartSimulation();
                }
            } catch { /* A null reference exception is thrown when checking if EditorLogic.fetch != null??? */ }
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

                    _position = GUILayout.Window(_windowID, _position, OnWindow, string.Empty, _windowStyle);

                    // Check and set that the window is at the bottom of the screen.
                    if (_position.y + _position.height != Screen.height - 5f)
                        _position.y = Screen.height - _position.height - 5f;

                    // Find if a part is selected or being hovered over.
                    if (EditorLogic.SelectedPart != null)
                    {
                        DrawTooltip(EditorLogic.SelectedPart);
                    }
                    else
                    {
                        foreach (Part part in EditorLogic.SortedShipList)
                        {
                            if (part.stackIcon.highlightIcon)
                            {
                                DrawTooltip(part);
                                break;
                            }
                        }
                    }
                }
            }
            catch { /* A null reference exception is thrown when checking if EditorLogic.fetch != null??? */ }
        }

        private void OnWindow(int windowID)
        {
            GUILayout.BeginHorizontal();

            // Titles
            GUILayout.BeginVertical(GUILayout.Width(75f));
            GUILayout.Label("Parts:", _infoStyle);
            GUILayout.Label("Delta-V:", _infoStyle);
            GUILayout.Label("TWR:", _infoStyle);
            GUILayout.EndVertical();

            // Details
            GUILayout.BeginVertical(GUILayout.Width(100f));
            GUILayout.Label(SimulationManager.Instance.LastStage.partCount.ToString(), _infoStyle);
            GUILayout.Label(SimulationManager.Instance.LastStage.totalDeltaV.ToString("#,0.") + " m/s", _infoStyle);
            GUILayout.Label(SimulationManager.Instance.LastStage.TWR, _infoStyle);
            GUILayout.EndVertical();

            GUILayout.EndHorizontal();
        }

        // Draws the tooltip details of the selected/highlighted part.
        private void DrawTooltip(Part part)
        {
            GUIContent content = new GUIContent(part.partInfo.title);
            Vector2 size = _tooltipStyle.CalcSize(content);
            Rect position = new Rect(Event.current.mousePosition.x + 16f, Event.current.mousePosition.y, size.x, size.y).ClampInsideScreen();
            if (position.x < Event.current.mousePosition.x + 16f) position.y += 16f;
            GUI.Label(position, content, _tooltipStyle);
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
                SettingsList.SaveToFile(EngineerGlobals.AssemblyPath + "Settings/BuildOverlay", list);

                print("[KerbalEngineer/BuildOverlay]: Successfully saved settings.");
            }
            catch { print("[KerbalEngineer/BuildOverlay]: Failed to save settings."); }
        }

        // loads the settings when this object is created.
        public void Awake()
        {
            try
            {
                SettingsList list = SettingsList.CreateFromFile(EngineerGlobals.AssemblyPath + "Settings/BuildOverlay");
                _visible = (bool)list.GetSetting("visible", _visible);

                print("[KerbalEngineer/BuildOverlay]: Successfully loaded settings.");
            }
            catch { print("[KerbalEngineer/BuildOverlay]: Failed to load settigns."); }
        }

        #endregion
    }
}
