// Kerbal Engineer Redux
// Author:  CYBUTEK
// License: Attribution-NonCommercial-ShareAlike 3.0 Unported
//
// This class has taken a lot of inspiration from r4m0n's MuMech FuelFlowSimulator.  Although extremely
// similar to the code used within MechJeb, it is a clean re-write.  The similarities are a testiment
// to how well the MuMech code works and the robustness of the simulation algorithem used.

using System;
using System.Collections.Generic;
using System.Linq;

using KerbalEngineer.Extensions;

using UnityEngine;

namespace KerbalEngineer.Simulation
{
    public class PartSim
    {
        public ResourceContainer resources = new ResourceContainer();
        public ResourceContainer resourceDrains = new ResourceContainer();
        ResourceContainer resourceFlowStates = new ResourceContainer();
        ResourceContainer resourceConsumptions = new ResourceContainer();

        Dictionary<int, bool> resourceCanSupply = new Dictionary<int, bool>();

        List<AttachNodeSim> attachNodes = new List<AttachNodeSim>();

        public Part part;              // This is only set while the data structures are being initialised
        public int partId = 0;
        public String name;
        public PartSim parent;
        public PartSim fuelLineTarget;
        public bool hasVessel;
        public bool isLanded;
        public int decoupledInStage;
        public int inverseStage;
        public int cost;
        double baseMass = 0d;
        double startMass = 0d;
        public double thrust = 0;
        public double actualThrust = 0;
        public double isp = 0;
        public String noCrossFeedNodeKey;
        public bool fuelCrossFeed;
        public bool isEngine;
        public bool isFuelLine;
        public bool isFuelTank;
        public bool isDecoupler;
        public bool isDockingNode;
        public bool isStrutOrFuelLine;
        public bool isSolidMotor;
        public bool isSepratron;
        public bool hasMultiModeEngine;
        public bool hasModuleEnginesFX;
        public bool hasModuleEngines;

        public PartSim(Part thePart, int id, double atmosphere)
        {
            this.part = thePart;
            this.partId = id;
            this.name = this.part.partInfo.name;
            //MonoBehaviour.print("Create PartSim for " + name);
            
            this.parent = null;
            this.fuelCrossFeed = this.part.fuelCrossFeed;
            this.noCrossFeedNodeKey = this.part.NoCrossFeedNodeKey;
            this.decoupledInStage = this.DecoupledInStage(this.part);
            this.isDecoupler = this.IsDecoupler(this.part);
            this.isDockingNode = this.IsDockingNode();
            this.isFuelLine = this.part is FuelLine;
            this.isFuelTank = this.part is FuelTank;
            this.isStrutOrFuelLine = this.IsStrutOrFuelLine();
            this.isSolidMotor = this.IsSolidMotor();
            this.isSepratron = this.IsSepratron();
            this.inverseStage = this.part.inverseStage;
            //MonoBehaviour.print("inverseStage = " + inverseStage);

            this.cost = this.part.partInfo.cost;

            if (!this.part.Modules.Contains("LaunchClamp") && this.part.physicalSignificance == Part.PhysicalSignificance.FULL)
                this.baseMass = this.part.mass;

            foreach (PartResource resource in this.part.Resources)
            {
                // Make sure it isn't NaN as this messes up the part mass and hence most of the values
                // This can happen if a resource capacity is 0 and tweakable
                if (!Double.IsNaN(resource.amount))
                {
                    //MonoBehaviour.print(resource.resourceName + " = " + resource.amount);
                    this.resources.Add(resource.info.id, resource.amount);
                    this.resourceFlowStates.Add(resource.info.id, resource.flowState ? 1 : 0);
                }
                else
                {
                    MonoBehaviour.print(resource.resourceName + " is NaN. Skipping.");
                }
            }

            this.startMass = this.GetMass();

            this.hasVessel = (this.part.vessel != null);
            this.isLanded = this.hasVessel && this.part.vessel.Landed;

            this.hasMultiModeEngine = this.part.HasModule<MultiModeEngine>();
            this.hasModuleEnginesFX = this.part.HasModule<ModuleEnginesFX>();
            this.hasModuleEngines = this.part.HasModule<ModuleEngines>();

            this.isEngine = this.hasMultiModeEngine || this.hasModuleEnginesFX || this.hasModuleEngines;

            //MonoBehaviour.print("Created " + name + ". Decoupled in stage " + decoupledInStage);
        }

