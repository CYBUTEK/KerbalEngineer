// 
//     Kerbal Engineer Redux
// 
//     Copyright (C) 2016 CYBUTEK
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
//     You should have received a copy of the GNU General Public License
//     along with this program.  If not, see <http://www.gnu.org/licenses/>.
//  

namespace KerbalEngineer.Editor
{
    using System;
    using System.Collections.Generic;
    using Extensions;
    using Helpers;
    using UnityEngine;

    public class BuildOverlayPartInfo : MonoBehaviour
    {
        private static bool clickToOpen = true;
        private static ModuleResource generatorResource;
        private static ModuleAlternator moduleAlternator;
        private static ModuleDataTransmitter moduleDataTransmitter;
        private static ModuleDeployableSolarPanel moduleDeployableSolarPanel;
        private static ModuleGenerator moduleGenerator;
        private static ModuleGimbal moduleGimbal;
        private static ModuleParachute moduleParachute;
        private static ModuleRCS moduleRcs;
        private static ModuleReactionWheel moduleReactionWheel;
        private static ModuleResource moduleResource;
        private static ModuleScienceExperiment moduleScienceExperiment;
        private static bool namesOnly;
        private static Part part;
        private static PartInfoItem partInfoItem;
        private static PartResource partResource;
        private static Propellant propellant;
        private static PartExtensions.ProtoModuleDecoupler protoModuleDecoupler;
        private static PartExtensions.ProtoModuleEngine protoModuleEngine;
        private static bool visible = true;

        private readonly List<PartInfoItem> infoItems = new List<PartInfoItem>();

        private Rect position;
        private Part selectedPart;
        private bool showInfo;
        private bool skipFrame;

        public static bool ClickToOpen
        {
            get
            {
                return clickToOpen;
            }
            set
            {
                clickToOpen = value;
            }
        }

        public static bool Hidden { get; set; }

        public static bool NamesOnly
        {
            get
            {
                return namesOnly;
            }
            set
            {
                namesOnly = value;
            }
        }

        public static bool Visible
        {
            get
            {
                return visible;
            }
            set
            {
                visible = value;
            }
        }

        protected void OnGUI()
        {
            try
            {
                if (!Visible || Hidden || selectedPart == null)
                {
                    return;
                }

                position = GUILayout.Window(GetInstanceID(), position, Window, string.Empty, BuildOverlay.WindowStyle);
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
                if (!Visible || Hidden || EditorLogic.RootPart == null || EditorLogic.fetch.editorScreen != EditorScreen.Parts)
                {
                    return;
                }

                position.x = Mathf.Clamp(Input.mousePosition.x + 16.0f, 0.0f, Screen.width - position.width);
                position.y = Mathf.Clamp(Screen.height - Input.mousePosition.y, 0.0f, Screen.height - position.height);
                if (position.x < Input.mousePosition.x + 20.0f)
                {
                    position.y = Mathf.Clamp(position.y + 20.0f, 0.0f, Screen.height - position.height);
                }
                if (position.x < Input.mousePosition.x + 16.0f && position.y < Screen.height - Input.mousePosition.y)
                {
                    position.x = Input.mousePosition.x - 3 - position.width;
                }

                RaycastHit rayHit;
                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out rayHit))
                {
                    part = rayHit.transform.GetComponent<Part>();
                }
                else
                {
                    part = EditorLogic.fetch.ship.parts.Find(p => p.highlighter.highlighted) ?? EditorLogic.SelectedPart;
                }

