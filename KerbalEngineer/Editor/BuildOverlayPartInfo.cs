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
using KerbalEngineer.Helpers;

using UnityEngine;

#endregion

namespace KerbalEngineer.Editor
{
    public class BuildOverlayPartInfo : MonoBehaviour
    {
        #region Fields

        private static bool clickToOpen = true;
        private static bool namesOnly;
        private static bool visible = true;

        private readonly List<PartInfoItem> infoItems = new List<PartInfoItem>();

        private Rect position;
        private Part selectedPart;
        private bool showInfo;
        private bool skipFrame;

        #endregion

        #region Properties

        public static bool ClickToOpen
        {
            get { return clickToOpen; }
            set { clickToOpen = value; }
        }

        public static bool Hidden { get; set; }

        public static bool NamesOnly
        {
            get { return namesOnly; }
            set { namesOnly = value; }
        }

        public static bool Visible
        {
            get { return visible; }
            set { visible = value; }
        }

        #endregion

        #region Methods: protected

        protected void OnGUI()
        {
            try
            {
                if (!Visible || Hidden || this.selectedPart == null)
                {
                    return;
                }

                this.position = GUILayout.Window(this.GetInstanceID(), this.position, this.Window, String.Empty, BuildOverlay.WindowStyle);
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
                if (!Visible || Hidden || EditorLogic.startPod == null || EditorLogic.fetch.editorScreen != EditorLogic.EditorScreen.Parts)
                {
                    return;
                }

                this.position.x = Mathf.Clamp(Input.mousePosition.x + 16.0f, 0.0f, Screen.width - this.position.width);
                this.position.y = Mathf.Clamp(Screen.height - Input.mousePosition.y, 0.0f, Screen.height - this.position.height);
                if (this.position.x < Input.mousePosition.x + 20.0f)
                {
                    this.position.y = Mathf.Clamp(this.position.y + 20.0f, 0.0f, Screen.height - this.position.height);
                }
                if (this.position.x < Input.mousePosition.x + 16.0f && this.position.y < Screen.height - Input.mousePosition.y)
                {
                    this.position.x = Input.mousePosition.x - 3 - this.position.width;
                }

                this.infoItems.Clear();
                var part = EditorLogic.fetch.ship.parts.Find(p => p.stackIcon.highlightIcon) ?? EditorLogic.SelectedPart;
                if (part != null)
                {
                    if (!part.Equals(this.selectedPart))
                    {
                        this.selectedPart = part;
                        this.ResetInfo();
                    }
                    if (NamesOnly || this.skipFrame)
                    {
                        this.skipFrame = false;
                        return;
                    }

                    this.SetCostInfo();
                    this.SetMassItems();
                    this.SetResourceItems();
                    this.SetEngineInfo();
                    this.SetAlternatorInfo();
                    this.SetGimbalInfo();
                    this.SetRcsInfo();
                    this.SetParachuteInfo();
                    this.SetSasInfo();
                    this.SetReactionWheelInfo();
                    this.SetSolarPanelInfo();
                    this.SetGeneratorInfo();
                    this.SetDecouplerInfo();
                    this.SetTransmitterInfo();
                    this.SetScienceExperimentInfo();
                    this.SetScienceContainerInfo();
                    this.SetSingleActivationInfo();

                    if (!this.showInfo && Input.GetMouseButtonDown(2))
                    {
                        this.showInfo = true;
                    }
                    else if (ClickToOpen && this.showInfo && Input.GetMouseButtonDown(2))
                    {
                        this.ResetInfo();
                    }
                }
                else
                {
                    this.selectedPart = null;
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }

        #endregion

        #region Methods: private

        private void ResetInfo()
        {
            this.showInfo = !clickToOpen;
            this.skipFrame = true;
            this.position.width = namesOnly || clickToOpen ? 0.0f : 200.0f;
            this.position.height = 0.0f;
        }

        private void SetAlternatorInfo()
        {
            if (!this.selectedPart.HasModule<ModuleAlternator>())
            {
                return;
            }

            var alternator = this.selectedPart.GetModule<ModuleAlternator>();
            this.infoItems.Add(new PartInfoItem("Alternator"));
            foreach (var resource in alternator.outputResources)
            {
                this.infoItems.Add(new PartInfoItem("\t" + resource.name, resource.rate.ToRate()));
            }
        }

        private void SetCostInfo()
        {
            this.infoItems.Add(new PartInfoItem("Cost", Units.Concat(this.selectedPart.GetCostDry(), this.selectedPart.GetCostWet())));
        }

        private void SetDecouplerInfo()
        {
            if (!this.selectedPart.IsDecoupler())
            {
                return;
            }

            var decoupler = this.selectedPart.GetProtoModuleDecoupler();
            this.infoItems.Add(new PartInfoItem("Ejection Force", decoupler.EjectionForce.ToForce()));
            if (decoupler.IsOmniDecoupler)
            {
                this.infoItems.Add(new PartInfoItem("Omni-directional"));
            }
        }

        private void SetEngineInfo()
        {
            if (!this.selectedPart.IsEngine())
            {
                return;
            }

            var engine = this.selectedPart.GetProtoModuleEngine();
            this.infoItems.Add(new PartInfoItem("Thrust", Units.ToForce(engine.MinimumThrust, engine.MaximumThrust)));
            this.infoItems.Add(new PartInfoItem("Isp", Units.Concat(engine.GetSpecificImpulse(1.0f), engine.GetSpecificImpulse(0.0f)) + "s"));
            if (engine.Propellants.Count > 0)
            {
                this.infoItems.Add(new PartInfoItem("Propellants"));
                var totalRatio = engine.Propellants.Sum(p => p.ratio);
                foreach (var propellant in engine.Propellants)
                {
                    this.infoItems.Add(new PartInfoItem("\t" + propellant.name, (propellant.ratio / totalRatio).ToPercent()));
                }
            }
        }

        private void SetGeneratorInfo()
        {
            if (!this.selectedPart.HasModule<ModuleGenerator>())
            {
                return;
            }

            var generator = this.selectedPart.GetModule<ModuleGenerator>();
            if (generator.inputList.Count > 0)
            {
                this.infoItems.Add(new PartInfoItem("Generator Input"));
                foreach (var resource in generator.inputList)
                {
                    this.infoItems.Add(new PartInfoItem("\t" + resource.name, resource.rate.ToRate()));
                }
            }
            if (generator.outputList.Count > 0)
            {
                this.infoItems.Add(new PartInfoItem("Generator Output"));
                foreach (var resource in generator.outputList)
                {
                    this.infoItems.Add(new PartInfoItem("\t" + resource.name, resource.rate.ToRate()));
                }
            }
            if (generator.isAlwaysActive)
            {
                this.infoItems.Add(new PartInfoItem("Generator is Always Active"));
            }
        }

        private void SetGimbalInfo()
        {
            if (!this.selectedPart.HasModule<ModuleGimbal>())
            {
                return;
            }

            var gimbal = this.selectedPart.GetModule<ModuleGimbal>();
            this.infoItems.Add(new PartInfoItem("Thrust Vectoring", gimbal.gimbalRange.ToString("F2")));
        }

        private void SetMassItems()
        {
            if (this.selectedPart.physicalSignificance == Part.PhysicalSignificance.FULL)
            {
                this.infoItems.Add(new PartInfoItem("Mass", Units.ToMass(this.selectedPart.GetDryMass(), this.selectedPart.GetWetMass())));
            }
        }

        private void SetParachuteInfo()
        {
            if (!this.selectedPart.HasModule<ModuleParachute>())
            {
                return;
            }

            var parachute = this.selectedPart.GetModule<ModuleParachute>();
            this.infoItems.Add(new PartInfoItem("Deployed Drag", Units.Concat(parachute.semiDeployedDrag, parachute.fullyDeployedDrag)));
            this.infoItems.Add(new PartInfoItem("Deployment Altitude", parachute.deployAltitude.ToDistance()));
            this.infoItems.Add(new PartInfoItem("Deployment Pressure", parachute.minAirPressureToOpen.ToString("F2")));
        }

        private void SetRcsInfo()
        {
            if (!this.selectedPart.HasModule<ModuleRCS>())
            {
                return;
            }

            var rcs = this.selectedPart.GetModule<ModuleRCS>();
            this.infoItems.Add(new PartInfoItem("Thruster Power", rcs.thrusterPower.ToForce()));
            this.infoItems.Add(new PartInfoItem("Specific Impulse", Units.Concat(rcs.atmosphereCurve.Evaluate(1.0f), rcs.atmosphereCurve.Evaluate(0.0f)) + "s"));
        }

        private void SetReactionWheelInfo()
        {
            if (!this.selectedPart.HasModule<ModuleReactionWheel>())
            {
                return;
            }

            var reactionWheel = this.selectedPart.GetModule<ModuleReactionWheel>();
            this.infoItems.Add(new PartInfoItem("Reaction Wheel Torque"));
            this.infoItems.Add(new PartInfoItem("\tPitch", reactionWheel.PitchTorque.ToTorque()));
            this.infoItems.Add(new PartInfoItem("\tRoll", reactionWheel.RollTorque.ToTorque()));
            this.infoItems.Add(new PartInfoItem("\tYaw", reactionWheel.YawTorque.ToTorque()));
            foreach (var resource in reactionWheel.inputResources)
            {
                this.infoItems.Add(new PartInfoItem("\t" + resource.name, resource.rate.ToRate()));
            }
        }

        private void SetResourceItems()
        {
            if (this.selectedPart.Resources.list.Any(r => !r.hideFlow))
            {
                this.infoItems.Add(new PartInfoItem("Resources"));
                foreach (var resource in this.selectedPart.Resources.list.Where(r => !r.hideFlow))
                {
                    this.infoItems.Add(resource.GetDensity() > 0
                        ? new PartInfoItem("\t" + resource.info.name, "(" + resource.GetMass().ToMass() + ") " + resource.amount.ToString("F1"))
                        : new PartInfoItem("\t" + resource.info.name, resource.amount.ToString("F1")));
                }
            }
        }

        private void SetSasInfo()
        {
            if (this.selectedPart.HasModule<ModuleSAS>())
            {
                this.infoItems.Add(new PartInfoItem("SAS Equiped"));
            }
        }

        private void SetScienceContainerInfo()
        {
            if (this.selectedPart.HasModule<ModuleScienceContainer>())
            {
                this.infoItems.Add(new PartInfoItem("Science Container"));
            }
        }

        private void SetScienceExperimentInfo()
        {
            if (!this.selectedPart.HasModule<ModuleScienceExperiment>())
            {
                return;
            }

            var experiment = this.selectedPart.GetModule<ModuleScienceExperiment>();
            this.infoItems.Add(new PartInfoItem("Science Experiment", experiment.experimentActionName));
            this.infoItems.Add(new PartInfoItem("\tTransmit Efficiency", experiment.xmitDataScalar.ToPercent()));
            if (!experiment.rerunnable)
            {
                this.infoItems.Add(new PartInfoItem("\tSingle Usage"));
            }
        }

        private void SetSingleActivationInfo()
        {
            if (this.selectedPart.HasModule<ModuleAnimateGeneric>(m => m.isOneShot))
            {
                this.infoItems.Add(new PartInfoItem("Single Activation"));
            }
        }

        private void SetSolarPanelInfo()
        {
            if (!this.selectedPart.HasModule<ModuleDeployableSolarPanel>())
            {
                return;
            }

            var solarPanel = this.selectedPart.GetModule<ModuleDeployableSolarPanel>();
            this.infoItems.Add(new PartInfoItem("Charge Rate", solarPanel.chargeRate.ToRate()));
            if (solarPanel.isBreakable)
            {
                this.infoItems.Add(new PartInfoItem("Breakable"));
            }
            if (solarPanel.sunTracking)
            {
                this.infoItems.Add(new PartInfoItem("Sun Tracking"));
            }
        }

        private void SetTransmitterInfo()
        {
            if (!this.selectedPart.HasModule<ModuleDataTransmitter>())
            {
                return;
            }

            var transmitter = this.selectedPart.GetModule<ModuleDataTransmitter>();
            this.infoItems.Add(new PartInfoItem("Packet Size", transmitter.packetSize.ToString("F2") + " Mits"));
            this.infoItems.Add(new PartInfoItem("Bandwidth", (transmitter.packetInterval * transmitter.packetSize).ToString("F2") + "Mits/sec"));
            this.infoItems.Add(new PartInfoItem(transmitter.requiredResource, transmitter.packetResourceCost.ToString("F2") + "/Packet"));
        }

        private void Window(int windowId)
        {
            try
            {
                GUILayout.Label(this.selectedPart.partInfo.title, BuildOverlay.TitleStyle);
                if (this.showInfo)
                {
                    foreach (var item in this.infoItems)
                    {
                        GUILayout.Space(2.0f);
                        GUILayout.BeginHorizontal();
                        if (item.Value != null)
                        {
                            GUILayout.Label(item.Name + ":", BuildOverlay.NameStyle);
                            GUILayout.Space(25.0f);
                            GUILayout.Label(item.Value, BuildOverlay.ValueStyle);
                        }
                        else
                        {
                            GUILayout.Label(item.Name, BuildOverlay.NameStyle);
                        }
                        GUILayout.EndHorizontal();
                    }
                }
                else if (this.infoItems.Count > 0)
                {
                    GUILayout.Space(2.0f);
                    GUILayout.Label("Click middle mouse to show more info...", BuildOverlay.NameStyle);
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