        public void CreateEngineSims(List<EngineSim> allEngines, double atmosphere)
        {
            LogMsg log = new LogMsg();
            log.buf.AppendLine("CreateEngineSims for " + this.name);

            foreach (PartModule partMod in this.part.Modules)
            {
                log.buf.AppendLine("Module: " + partMod.moduleName);
            }

            if (this.hasMultiModeEngine)
            {
                // A multi-mode engine has multiple ModuleEnginesFX but only one is active at any point
                // The mode of the engine is the engineID of the ModuleEnginesFX that is active
                string mode = this.part.GetModule<MultiModeEngine>().mode;

                foreach (ModuleEnginesFX engine in this.part.GetModules<ModuleEnginesFX>())
                {
                    if (engine.engineID == mode)
                    {
                        EngineSim engineSim = new EngineSim(this, atmosphere,
                                                            engine.maxThrust,
                                                            engine.thrustPercentage,
                                                            engine.requestedThrust,
                                                            engine.atmosphereCurve,
                                                            engine.throttleLocked,
                                                            engine.propellants);
                        allEngines.Add(engineSim);
                    }
                }
            }
            else
            {

                if (this.hasModuleEnginesFX)
                {
                    foreach (ModuleEnginesFX engine in this.part.GetModules<ModuleEnginesFX>())
                    {
                        EngineSim engineSim = new EngineSim(this, atmosphere,
                                                            engine.maxThrust,
                                                            engine.thrustPercentage,
                                                            engine.requestedThrust,
                                                            engine.atmosphereCurve,
                                                            engine.throttleLocked,
                                                            engine.propellants);
                        allEngines.Add(engineSim);
                    }
                }

                if (this.hasModuleEngines)
                {
                    foreach (ModuleEngines engine in this.part.GetModules<ModuleEngines>())
                    {
                        EngineSim engineSim = new EngineSim(this, atmosphere,
                                                            engine.maxThrust,
                                                            engine.thrustPercentage,
                                                            engine.requestedThrust,
                                                            engine.atmosphereCurve,
                                                            engine.throttleLocked,
                                                            engine.propellants);
                        allEngines.Add(engineSim);
                    }
                }
            }

            log.Flush();
        }


        public void SetupAttachNodes(Dictionary<Part, PartSim> partSimLookup)
        {
            this.attachNodes.Clear();
            foreach (AttachNode attachNode in this.part.attachNodes)
            {
                if (attachNode.attachedPart != null)
                {
                    PartSim attachedSim;
                    if (partSimLookup.TryGetValue(attachNode.attachedPart, out attachedSim))
                    {
                        this.attachNodes.Add(new AttachNodeSim(attachedSim, attachNode.id, attachNode.nodeType));
                    }
                    else
                    {
                        MonoBehaviour.print("No PartSim for attached part (" + attachNode.attachedPart.partInfo.name + ")");
                    }
                }
            }

            if (this.isFuelLine)
            {
                if ((this.part as FuelLine).target != null)
                {
                    PartSim targetSim;
                    if (partSimLookup.TryGetValue((this.part as FuelLine).target, out targetSim))
                    {
                        this.fuelLineTarget = targetSim;
                    }
                }
                else
                {
                    this.fuelLineTarget = null;
                }
            }

            if (this.part.parent != null)
            {
                this.parent = null;
                if (!partSimLookup.TryGetValue(this.part.parent, out this.parent))
                {
                    MonoBehaviour.print("No PartSim for parent part (" + this.part.parent.partInfo.name + ")");
                }
            }
        }

        public int DecoupledInStage(Part thePart, int stage = -1)
        {
            if (this.IsDecoupler(thePart))
            {
                if (thePart.inverseStage > stage)
                {
                    stage = thePart.inverseStage;
                }
            }

            if (thePart.parent != null)
            {
                stage = this.DecoupledInStage(thePart.parent, stage);
            }

            return stage;
        }

        private bool IsDecoupler(Part thePart)
        {
            return thePart is Decoupler || thePart is RadialDecoupler || thePart.Modules.OfType<ModuleDecouple>().Count() > 0 || thePart.Modules.OfType<ModuleAnchoredDecoupler>().Count() > 0;
        }

        private bool IsDockingNode()
        {
            return this.part.Modules.OfType<ModuleDockingNode>().Count() > 0;
        }

        private bool IsStrutOrFuelLine()
        {
            return (this.part is StrutConnector || this.part is FuelLine) ? true : false;
        }

        private bool IsSolidMotor()
        {
            foreach (ModuleEngines engine in this.part.Modules.OfType<ModuleEngines>())
            {
                if (engine.throttleLocked)
                    return true;
            }

            return false;
        }