                if (part != null)
                {
                    if (!part.Equals(selectedPart))
                    {
                        selectedPart = part;
                        ResetInfo();
                    }
                    if (NamesOnly || skipFrame)
                    {
                        skipFrame = false;
                        return;
                    }

                    if (!showInfo && Input.GetMouseButtonDown(2))
                    {
                        showInfo = true;
                    }
                    else if (ClickToOpen && showInfo && Input.GetMouseButtonDown(2))
                    {
                        ResetInfo();
                    }

                    if (showInfo)
                    {
                        PartInfoItem.Release(infoItems);
                        infoItems.Clear();
                        SetCostInfo();
                        SetMassItems();
                        SetResourceItems();
                        SetEngineInfo();
                        SetAlternatorInfo();
                        SetGimbalInfo();
                        SetRcsInfo();
                        SetParachuteInfo();
                        SetSasInfo();
                        SetReactionWheelInfo();
                        SetSolarPanelInfo();
                        SetGeneratorInfo();
                        SetDecouplerInfo();
                        SetTransmitterInfo();
                        SetScienceExperimentInfo();
                        SetScienceContainerInfo();
                        SetSingleActivationInfo();
                    }
                }
                else
                {
                    selectedPart = null;
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }

        private void ResetInfo()
        {
            showInfo = !clickToOpen;
            skipFrame = true;
            position.width = namesOnly || clickToOpen ? 0.0f : 200.0f;
            position.height = 0.0f;
        }

        private void SetAlternatorInfo()
        {
            moduleAlternator = selectedPart.GetModule<ModuleAlternator>();
            if (moduleAlternator != null)
            {
                infoItems.Add(PartInfoItem.Create("Alternator"));
                for (int i = 0; i < moduleAlternator.outputResources.Count; ++i)
                {
                    moduleResource = moduleAlternator.outputResources[i];
                    infoItems.Add(PartInfoItem.Create("\t" + moduleResource.name, moduleResource.rate.ToRate()));
                }
            }
        }

        private void SetCostInfo()
        {
            infoItems.Add(PartInfoItem.Create("Cost", Units.ConcatF(selectedPart.GetCostDry(), selectedPart.GetCostWet())));
        }

        private void SetDecouplerInfo()
        {
            protoModuleDecoupler = selectedPart.GetProtoModuleDecoupler();
            if (protoModuleDecoupler != null)
            {
                infoItems.Add(PartInfoItem.Create("Ejection Force", protoModuleDecoupler.EjectionForce.ToForce()));
                if (protoModuleDecoupler.IsOmniDecoupler)
                {
                    infoItems.Add(PartInfoItem.Create("Omni-directional"));
                }
            }
        }

        private void SetEngineInfo()
        {
            protoModuleEngine = selectedPart.GetProtoModuleEngine();
            if (protoModuleEngine != null)
            {
                infoItems.Add(PartInfoItem.Create("Thrust", Units.ToForce(protoModuleEngine.MinimumThrust, protoModuleEngine.MaximumThrust)));
                infoItems.Add(PartInfoItem.Create("Isp", Units.ConcatF(protoModuleEngine.GetSpecificImpulse(1.0f), protoModuleEngine.GetSpecificImpulse(0.0f)) + "s"));
                if (protoModuleEngine.Propellants.Count > 0)
                {
                    infoItems.Add(PartInfoItem.Create("Propellants"));

                    float totalRatio = 0.0f;
                    for (int i = 0; i < protoModuleEngine.Propellants.Count; ++i)
                    {
                        totalRatio = totalRatio + protoModuleEngine.Propellants[i].ratio;
                    }

                    for (int i = 0; i < protoModuleEngine.Propellants.Count; ++i)
                    {
                        propellant = protoModuleEngine.Propellants[i];
                        infoItems.Add(PartInfoItem.Create("\t" + propellant.name, (propellant.ratio / totalRatio).ToPercent()));
                    }
                }
            }
        }

