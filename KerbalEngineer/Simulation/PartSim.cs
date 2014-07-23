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
using System.Text;

using KerbalEngineer.Extensions;

using UnityEngine;

namespace KerbalEngineer.Simulation
{
    public class PartSim
    {
        public ResourceContainer resources = new ResourceContainer();
        public ResourceContainer resourceDrains = new ResourceContainer();
        ResourceContainer resourceFlowStates = new ResourceContainer();
        //ResourceContainer resourceConsumptions = new ResourceContainer();

        //Dictionary<int, bool> resourceCanSupply = new Dictionary<int, bool>();

        List<AttachNodeSim> attachNodes = new List<AttachNodeSim>();

        public Part part;              // This is only set while the data structures are being initialised
        public int partId = 0;
        public String name;
        public PartSim parent;
        public PartSim fuelLineTarget;
        public bool hasVessel;
        public bool isLanded;
        public bool isDecoupler;
        public int decoupledInStage;
        public int inverseStage;
        public int cost;
        double baseMass = 0d;
        double startMass = 0d;
        public String noCrossFeedNodeKey;
        public bool fuelCrossFeed;
        public bool isEngine;
        public bool isFuelLine;
        public bool isFuelTank;
        public bool isSepratron;
        public bool hasMultiModeEngine;
        public bool hasModuleEnginesFX;
        public bool hasModuleEngines;
        public bool isNoPhysics;
        public bool localCorrectThrust;

        public PartSim(Part thePart, int id, double atmosphere)
        {
            this.part = thePart;
            this.partId = id;
            this.name = this.part.partInfo.name;
#if LOG
            MonoBehaviour.print("Create PartSim for " + name);
#endif
            this.parent = null;
            this.fuelCrossFeed = this.part.fuelCrossFeed;
            this.noCrossFeedNodeKey = this.part.NoCrossFeedNodeKey;
            this.decoupledInStage = this.DecoupledInStage(this.part);
            this.isFuelLine = this.part is FuelLine;
            this.isFuelTank = this.part is FuelTank;
            this.isSepratron = this.IsSepratron();
            this.inverseStage = this.part.inverseStage;
            //MonoBehaviour.print("inverseStage = " + inverseStage);

            //this.cost = this.part.partInfo.cost;

            // Work out if the part should have no physical significance
            this.isNoPhysics = this.part.HasModule<ModuleLandingGear>() ||
                            this.part.HasModule<LaunchClamp>() ||
                            this.part.physicalSignificance == Part.PhysicalSignificance.NONE ||
                            this.part.PhysicsSignificance == 1;

            if (!this.isNoPhysics)
                this.baseMass = this.part.mass;
#if LOG
            MonoBehaviour.print((isNoPhysics ? "Ignoring" : "Using") + " part.mass of " + part.mass);
#endif
            foreach (PartResource resource in this.part.Resources)
            {
                // Make sure it isn't NaN as this messes up the part mass and hence most of the values
                // This can happen if a resource capacity is 0 and tweakable
                if (!Double.IsNaN(resource.amount))
                {
#if LOG
                    MonoBehaviour.print(resource.resourceName + " = " + resource.amount);
#endif
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
#if LOG
            MonoBehaviour.print("Created " + name + ". Decoupled in stage " + decoupledInStage);
#endif
        }

        public void CreateEngineSims(List<EngineSim> allEngines, double atmosphere)
        {
            bool correctThrust = SimManager.DoesEngineUseCorrectedThrust(this.part);
            //MonoBehaviour.print("Engine " + name + " correctThrust = " + correctThrust);
#if LOG
            LogMsg log = new LogMsg();
            log.buf.AppendLine("CreateEngineSims for " + name);

            foreach (PartModule partMod in part.Modules)
            {
                log.buf.AppendLine("Module: " + partMod.moduleName);
            }

            log.buf.AppendLine("correctThrust = " + correctThrust);
#endif

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
                                                            engine.realIsp,
                                                            engine.atmosphereCurve,
                                                            engine.throttleLocked,
                                                            engine.propellants,
                                                            correctThrust);
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
                                                            engine.realIsp,
                                                            engine.atmosphereCurve,
                                                            engine.throttleLocked,
                                                            engine.propellants,
                                                            correctThrust);
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
                                                            engine.realIsp,
                                                            engine.atmosphereCurve,
                                                            engine.throttleLocked,
                                                            engine.propellants,
                                                            correctThrust);
                        allEngines.Add(engineSim);
                    }
                }
            }
#if LOG
            log.Flush();