        private bool IsSepratron()
        {
            if (!this.part.ActivatesEvenIfDisconnected)
                return false;

            if (this.part is SolidRocket)
                return true;

            if (this.part.Modules.OfType<ModuleEngines>().Count() == 0)
                return false;

            if (this.part.Modules.OfType<ModuleEngines>().First().throttleLocked == true)
                return true;

            return false;
        }

        public void ReleasePart()
        {
            this.part = null;
        }


        // All functions below this point must not rely on the part member (it may be null)
        //

        public HashSet<PartSim> GetSourceSet(int type, List<PartSim> allParts, List<PartSim> allFuelLines, HashSet<PartSim> visited)
        {
            //MonoBehaviour.print("GetSourceSet(" + ResourceContainer.GetResourceName(type) + ") for " + name + ":" + partId);

            HashSet<PartSim> allSources = new HashSet<PartSim>();
            HashSet<PartSim> partSources = new HashSet<PartSim>();

            // Rule 1: Each part can be only visited once, If it is visited for second time in particular search it returns empty list.
            if (visited.Contains(this))
            {
                //MonoBehaviour.print("Returning empty set, already visited (" + name + ":" + partId + ")");
                return allSources;
            }

            //MonoBehaviour.print("Adding this to visited");
            visited.Add(this);

            // Rule 2: Part performs scan on start of every fuel pipe ending in it. This scan is done in order in which pipes were installed. Then it makes an union of fuel tank sets each pipe scan returned. If the resulting list is not empty, it is returned as result.
            //MonoBehaviour.print("foreach fuel line");
            foreach (PartSim partSim in allFuelLines)
            {
                if (partSim.fuelLineTarget == this)
                {
                    //MonoBehaviour.print("Adding fuel line as source (" + partSim.name + ":" + partSim.partId + ")");
                    partSources = partSim.GetSourceSet(type, allParts, allFuelLines, visited);
                    if (partSources.Count > 0)
                    {
                        allSources.UnionWith(partSources);
                        partSources.Clear();
                    }
                }
            }

            if (allSources.Count > 0)
            {
                //MonoBehaviour.print("Returning " + allSources.Count + " fuel line sources (" + name + ":" + partId + ")");
                return allSources;
            }

            // Rule 3: If the part is not crossfeed capable, it returns empty list.
            //MonoBehaviour.print("Test crossfeed");
            if (!this.fuelCrossFeed)
            {
                //MonoBehaviour.print("Returning empty set, no cross feed (" + name + ":" + partId + ")");
                return allSources;
            }

            // Rule 4: Part performs scan on each of its axially mounted neighbors. 
            //  Couplers (bicoupler, tricoupler, ...) are an exception, they only scan one attach point on the single attachment side, skip the points on the side where multiple points are. [Experiment]
            //  Again, the part creates union of scan lists from each of its neighbor and if it is not empty, returns this list. 
            //  The order in which mount points of a part are scanned appears to be fixed and defined by the part specification file. [Experiment]
            //MonoBehaviour.print("foreach attach node");
            foreach (AttachNodeSim attachSim in this.attachNodes)
            {
                if (attachSim.attachedPartSim != null)
                {
                    if (attachSim.nodeType == AttachNode.NodeType.Stack &&
                        (attachSim.attachedPartSim.fuelCrossFeed || attachSim.attachedPartSim.isFuelTank) &&
                        !(this.noCrossFeedNodeKey != null && this.noCrossFeedNodeKey.Length > 0 && attachSim.id.Contains(this.noCrossFeedNodeKey)))
                    {
                        //MonoBehaviour.print("Adding attached part as source (" + attachSim.attachedPartSim.name + ":" + attachSim.attachedPartSim.partId + ")");
                        partSources = attachSim.attachedPartSim.GetSourceSet(type, allParts, allFuelLines, visited);
                        if (partSources.Count > 0)
                        {
                            allSources.UnionWith(partSources);
                            partSources.Clear();
                        }
                    }
                }
            }

            if (allSources.Count > 0)
            {
                //MonoBehaviour.print("Returning " + allSources.Count + " attached sources (" + name + ":" + partId + ")");
                return allSources;
            }

            // Rule 5: If the part is fuel container for searched type of fuel (i.e. it has capability to contain that type of fuel and the fuel type was not disabled [Experiment]) and it contains fuel, it returns itself.
            // Rule 6: If the part is fuel container for searched type of fuel (i.e. it has capability to contain that type of fuel and the fuel type was not disabled) but it does not contain the requested fuel, it returns empty list. [Experiment]
            //MonoBehaviour.print("testing enabled container");
            if (this.resources.HasType(type) && this.resourceFlowStates[type] != 0)
            {
                if (this.resources[type] > 1f)
                    allSources.Add(this);

                //MonoBehaviour.print("Returning this as only source (" + name + ":" + partId + ")");
                return allSources;
            }

            // Rule 7: If the part is radially attached to another part and it is child of that part in the ship's tree structure, it scans its parent and returns whatever the parent scan returned. [Experiment] [Experiment]
            if (this.parent != null)
            {
                allSources = this.parent.GetSourceSet(type, allParts, allFuelLines, visited);
                if (allSources.Count > 0)
                {
                    //MonoBehaviour.print("Returning " + allSources.Count + " parent sources (" + name + ":" + partId + ")");
                    return allSources;
                }
            }

            // Rule 8: If all preceding rules failed, part returns empty list.
            //MonoBehaviour.print("Returning empty set, no sources found (" + name + ":" + partId + ")");
            return allSources;
        }