        private void SetGeneratorInfo()
        {
            moduleGenerator = selectedPart.GetModule<ModuleGenerator>();
            if (moduleGenerator != null)
            {
                if (moduleGenerator.inputList.Count > 0)
                {
                    infoItems.Add(PartInfoItem.Create("Generator Input"));
                    for (int i = 0; i < moduleGenerator.inputList.Count; ++i)
                    {
                        generatorResource = moduleGenerator.inputList[i];
                        infoItems.Add(PartInfoItem.Create("\t" + generatorResource.name, generatorResource.rate.ToRate()));
                    }
                }
                if (moduleGenerator.outputList.Count > 0)
                {
                    infoItems.Add(PartInfoItem.Create("Generator Output"));
                    for (int i = 0; i < moduleGenerator.outputList.Count; ++i)
                    {
                        generatorResource = moduleGenerator.outputList[i];
                        infoItems.Add(PartInfoItem.Create("\t" + generatorResource.name, generatorResource.rate.ToRate()));
                    }
                }
                if (moduleGenerator.isAlwaysActive)
                {
                    infoItems.Add(PartInfoItem.Create("Generator is Always Active"));
                }
            }
        }

        private void SetGimbalInfo()
        {
            moduleGimbal = selectedPart.GetModule<ModuleGimbal>();
            if (moduleGimbal != null)
            {
                infoItems.Add(PartInfoItem.Create("Thrust Vectoring", moduleGimbal.gimbalRange.ToString("F2")));
            }
        }

        private void SetMassItems()
        {
            if (selectedPart.physicalSignificance == Part.PhysicalSignificance.FULL)
            {
                infoItems.Add(PartInfoItem.Create("Mass", Units.ToMass(selectedPart.GetDryMass(), selectedPart.GetWetMass())));
            }
        }

        private void SetParachuteInfo()
        {
            moduleParachute = selectedPart.GetModule<ModuleParachute>();
            if (moduleParachute != null)
            {
                infoItems.Add(PartInfoItem.Create("Deployed Drag", Units.ConcatF(moduleParachute.semiDeployedDrag, moduleParachute.fullyDeployedDrag)));
                infoItems.Add(PartInfoItem.Create("Deployment Altitude", moduleParachute.deployAltitude.ToDistance()));
                infoItems.Add(PartInfoItem.Create("Deployment Pressure", moduleParachute.minAirPressureToOpen.ToString("F2")));
            }
        }

        private void SetRcsInfo()
        {
            moduleRcs = selectedPart.GetModule<ModuleRCS>();
            if (moduleRcs != null)
            {
                infoItems.Add(PartInfoItem.Create("Thruster Power", moduleRcs.thrusterPower.ToForce()));
                infoItems.Add(PartInfoItem.Create("Specific Impulse", Units.ConcatF(moduleRcs.atmosphereCurve.Evaluate(1.0f), moduleRcs.atmosphereCurve.Evaluate(0.0f)) + "s"));
            }
        }

        private void SetReactionWheelInfo()
        {
            moduleReactionWheel = selectedPart.GetModule<ModuleReactionWheel>();
            if (moduleReactionWheel != null)
            {
                infoItems.Add(PartInfoItem.Create("Reaction Wheel Torque"));
                infoItems.Add(PartInfoItem.Create("\tPitch", moduleReactionWheel.PitchTorque.ToTorque()));
                infoItems.Add(PartInfoItem.Create("\tRoll", moduleReactionWheel.RollTorque.ToTorque()));
                infoItems.Add(PartInfoItem.Create("\tYaw", moduleReactionWheel.YawTorque.ToTorque()));
                for (int i = 0; i < moduleReactionWheel.inputResources.Count; ++i)
                {
                    moduleResource = moduleReactionWheel.inputResources[i];
                    infoItems.Add(PartInfoItem.Create("\t" + moduleResource.name, moduleResource.rate.ToRate()));
                }
            }
        }

        private void SetResourceItems()
        {
            bool visibleResources = false;
            for (int i = 0; i < selectedPart.Resources.list.Count; ++i)
            {
                if (selectedPart.Resources.list[i].hideFlow == false)
                {
                    visibleResources = true;
                    break;
                }
            }
            if (visibleResources)
            {
                infoItems.Add(PartInfoItem.Create("Resources"));
                for (int i = 0; i < selectedPart.Resources.list.Count; ++i)
                {
                    partResource = selectedPart.Resources.list[i];

                    if (partResource.hideFlow == false)
                    {
                        infoItems.Add(partResource.GetDensity() > 0
                            ? PartInfoItem.Create("\t" + partResource.info.name, "(" + partResource.GetMass().ToMass() + ") " + partResource.amount.ToString("F1"))
                            : PartInfoItem.Create("\t" + partResource.info.name, partResource.amount.ToString("F1")));
                    }
                }
            }
        }