#endif
        }


        public void SetupAttachNodes(Dictionary<Part, PartSim> partSimLookup)
        {
#if LOG
            LogMsg log = new LogMsg();
            log.buf.AppendLine("SetupAttachNodes for " + name + ":" + partId + "");
#endif
            this.attachNodes.Clear();
            foreach (AttachNode attachNode in this.part.attachNodes)
            {
#if LOG
                log.buf.AppendLine("AttachNode " + attachNode.id + " = " + (attachNode.attachedPart != null ? attachNode.attachedPart.partInfo.name : "null"));
#endif
                if (attachNode.attachedPart != null && attachNode.id != "Strut")
                {
                    PartSim attachedSim;
                    if (partSimLookup.TryGetValue(attachNode.attachedPart, out attachedSim))
                    {
#if LOG
                        log.buf.AppendLine("Adding attached node " + attachedSim.name + ":" + attachedSim.partId + "");
#endif
                        this.attachNodes.Add(new AttachNodeSim(attachedSim, attachNode.id, attachNode.nodeType));
                    }
                    else
                    {
#if LOG
                        log.buf.AppendLine("No PartSim for attached part (" + attachNode.attachedPart.partInfo.name + ")");
#endif
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
#if LOG
                        log.buf.AppendLine("Fuel line target is " + targetSim.name + ":" + targetSim.partId);
#endif
                        this.fuelLineTarget = targetSim;
                    }
                    else
                    {
#if LOG
                        log.buf.AppendLine("No PartSim for fuel line target (" + part.partInfo.name + ")");
#endif
                        this.fuelLineTarget = null;
                    }

                }
                else
                {
#if LOG
                    log.buf.AppendLine("Fuel line target is null");
#endif
                    this.fuelLineTarget = null;
                }
            }

            if (this.part.parent != null)
            {
                this.parent = null;
                if (partSimLookup.TryGetValue(this.part.parent, out this.parent))
                {
#if LOG
                    log.buf.AppendLine("Parent part is " + parent.name + ":" + parent.partId);
#endif
                }
                else
                {
#if LOG
                    log.buf.AppendLine("No PartSim for parent part (" + part.parent.partInfo.name + ")");
#endif
                }
            }
#if LOG
            log.Flush();
#endif
        }

        private int DecoupledInStage(Part thePart, int stage = -1)
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
            return thePart.HasModule<ModuleDecouple>() ||
                    thePart.HasModule<ModuleAnchoredDecoupler>();
        }

        private bool IsActiveDecoupler(Part thePart)
        {
            return thePart.FindModulesImplementing<ModuleDecouple>().Any(mod => !mod.isDecoupled) ||
                    thePart.FindModulesImplementing<ModuleAnchoredDecoupler>().Any(mod => !mod.isDecoupled);
        }

        private bool IsSepratron()
        {
            if (!this.part.ActivatesEvenIfDisconnected)
                return false;

            if (this.part is SolidRocket)
                return true;

            var modList = this.part.Modules.OfType<ModuleEngines>();
            if (modList.Count() == 0)
                return false;

            if (modList.First().throttleLocked == true)
                return true;

            return false;
        }

        public void ReleasePart()
        {
            this.part = null;
        }


        // All functions below this point must not rely on the part member (it may be null)
        //

        public HashSet<PartSim> GetSourceSet(int type, List<PartSim> allParts, List<PartSim> allFuelLines, HashSet<PartSim> visited, LogMsg log, String indent)
        {
#if LOG
            log.buf.AppendLine(indent + "GetSourceSet(" + ResourceContainer.GetResourceName(type) + ") for " + name + ":" + partId);
            indent += "  ";
#endif
            HashSet<PartSim> allSources = new HashSet<PartSim>();
            HashSet<PartSim> partSources = null;

            // Rule 1: Each part can be only visited once, If it is visited for second time in particular search it returns empty list.
            if (visited.Contains(this))
            {
#if LOG
                log.buf.AppendLine(indent + "Returning empty set, already visited (" + name + ":" + partId + ")");
#endif
                return allSources;
            }

#if LOG
            log.buf.AppendLine("Adding this to visited");
#endif
            visited.Add(this);

            // Rule 2: Part performs scan on start of every fuel pipe ending in it. This scan is done in order in which pipes were installed. Then it makes an union of fuel tank sets each pipe scan returned. If the resulting list is not empty, it is returned as result.
            //MonoBehaviour.print("foreach fuel line");
            
            foreach (PartSim partSim in allFuelLines)
            {
                if (partSim.fuelLineTarget == this)
                {
#if LOG
                    log.buf.AppendLine(indent + "Adding fuel line as source (" + partSim.name + ":" + partSim.partId + ")");
#endif
                    partSources = partSim.GetSourceSet(type, allParts, allFuelLines, visited, log, indent);
                    if (partSources.Count > 0)
                    {
                        allSources.UnionWith(partSources);
                        partSources.Clear();
                    }
                }
            }

            if (allSources.Count > 0)
            {
#if LOG
                log.buf.AppendLine(indent + "Returning " + allSources.Count + " fuel line sources (" + name + ":" + partId + ")");
#endif
                return allSources;
            }

            // Rule 3: If the part is not crossfeed capable, it returns empty list.
            //MonoBehaviour.print("Test crossfeed");
            if (!this.fuelCrossFeed)
            {
#if LOG
                log.buf.AppendLine(indent + "Returning empty set, no cross feed (" + name + ":" + partId + ")");
#endif
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
#if LOG
                        log.buf.AppendLine(indent + "Adding attached part as source (" + attachSim.attachedPartSim.name + ":" + attachSim.attachedPartSim.partId + ")");
#endif
                        partSources = attachSim.attachedPartSim.GetSourceSet(type, allParts, allFuelLines, visited, log, indent);
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
#if LOG
                log.buf.AppendLine(indent + "Returning " + allSources.Count + " attached sources (" + name + ":" + partId + ")");
#endif
                return allSources;
            }

            // Rule 5: If the part is fuel container for searched type of fuel (i.e. it has capability to contain that type of fuel and the fuel type was not disabled [Experiment]) and it contains fuel, it returns itself.
            // Rule 6: If the part is fuel container for searched type of fuel (i.e. it has capability to contain that type of fuel and the fuel type was not disabled) but it does not contain the requested fuel, it returns empty list. [Experiment]
            if (this.resources.HasType(type) && this.resourceFlowStates[type] != 0)
            {
                if (this.resources[type] > SimManager.RESOURCE_MIN)
                {
                    allSources.Add(this);
#if LOG
                    log.buf.AppendLine(indent + "Returning enabled tank as only source (" + name + ":" + partId + ")");
#endif
                }
                else
                {
#if LOG
                    log.buf.AppendLine(indent + "Returning empty set, enabled tank is empty (" + name + ":" + partId + ")");
#endif
                }

                return allSources;
            }

            // Rule 7: If the part is radially attached to another part and it is child of that part in the ship's tree structure, it scans its parent and returns whatever the parent scan returned. [Experiment] [Experiment]
            if (this.parent != null)
            {
                allSources = this.parent.GetSourceSet(type, allParts, allFuelLines, visited, log, indent);
                if (allSources.Count > 0)
                {
#if LOG
                    log.buf.AppendLine(indent + "Returning " + allSources.Count + " parent sources (" + name + ":" + partId + ")");
#endif
                    return allSources;
                }
            }

            // Rule 8: If all preceding rules failed, part returns empty list.
#if LOG
            log.buf.AppendLine(indent + "Returning empty set, no sources found (" + name + ":" + partId + ")");
#endif
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
                if (this.resourceDrains[type] > 0)
                {
                    time = Math.Min(time, this.resources[type] / this.resourceDrains[type]);
                    //MonoBehaviour.print("type = " + ResourceContainer.GetResourceName(type) + "  amount = " + resources[type] + "  rate = " + resourceDrains[type] + "  time = " + time);
                }
            }

            //if (time < double.MaxValue)
            //    MonoBehaviour.print("TimeToDrainResource(" + name + ":" + partId + ") = " + time);
            return time;
        }

        public int DecouplerCount()
        {
            int count = 0;
            PartSim partSim = this;
            while (partSim != null)
            {
                if (partSim.isDecoupler)
                    count++;

                partSim = partSim.parent;
            }
            return count;
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
#if false
        public ResourceContainer ResourceConsumptions
        {
            get
            {
                return resourceConsumptions;
            }
        }
#endif
        public ResourceContainer ResourceDrains
        {
            get
            {
                return this.resourceDrains;
            }
        }

#if LOG || true
        public String DumpPartAndParentsToBuffer(StringBuilder buffer, String prefix)
        {
            if (this.parent != null)
            {
                prefix = this.parent.DumpPartAndParentsToBuffer(buffer, prefix) + " ";
            }

            this.DumpPartToBuffer(buffer, prefix);

            return prefix;
        }

        public void DumpPartToBuffer(StringBuilder buffer, String prefix, List<PartSim> allParts = null)
        {
            buffer.Append(prefix);
            buffer.Append(this.name);
            buffer.AppendFormat(":[id = {0:d}, decouple = {1:d}, invstage = {2:d}", this.partId, this.decoupledInStage, this.inverseStage);

            buffer.AppendFormat(", fuelCF = {0}", this.fuelCrossFeed);
            buffer.AppendFormat(", noCFNKey = '{0}'", this.noCrossFeedNodeKey);

            if (this.isFuelLine)
                buffer.AppendFormat(", fuelLineTarget = {0:d}", this.fuelLineTarget == null ? -1 : this.fuelLineTarget.partId);
            
            buffer.AppendFormat(", isSep = {0}", this.isSepratron);

            foreach (int type in this.resources.Types)
                buffer.AppendFormat(", {0} = {1:g6}", ResourceContainer.GetResourceName(type), this.resources[type]);

            if (this.attachNodes.Count > 0)
            {
                buffer.Append(", attached = <");
                this.attachNodes[0].DumpToBuffer(buffer);
                for (int i = 1; i < this.attachNodes.Count; i++)
                {
                    buffer.Append(", ");
                    this.attachNodes[i].DumpToBuffer(buffer);
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
