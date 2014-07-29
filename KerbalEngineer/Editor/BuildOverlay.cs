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

using KerbalEngineer.Extensions;
using KerbalEngineer.Settings;
using KerbalEngineer.VesselSimulator;

using UnityEngine;

#endregion

namespace KerbalEngineer.Editor
{
    [KSPAddon(KSPAddon.Startup.EditorAny, false)]
    public class BuildOverlay : MonoBehaviour
    {
        #region Instance

        /// <summary>
        ///     Gets the current instance if started or returns null.
        /// </summary>
        public static BuildOverlay Instance { get; private set; }

        #endregion

        #region Fields

        private readonly Stopwatch tooltipInfoTimer = new Stopwatch();
        private Stage lastStage;

        private Part selectedPart;
        private int windowId;
        private Rect windowPosition = new Rect(300.0f, 0, 0, 0);

        #endregion

        #region Constructors

        private void Awake()
        {
            Instance = this;
            GuiDisplaySize.OnSizeChanged += this.OnSizeChanged;
        }

        private void Start()
        {
            this.windowId = this.GetHashCode();
            this.InitialiseStyles();
            this.Load();
            RenderingManager.AddToPostDrawQueue(0, this.OnDraw);
        }

        #endregion

        #region Properties

        private float tooltipInfoDelay = 0.5f;
        private bool visible = true;

        public float TooltipInfoDelay
        {
            get { return this.tooltipInfoDelay; }
            set
            {
                this.tooltipInfoDelay = value;
                Logger.Log("BuildOverlay->TooltipInfoDelay = " + value);
            }
        }

        /// <summary>
        ///     Gets and sets whether the display is enabled.
        /// </summary>
        public bool Visible
        {
            get { return this.visible; }
            set
            {
                this.visible = value;
                Logger.Log("BuildOverlay->Visible = " + value);
            }
        }

        #endregion

        #region GUIStyles

        private GUIStyle infoStyle;
        private GUIStyle titleStyle;
        private GUIStyle tooltipInfoStyle;
        private GUIStyle tooltipTitleStyle;
        private GUIStyle windowStyle;

