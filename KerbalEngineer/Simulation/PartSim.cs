// Name:    Kerbal Engineer Redux
// Author:  CYBUTEK
// License: Attribution-NonCommercial-ShareAlike 3.0 Unported
//
// This class has taken a lot of inspiration from r4m0n's MuMech FuelFlowSimulator.  Although extremely
// similar to the code used within MechJeb, it is a clean re-write.  The similarities are a testiment
// to how well the MuMech code works and the robustness of the simulation algorithem used.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using KerbalEngineer.Extensions;
using UnityEngine;

namespace KerbalEngineer.Simulation
{
    public class PartSim
    {
        ResourceContainer resources = new ResourceContainer();
        ResourceContainer resourceConsumptions = new ResourceContainer();
        ResourceContainer resourceDrains = new ResourceContainer();

        List<PartSim> sourceParts = new List<PartSim>();

        public Part part;
        public int decoupledInStage;
        public double thrust = 0;
        public double actualThrust = 0;
        public double isp = 0;

        public PartSim(Part part, double atmosphere)
        {
            this.part = part;

            foreach (PartResource resource in part.Resources)
            {
                resources.Add(resource.info.id, resource.amount);
            }

            foreach (ModuleEngines engine in part.Modules.OfType<ModuleEngines>())
                SetEngineInfo(engine.requestedThrust, engine.maxThrust, engine.thrustPercentage, engine.atmosphereCurve, atmosphere);

            if (part.HasModule<MultiModeEngine>())
            {
                string mode = part.GetModule<MultiModeEngine>().mode;

                foreach (ModuleEnginesFX engine in part.Modules.OfType<ModuleEnginesFX>())
                {
                    if (engine.engineID == mode)
                        SetEngineInfo(engine.requestedThrust, engine.maxThrust, engine.thrustPercentage, engine.atmosphereCurve, atmosphere);
                }
            }

            decoupledInStage = DecoupledInStage();
        }

        public void SetEngineInfo(float requestedThrust, float maxThrust, float thrustPercentage, FloatCurve atmosphereCurve, double atmosphere)
        {
            float thrustLimiter = (thrustPercentage > 0) ? thrustPercentage / 100f : 0f;

            if (part.vessel != null)
            {
                actualThrust += requestedThrust * thrustLimiter;
                isp = atmosphereCurve.Evaluate((float)part.staticPressureAtm);
            }
            else
            {
                isp = atmosphereCurve.Evaluate((float)atmosphere);
            }

            thrust += maxThrust * thrustLimiter;
        }

        public bool CanDrawNeededResources(List<PartSim> partSims)
        {
            foreach (int type in resourceConsumptions.Types)
            {
                switch (ResourceContainer.GetResourceFlowMode(type))
                {
                    case ResourceFlowMode.NO_FLOW:
                        if (resources[type] < 1f)
                        {
                            return false;
                        }
                        break;

                    case ResourceFlowMode.ALL_VESSEL:
                        foreach (PartSim partSim in partSims)
                        {
                            if (partSim.resources[type] > 1f)
                            {
                                return true;
                            }
                        }
                        return false;

                    case ResourceFlowMode.STACK_PRIORITY_SEARCH:
                        if (!CanSupplyResourceRecursive(type))
                        {
                            return false;
                        }
                        break;

                    default:
                        return false;
                }
            }
            return true;
        }

        public void SetResourceConsumptions()
        {
            foreach (ModuleEngines engine in part.Modules.OfType<ModuleEngines>())
            {
                float thrustLimiter = (engine.thrustPercentage > 0) ? engine.thrustPercentage / 100f : 0f;
                double flowRate = GetPropellantFlowRate(engine.requestedThrust * thrustLimiter, engine.maxThrust * thrustLimiter, engine.throttleLocked);
                double flowMass = GetPropellantFlowMass(engine.propellants);
                SetPropellantConsumptionRate(engine.propellants, flowRate, flowMass);
            }

            if (part.HasModule<MultiModeEngine>())
            {
                string mode = part.GetModule<MultiModeEngine>().mode;

                foreach (ModuleEnginesFX engine in part.Modules.OfType<ModuleEnginesFX>())
                {
                    if (engine.engineID == mode)
                    {
                        float thrustLimiter = (engine.thrustPercentage > 0) ? engine.thrustPercentage / 100f : 0f;
                        double flowRate = GetPropellantFlowRate(engine.requestedThrust * thrustLimiter, engine.maxThrust * thrustLimiter, engine.throttleLocked);
                        double flowMass = GetPropellantFlowMass(engine.propellants);
                        SetPropellantConsumptionRate(engine.propellants, flowRate, flowMass);
                    }
                }
            }
        }