        private void SetSasInfo()
        {
            if (selectedPart.HasModule<ModuleSAS>())
            {
                infoItems.Add(PartInfoItem.Create("SAS Equiped"));
            }
        }

        private void SetScienceContainerInfo()
        {
            if (selectedPart.HasModule<ModuleScienceContainer>())
            {
                infoItems.Add(PartInfoItem.Create("Science Container"));
            }
        }

        private void SetScienceExperimentInfo()
        {
            moduleScienceExperiment = selectedPart.GetModule<ModuleScienceExperiment>();
            if (moduleScienceExperiment != null)
            {
                infoItems.Add(PartInfoItem.Create("Science Experiment", moduleScienceExperiment.experimentActionName));
                infoItems.Add(PartInfoItem.Create("\tTransmit Efficiency", moduleScienceExperiment.xmitDataScalar.ToPercent()));
                if (moduleScienceExperiment.rerunnable == false)
                {
                    infoItems.Add(PartInfoItem.Create("\tSingle Usage"));
                }
            }
        }

        private void SetSingleActivationInfo()
        {
            if (selectedPart.HasModule<ModuleAnimateGeneric>(m => m.isOneShot))
            {
                infoItems.Add(PartInfoItem.Create("Single Activation"));
            }
        }

        private void SetSolarPanelInfo()
        {
            moduleDeployableSolarPanel = selectedPart.GetModule<ModuleDeployableSolarPanel>();
            if (moduleDeployableSolarPanel != null)
            {
                infoItems.Add(PartInfoItem.Create("Charge Rate", moduleDeployableSolarPanel.chargeRate.ToRate()));
                if (moduleDeployableSolarPanel.isBreakable)
                {
                    infoItems.Add(PartInfoItem.Create("Breakable"));
                }
                if (moduleDeployableSolarPanel.sunTracking)
                {
                    infoItems.Add(PartInfoItem.Create("Sun Tracking"));
                }
            }
        }

        private void SetTransmitterInfo()
        {
            moduleDataTransmitter = selectedPart.GetModule<ModuleDataTransmitter>();
            if (moduleDataTransmitter != null)
            {
                infoItems.Add(PartInfoItem.Create("Packet Size", moduleDataTransmitter.packetSize.ToString("F2") + " Mits"));
                infoItems.Add(PartInfoItem.Create("Bandwidth", (moduleDataTransmitter.packetInterval * moduleDataTransmitter.packetSize).ToString("F2") + "Mits/sec"));
                infoItems.Add(PartInfoItem.Create(moduleDataTransmitter.requiredResource, moduleDataTransmitter.packetResourceCost.ToString("F2") + "/Packet"));
            }
        }

        private void Window(int windowId)
        {
            try
            {
                GUILayout.Label(selectedPart.partInfo.title, BuildOverlay.TitleStyle);
                if (showInfo)
                {
                    for (int i = 0; i < infoItems.Count; ++i)
                    {
                        partInfoItem = infoItems[i];
                        GUILayout.Space(2.0f);
                        GUILayout.BeginHorizontal();
                        if (partInfoItem.Value != null)
                        {
                            GUILayout.Label(partInfoItem.Name + ":", BuildOverlay.NameStyle);
                            GUILayout.Space(25.0f);
                            GUILayout.Label(partInfoItem.Value, BuildOverlay.ValueStyle);
                        }
                        else
                        {
                            GUILayout.Label(partInfoItem.Name, BuildOverlay.NameStyle);
                        }
                        GUILayout.EndHorizontal();
                    }
                }
                else if (clickToOpen && namesOnly == false)
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
    }
}