        private void InitialiseStyles()
        {
            try
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

                this.tooltipTitleStyle = new GUIStyle(HighLogic.Skin.label)
                {
                    normal =
                    {
                        textColor = Color.white
                    },
                    fontSize = (int)(11 * GuiDisplaySize.Offset),
                    fontStyle = FontStyle.Bold,
                    stretchWidth = true
                };

                this.tooltipInfoStyle = new GUIStyle(HighLogic.Skin.label)
                {
                    fontSize = (int)(11 * GuiDisplaySize.Offset),
                    fontStyle = FontStyle.Bold,
                    stretchWidth = true
                };
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "BuildOverlay->InitialiseStyles");
            }
        }

        private void OnSizeChanged()
        {
            this.InitialiseStyles();
            this.windowPosition.width = 0;
            this.windowPosition.height = 0;
        }

        #endregion

        #region Update and Drawing

        private void Update()
        {
            try
            {
                if (!this.visible || BuildAdvanced.Instance == null || EditorLogic.fetch == null || EditorLogic.fetch.ship.parts.Count == 0 || EditorLogic.fetch.editorScreen != EditorLogic.EditorScreen.Parts)
                {
                    return;
                }

                // Configure the simulation parameters based on the selected reference body.
                SimManager.Gravity = CelestialBodies.Instance.SelectedBodyInfo.Gravity;

                if (BuildAdvanced.Instance.ShowAtmosphericDetails)
                {
                    SimManager.Atmosphere = CelestialBodies.Instance.SelectedBodyInfo.Atmosphere * 0.01d;
                }
                else
                {
                    SimManager.Atmosphere = 0;
                }

                SimManager.TryStartSimulation();
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "BuildOverlay->Update");
            }
        }

        private void OnDraw()
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

                // Find if a part is selected or being hovered over.
                if (EditorLogic.SelectedPart != null)
                {
                    // Do not allow the extended information to be shown.
                    if (this.selectedPart != null)
                    {
                        this.selectedPart = null;
                        this.tooltipInfoTimer.Reset();
                    }

                    this.DrawTooltip(EditorLogic.SelectedPart);
                }
                else
                {
                    var isPartSelected = false;
                    foreach (var part in EditorLogic.SortedShipList)
                    {
                        if (part.stackIcon.highlightIcon)
                        {
                            // Start the extended information timer.
                            if (part != this.selectedPart)
                            {
                                this.selectedPart = part;
                                this.tooltipInfoTimer.Reset();
                                this.tooltipInfoTimer.Start();
                            }
                            isPartSelected = true;

                            this.DrawTooltip(part);
                            break;
                        }
                    }

                    // If no part is being hovered over we must reset the extended information timer.
                    if (!isPartSelected && this.selectedPart != null)
                    {
                        this.selectedPart = null;
                        this.tooltipInfoTimer.Reset();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "BuildOverlay->OnDraw");
            }
        }

        private void Window(int windowId)
        {
            try
            {
                GUILayout.BeginHorizontal();

                // Titles
                GUILayout.BeginVertical(GUILayout.Width(75.0f * GuiDisplaySize.Offset));
                //GUILayout.Label("Parts:", this.titleStyle);
                GUILayout.Label("Delta-V:", this.titleStyle);
                GUILayout.Label("TWR:", this.titleStyle);
                GUILayout.EndVertical();

                // Details
                GUILayout.BeginVertical(GUILayout.Width(100.0f * GuiDisplaySize.Offset));
                //GUILayout.Label(SimulationManager.Instance.LastStage.partCount.ToString("N0"), this.infoStyle);
                GUILayout.Label(this.lastStage.totalDeltaV.ToString("N0") + " m/s", this.infoStyle);
                GUILayout.Label(this.lastStage.thrustToWeight.ToString("F2"), this.infoStyle);
                GUILayout.EndVertical();

                GUILayout.EndHorizontal();
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "BuildOverlay->Window");
            }
        }

        /// <summary>
        ///     Draws the tooltip details of the selected/highlighted part.
        /// </summary>
        private void DrawTooltip(Part part)
        {
            try
            {
                // Tooltip title (name of part).
                var content = new GUIContent(part.partInfo.title);
                var size = this.tooltipTitleStyle.CalcSize(content);
                var position = new Rect(Event.current.mousePosition.x + 16.0f, Event.current.mousePosition.y, size.x, size.y).ClampInsideScreen();

                if (position.x < Event.current.mousePosition.x + 16.0f)
                {
                    position.y += 16.0f;
                }
                GUI.Label(position, content, this.tooltipTitleStyle);

                // After hovering for a period of time, show extended information.
                if (this.tooltipInfoTimer.Elapsed.TotalSeconds >= this.tooltipInfoDelay)
                {
                    // Stop the timer as it is no longer needed.
                    if (this.tooltipInfoTimer.IsRunning)
                    {
                        this.tooltipInfoTimer.Stop();
                    }

                    // Show the dry mass of the part if applicable.
                    if (part.physicalSignificance == Part.PhysicalSignificance.FULL)
                    {
                        this.DrawTooltipInfo(ref position, "Dry Mass: " + part.GetDryMass().ToMass());
                    }

                    // Show resources contained within the part.
                    if (part.ContainsResources())
                    {
                        // Show the wet mass of the part if applicable.
                        if (part.GetResourceMass() > 0)
                        {
                            this.DrawTooltipInfo(ref position, "Wet Mass: " + part.GetWetMass().ToMass());
                        }

                        // List all the resources contained within the part.
                        foreach (PartResource resource in part.Resources)
                        {
                            if (resource.GetDensity() > 0)
                            {
                                this.DrawTooltipInfo(ref position, resource.info.name + ": " + resource.GetMass().ToMass() + " (" + resource.amount + ")");
                            }
                            else
                            {
                                this.DrawTooltipInfo(ref position, resource.info.name + ": " + resource.amount);
                            }
                        }
                    }

                    // Show details for engines.
                    if (part.IsEngine())
                    {
                        this.DrawTooltipInfo(ref position, "Maximum Thrust: " + part.GetMaxThrust().ToForce());
                        this.DrawTooltipInfo(ref position, "Specific Impulse: " + part.GetSpecificImpulse(1f) + " / " + part.GetSpecificImpulse(0f) + "s");

                        // Thrust vectoring.
                        if (part.HasModule("ModuleGimbal"))
                        {
                            this.DrawTooltipInfo(ref position, "Thrust Vectoring Enabled");
                        }

                        // Contains alternator.
                        if (part.HasModule("ModuleAlternator"))
                        {
                            this.DrawTooltipInfo(ref position, "Contains Alternator");
                        }
                    }

                    // Show details for RCS.
                    if (part.IsRcsModule())
                    {
                        var moduleRcs = part.GetModuleRcs();
                        this.DrawTooltipInfo(ref position, "Thrust Power: " + moduleRcs.thrusterPower.ToDouble().ToForce());
                        this.DrawTooltipInfo(ref position, "Specific Impulse: " + moduleRcs.atmosphereCurve.Evaluate(1f) + " / " + moduleRcs.atmosphereCurve.Evaluate(0f) + "s");
                    }

                    // Show details for solar panels.
                    if (part.IsSolarPanel())
                    {
                        this.DrawTooltipInfo(ref position, "Charge Rate: " + part.GetModuleDeployableSolarPanel().chargeRate.ToDouble().ToRate());
                    }

                    // Show details for generators.
                    if (part.IsGenerator())
                    {
                        foreach (var resource in part.GetModuleGenerator().inputList)
                        {
                            this.DrawTooltipInfo(ref position, "Input: " + resource.name + " (" + resource.rate.ToDouble().ToRate() + ")");
                        }

                        foreach (var resource in part.GetModuleGenerator().outputList)
                        {
                            this.DrawTooltipInfo(ref position, "Output: " + resource.name + " (" + resource.rate.ToDouble().ToRate() + ")");
                        }
                    }

                    // Show details for parachutes.
                    if (part.IsParachute())
                    {
                        var module = part.GetModuleParachute();
                        this.DrawTooltipInfo(ref position, "Semi Deployed Drag: " + module.semiDeployedDrag);
                        this.DrawTooltipInfo(ref position, "Fully Deployed Drag: " + module.fullyDeployedDrag);
                        this.DrawTooltipInfo(ref position, "Deployment Altitude: " + module.deployAltitude.ToDouble().ToDistance());
                    }

                    // Contains stability augmentation system.
                    if (part.HasModule("ModuleSAS"))
                    {
                        this.DrawTooltipInfo(ref position, "Contains SAS");
                    }

                    // Contains reaction wheels.
                    if (part.HasModule("ModuleReactionWheel"))
                    {
                        this.DrawTooltipInfo(ref position, "Contains Reaction Wheels");
                    }

                    // Show if the part has an animation that can only be used once.
                    if (part.HasOneShotAnimation())
                    {
                        this.DrawTooltipInfo(ref position, "Single Activation Only");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "BuildOverlay->DrawTooltip");
            }
        }

        /// <summary>
        ///     Draws a line of extended information below the previous.
        /// </summary>
        private void DrawTooltipInfo(ref Rect position, string value)
        {
            try
            {
                var content = new GUIContent(value);
                var size = this.tooltipInfoStyle.CalcSize(content);

                position.y += 16.0f * GuiDisplaySize.Offset;
                position.width = size.x;
                position.height = size.y;
                GUI.Label(position, content, this.tooltipInfoStyle);
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "BuildOverlay->DrawTooltipInfo");
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
                handler.Save("BuildOverlay.xml");
                GuiDisplaySize.OnSizeChanged -= this.OnSizeChanged;
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "BuildOverlay->OnDestroy");
            }
        }

        /// <summary>
        ///     Loads the settings when this object is created.
        /// </summary>
        private void Load()
        {
            try
            {
                var handler = SettingHandler.Load("BuildOverlay.xml");
                handler.Get("visible", ref this.visible);
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "BuildOverlay->Load");
            }
        }

        #endregion
    }
}