        public double GetPropellantFlowRate(float requestedThrust, float maxThrust, bool throttleLocked)
        {
            if (part.vessel != null)
            {
                if (throttleLocked)
                {
                    return maxThrust / (isp * Simulation.STD_GRAVITY);
                }
                else
                {
                    if (part.vessel.Landed)
                    {
                        return Math.Max(0.000001d, maxThrust * FlightInputHandler.state.mainThrottle) / (isp * Simulation.STD_GRAVITY);
                    }
                    else
                    {
                        if (requestedThrust > 0)
                            return requestedThrust / (isp * Simulation.STD_GRAVITY);
                        else
                            return maxThrust / (isp * Simulation.STD_GRAVITY);
                    }
                }
            }
            else
            {
                return maxThrust / (isp * Simulation.STD_GRAVITY);
            }
        }

        public double GetPropellantFlowMass(List<Propellant> propellants)
        {
            double flowMass = 0d;
            foreach (Propellant propellant in propellants)
            {
                flowMass += propellant.ratio * ResourceContainer.GetResourceDensity(propellant.id);
            }
            return flowMass;
        }

        public void SetPropellantConsumptionRate(List<Propellant> propellants, double flowRate, double flowMass)
        {
            foreach (Propellant propellant in propellants)
            {
                if (propellant.name == "ElectricCharge" || propellant.name == "IntakeAir")
                {
                    continue;
                }

                double consumptionRate = propellant.ratio * flowRate / flowMass;
                resourceConsumptions.Add(propellant.id, consumptionRate);
            }
        }

        public void SetResourceDrainRates(List<PartSim> partSims)
        {
            foreach (int type in resourceConsumptions.Types)
            {
                double amount = resourceConsumptions[type];

                switch (ResourceContainer.GetResourceFlowMode(type))
                {
                    case ResourceFlowMode.NO_FLOW:
                        this.resourceDrains.Add(type, amount);
                        break;

                    case ResourceFlowMode.ALL_VESSEL:
                        SetResourceDrainRateAllVessel(type, amount, partSims);
                        break;

                    case ResourceFlowMode.STACK_PRIORITY_SEARCH:
                        SetResourceDrainRateRecursive(type, amount);
                        break;
                }
            }
        }

        public void SetSourceNodes(Hashtable partSimLookup)
        {
            foreach (PartSim partSim in partSimLookup.Values)
            {
                if (partSim.part is FuelLine && (partSim.part as FuelLine).target == this.part)
                {
                    sourceParts.Add(partSim);
                }
            }

            foreach (AttachNode attachNode in this.part.attachNodes)
            {
                if (attachNode.attachedPart != null && attachNode.nodeType == AttachNode.NodeType.Stack &&
                    (attachNode.attachedPart.fuelCrossFeed || attachNode.attachedPart is FuelTank) &&
                    !(this.part.NoCrossFeedNodeKey.Length > 0 && attachNode.id.Contains(this.part.NoCrossFeedNodeKey)))
                {
                    sourceParts.Add((PartSim)partSimLookup[attachNode.attachedPart]);
                }
            }

            if (this.part.parent != null && this.part.parent.fuelCrossFeed == true && !IsDecoupler(this.part.parent))
            {
                sourceParts.Add((PartSim)partSimLookup[this.part.parent]);
            }
        }

        public void RemoveSourcePart(PartSim part)
        {
            if (sourceParts.Contains(part))
            {
                sourceParts.Remove(part);
            }
        }

        public int DecoupledInStage(Part part = null, int stage = -1)
        {
            if (part == null)
            {
                part = this.part;
            }

            if (IsDecoupler(part))
            {
                if (part.inverseStage > stage)
                {
                    stage = part.inverseStage;
                }
            }

            if (part.parent != null)
            {
                stage = DecoupledInStage(part.parent, stage);
            }

            return stage;
        }

        public void DrainResources(double time)
        {
            foreach (int type in resourceDrains.Types)
            {
                resources.Add(type, -time * resourceDrains[type]);
            }
        }

