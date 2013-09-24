// Name:    Kerbal Engineer Redux
// Author:  CYBUTEK
// License: Attribution-NonCommercial-ShareAlike 3.0 Unported

using System.Collections.Generic;
using System.Diagnostics;
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
        private GUIStyle _windowStyle, _titleStyle, _infoStyle, _tooltipTitleStyle, _tooltipInfoStyle;
        private int _windowID = EngineerGlobals.GetNextWindowID();
        private bool _hasInitStyles = false;

        private Part _selectedPart = null;
        private Stopwatch _tooltipInfoTimer = new Stopwatch();
        private double _tooltipInfoDelay = 0.5d;

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

            _titleStyle = new GUIStyle(HighLogic.Skin.label);
            _titleStyle.normal.textColor = Color.white;
            _titleStyle.margin = new RectOffset();
            _titleStyle.padding = new RectOffset();
            _titleStyle.fontSize = 11;
            _titleStyle.fontStyle = FontStyle.Bold;
            _titleStyle.stretchWidth = true;

            _infoStyle = new GUIStyle(HighLogic.Skin.label);
            _infoStyle.margin = new RectOffset();
            _infoStyle.padding = new RectOffset();
            _infoStyle.fontSize = 11;
            _infoStyle.fontStyle = FontStyle.Bold;
            _infoStyle.stretchWidth = true;

            _tooltipTitleStyle = new GUIStyle(HighLogic.Skin.label);
            _tooltipTitleStyle.normal.textColor = Color.white;
            _tooltipTitleStyle.fontSize = 11;
            _tooltipTitleStyle.fontStyle = FontStyle.Bold;
            _tooltipTitleStyle.stretchWidth = true;

            _tooltipInfoStyle = new GUIStyle(HighLogic.Skin.label);
            _tooltipInfoStyle.fontSize = 11;
            _tooltipInfoStyle.fontStyle = FontStyle.Bold;
            _tooltipInfoStyle.stretchWidth = true;
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
                        // Do not allow the extended information to be shown.
                        if (_selectedPart != null)
                        {
                            _selectedPart = null;
                            _tooltipInfoTimer.Reset();
                        }

                        DrawTooltip(EditorLogic.SelectedPart);
                    }
                    else
                    {
                        bool isPartSelected = false;
                        foreach (Part part in EditorLogic.SortedShipList)
                        { 
                            if (part.stackIcon.highlightIcon)
                            {
                                // Start the extended information timer.
                                if (part != _selectedPart)
                                {
                                    _selectedPart = part;
                                    _tooltipInfoTimer.Reset();
                                    _tooltipInfoTimer.Start();
                                }
                                isPartSelected = true;

                                DrawTooltip(part);
                                break;
                            }
                        }

                        // If no part is being hovered over we must reset the extended information timer.
                        if (!isPartSelected)
                        {
                            if (_selectedPart != null)
                            {
                                _selectedPart = null;
                                _tooltipInfoTimer.Reset();
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
            GUILayout.Label("Parts:", _titleStyle);
            GUILayout.Label("Delta-V:", _titleStyle);
            GUILayout.Label("TWR:", _titleStyle);
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
            // Tooltip title (name of part).
            GUIContent content = new GUIContent(part.partInfo.title);
            Vector2 size = _tooltipTitleStyle.CalcSize(content);
            Rect position = new Rect(Event.current.mousePosition.x + 16f, Event.current.mousePosition.y, size.x, size.y).ClampInsideScreen();
            if (position.x < Event.current.mousePosition.x + 16f) position.y += 16f;
            GUI.Label(position, content, _tooltipTitleStyle);

            // After hovering for a period of time, show extended information.
            if (_tooltipInfoTimer.Elapsed.TotalSeconds >= _tooltipInfoDelay)
            {
                // Stop the timer as it is no longer needed.
                if (_tooltipInfoTimer.IsRunning)
                    _tooltipInfoTimer.Stop();

                // Show the dry mass of the part if applicable.
                if (part.physicalSignificance == Part.PhysicalSignificance.FULL)
                    DrawTooltipInfo(ref position, "Dry Mass: " + part.GetDryMass().ToMass());

                // Show resources contained within the part.
                if (part.ContainsResources())
                {
                    // Show the wet mass of the part if applicable.
                    if (part.GetResourceMass() > 0f)
                        DrawTooltipInfo(ref position, "Wet Mass: " + part.GetWetMass().ToMass());

                    // List all the resources contained within the part.
                    foreach (PartResource resource in part.Resources)
                    {
                        double density = resource.GetDensity();
                        if (density > 0d)
                            DrawTooltipInfo(ref position, resource.info.name + ": " + resource.GetMass().ToMass() + " (" + resource.amount + ")");
                        else
                            DrawTooltipInfo(ref position, resource.info.name + ": " + resource.amount);
                    }
                }

                // Show details for engines.
                if (part.IsEngine())
                {
                    DrawTooltipInfo(ref position, "Maximum Thrust: " + part.GetMaxThrust().ToForce());
                    DrawTooltipInfo(ref position, "Specific Impulse: " + part.GetSpecificImpulse(1f) + " / " + part.GetSpecificImpulse(0f) + "s");

                    // Thrust vectoring.
                    if (part.HasModule("ModuleGimbal"))
                        DrawTooltipInfo(ref position, "Thrust Vectoring Enabled");

                    // Contains alternator.
                    if (part.HasModule("ModuleAlternator"))
                        DrawTooltipInfo(ref position, "Contains Alternator");
                }

                // Show details for solar panels.
                if (part.IsSolarPanel())
                {
                    ModuleDeployableSolarPanel module = part.GetModuleDeployableSolarPanel();
                    DrawTooltipInfo(ref position, "Charge Rate: " + module.chargeRate.ToDouble().ToRate());
                }

                // Show details for generators.
                if (part.IsGenerator())
                {
                    foreach (ModuleGenerator.GeneratorResource resource in part.GetModuleGenerator().inputList)
                        DrawTooltipInfo(ref position, "Input: " + resource.name + " (" + resource.rate.ToDouble().ToRate() + ")");

                    foreach (ModuleGenerator.GeneratorResource resource in part.GetModuleGenerator().outputList)
                        DrawTooltipInfo(ref position, "Output: " + resource.name + " (" + resource.rate.ToDouble().ToRate() + ")");
                }

                // Show details for parachutes.
                if (part.IsParachute())
                {
                    ModuleParachute module = part.GetModuleParachute();
                    DrawTooltipInfo(ref position, "Semi Deployed Drag: " + module.semiDeployedDrag);
                    DrawTooltipInfo(ref position, "Fully Deployed Drag: " + module.fullyDeployedDrag);
                    DrawTooltipInfo(ref position, "Deployment Altitude: " + module.deployAltitude.ToDouble().ToDistance());
                }

                // Contains stability augmentation system.
                if (part.HasModule("ModuleSAS"))
                    DrawTooltipInfo(ref position, "Contains SAS");

                // Contains reaction wheels.
                if (part.HasModule("ModuleReactionWheel"))
                    DrawTooltipInfo(ref position, "Contains Reaction Wheels");

                // Show if the part has an animation that can only be used once.
                if (part.HasOneShotAnimation())
                    DrawTooltipInfo(ref position, "Single Activation Only");
            }
        }

        // Draws a line of extended information below the previous.
        private void DrawTooltipInfo(ref Rect position, string value)
        {
            GUIContent content = new GUIContent(value);
            Vector2 size = _tooltipInfoStyle.CalcSize(content);
            position.y += 16f;
            position.width = size.x;
            position.height = size.y;
            GUI.Label(position, content, _tooltipInfoStyle);
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