        public void RemoveAttachedParts(HashSet<PartSim> partSims)
        {
            // Loop through the attached parts
            foreach (AttachNodeSim attachSim in this.attachNodes)
            {
                // If the part is in the set then "remove" it by clearing the PartSim reference
                if (partSims.Contains(attachSim.attachedPartSim))
                    attachSim.attachedPartSim = null;
            }
        }


        public void DrainResources(double time)
        {
            //MonoBehaviour.print("DrainResources(" + name + ":" + partId + ", " + time + ")");
            foreach (int type in this.resourceDrains.Types)
            {
                //MonoBehaviour.print("draining " + (time * resourceDrains[type]) + " " + ResourceContainer.GetResourceName(type));
                this.resources.Add(type, -time * this.resourceDrains[type]);
                //MonoBehaviour.print(ResourceContainer.GetResourceName(type) + " left = " + resources[type]);
            }
        }

        public double TimeToDrainResource()
        {
            //MonoBehaviour.print("TimeToDrainResource(" + name + ":" + partId + ")");
            double time = double.MaxValue;

            foreach (int type in this.resourceDrains.Types)
            {
                //MonoBehaviour.print("type = " + ResourceContainer.GetResourceName(type) + "  amount = " + resources[type] + "  rate = " + resourceDrains[type]);
                if (this.resourceDrains[type] > 0)
                    time = Math.Min(time, this.resources[type] / this.resourceDrains[type]);
            }

            //if (time < double.MaxValue)
            //    MonoBehaviour.print("TimeToDrainResource(" + name + ":" + partId + ") = " + time);
            return time;
        }

        public double GetStartMass()
        {
            return this.startMass;
        }

        public double GetMass()
        {
            double mass = this.baseMass;

            foreach (int type in this.resources.Types)
                mass += this.resources.GetResourceMass(type);

            return mass;
        }

        public ResourceContainer Resources
        {
            get
            {
                return this.resources;
            }
        }

        public ResourceContainer ResourceConsumptions
        {
            get
            {
                return this.resourceConsumptions;
            }
        }

        public ResourceContainer ResourceDrains
        {
            get
            {
                return this.resourceDrains;
            }
        }

#if LOG
        public String DumpPartAndParentsToBuffer(StringBuilder buffer, String prefix)
        {
            if (parent != null)
            {
                prefix = parent.DumpPartAndParentsToBuffer(buffer, prefix) + " ";
            }

            DumpPartToBuffer(buffer, prefix);

            return prefix;
        }

        public void DumpPartToBuffer(StringBuilder buffer, String prefix, List<PartSim> allParts = null)
        {
            buffer.Append(prefix);
            buffer.Append(name);
            buffer.AppendFormat(":[id = {0:d}, decouple = {1:d}, invstage = {2:d}", partId, decoupledInStage, inverseStage);

            buffer.AppendFormat(", isSep = {0}", isSepratron);

            foreach (int type in resources.Types)
                buffer.AppendFormat(", {0} = {1:g6}", ResourceContainer.GetResourceName(type), resources[type]);

            if (attachNodes.Count > 0)
            {
                buffer.Append(", attached = <");
                attachNodes[0].DumpToBuffer(buffer);
                for (int i = 1; i < attachNodes.Count; i++)
                {
                    buffer.Append(", ");
                    attachNodes[i].DumpToBuffer(buffer);
                }
                buffer.Append(">");
            }

            // Add more info here

            buffer.Append("]\n");

            if (allParts != null)
            {
                String newPrefix = prefix + " ";
                foreach (PartSim partSim in allParts)
                {
                    if (partSim.parent == this)
                        partSim.DumpPartToBuffer(buffer, newPrefix, allParts);
                }
            }
        }
#endif
    }
}