        public double TimeToDrainResource()
        {
            double time = double.MaxValue;

            foreach (int type in resourceDrains.Types)
            {
                if (resourceDrains[type] > 0)
                {
                    time = Math.Min(time, resources[type] / resourceDrains[type]);
                }
            }

            return time;
        }

        public bool IsDecoupler(Part part = null)
        {

            if (part == null)
            {
                part = this.part;
            }

            return part is Decoupler || part is RadialDecoupler || part.Modules.OfType<ModuleDecouple>().Count() > 0 || part.Modules.OfType<ModuleAnchoredDecoupler>().Count() > 0;
        }

        private void SetResourceDrainRateAllVessel(int type, double amount, List<PartSim> partSims)
        {
            PartSim source = null;

            foreach (PartSim partSim in partSims)
            {
                if (partSim.resources[type] > 1f)
                {
                    if (source == null || partSim.InverseStage > source.InverseStage)
                    {
                        source = partSim;
                    }
                }
            }

            if (source != null)
            {
                source.resourceDrains.Add(type, amount);
            }
        }

        private void SetResourceDrainRateRecursive(int type, double amount, List<PartSim> visited = null)
        {
            if (visited == null)
            {
                visited = new List<PartSim>();
            }

            List<PartSim> newVisited = new List<PartSim>(visited);
            newVisited.Add(this);

            List<PartSim> fuelLines = new List<PartSim>();
            foreach (PartSim partSim in sourceParts)
            {
                if (partSim.part is FuelLine && !visited.Contains(partSim))
                {
                    if (partSim.CanSupplyResourceRecursive(type, newVisited))
                    {
                        fuelLines.Add(partSim);
                    }
                }
            }

            if (fuelLines.Count > 0)
            {
                foreach (PartSim fuelLine in fuelLines)
                {
                    fuelLine.SetResourceDrainRateRecursive(type, amount / fuelLines.Count, newVisited);
                }
                return;
            }

            foreach (PartSim partSim in sourceParts)
            {
                if (!visited.Contains(partSim) && partSim.CanSupplyResourceRecursive(type, newVisited))
                {
                    if (DrainFromSourceBeforeSelf(type, partSim))
                    {
                        partSim.SetResourceDrainRateRecursive(type, amount, newVisited);
                        return;
                    }
                }
            }

            if (this.resources[type] > 0)
            {
                resourceDrains.Add(type, amount);
            }
        }

        private bool CanSupplyResourceRecursive(int type, List<PartSim> visited = null)
        {
            if (visited == null)
            {
                visited = new List<PartSim>();
            }

            if (this.resources[type] > 1f)
            {
                return true;
            }

            List<PartSim> newVisited = new List<PartSim>(visited);
            newVisited.Add(this);

            foreach (PartSim partSim in sourceParts)
            {
                if (!visited.Contains(partSim))
                {
                    if (partSim.CanSupplyResourceRecursive(type, newVisited))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private bool DrainFromSourceBeforeSelf(int type, PartSim source)
        {
            if (resources[type] < 1f)
            {
                return true;
            }

            if (source.part != this.part.parent)
            {
                return true;
            }

            if (this.part.parent == null)
            {
                return true;
            }

            foreach (AttachNode attachNode in this.part.parent.attachNodes)
            {
                if (attachNode.attachedPart == this.part && attachNode.nodeType != AttachNode.NodeType.Stack)
                {
                    return false;
                }
            }

            return true;
        }

        public double GetStartMass(int currentStage)
        {
            double mass = 0d;

            if (part.Modules.Contains("LaunchClamp"))
            {
                return 0d;
            }

            if (part.physicalSignificance == Part.PhysicalSignificance.NONE)
            {
                mass = part.GetResourceMass();
            }
            else
            {
                mass = part.mass + part.GetResourceMass();
            }

            return mass;
        }

        public double GetMass(int currentStage)
        {
            double mass = 0d;

            if (part.Modules.Contains("LaunchClamp"))
            {
                return 0d;
            }

            if (part.physicalSignificance == Part.PhysicalSignificance.FULL)
            {
                mass = part.mass;
            }

            foreach (int type in resources.Types)
            {
                mass += resources.GetResourceMass(type);
            }

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

        public int InverseStage
        {
            get
            {
                return part.inverseStage;
            }
        }

        public bool IsSolidMotor
        {
            get
            {
                foreach (ModuleEngines engine in part.Modules.OfType<ModuleEngines>())
                {
                    if (engine.throttleLocked)
                    {
                        return true;
                    }
                }

                return false;
            }
        }
    }
}