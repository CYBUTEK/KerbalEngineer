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

namespace KerbalEngineer.VesselSimulator
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Editor;
    using Helpers;
    using UnityEngine;

    public class EngineSim
    {
        private static readonly Pool<EngineSim> pool = new Pool<EngineSim>(Create, Reset);

        private readonly ResourceContainer resourceConsumptions = new ResourceContainer();
        private readonly ResourceContainer resourceFlowModes = new ResourceContainer();

        public double actualThrust = 0;
        public bool isActive = false;
        public double isp = 0;
        public PartSim partSim;
        public List<AppliedForce> appliedForces = new List<AppliedForce>();
        public float maxMach;
        public bool isFlamedOut;

        public double thrust = 0;

        // Add thrust vector to account for directional losses
        public Vector3 thrustVec;

        private static EngineSim Create()
        {
            return new EngineSim();
        }

        private static void Reset(EngineSim engineSim)
        {
            engineSim.resourceConsumptions.Reset();
            engineSim.resourceFlowModes.Reset();
            engineSim.partSim = null;
            engineSim.actualThrust = 0;
            engineSim.isActive = false;
            engineSim.isp = 0;
            for (int i = 0; i < engineSim.appliedForces.Count; i++)
            {
                engineSim.appliedForces[i].Release();
            }
            engineSim.appliedForces.Clear();
            engineSim.thrust = 0;
            engineSim.maxMach = 0f;
            engineSim.isFlamedOut = false;
        }

        public void Release()
        {
            pool.Release(this);
        }

        public static EngineSim New(PartSim theEngine,
									ModuleEngines engineMod,
									double atmosphere,
									float machNumber,
									bool vectoredThrust,
									bool fullThrust,
									LogMsg log)
        {
            float maxFuelFlow = engineMod.maxFuelFlow;
            float minFuelFlow = engineMod.minFuelFlow;
            float thrustPercentage = engineMod.thrustPercentage;
            List<Transform> thrustTransforms = engineMod.thrustTransforms;
            List<float> thrustTransformMultipliers = engineMod.thrustTransformMultipliers;
            Vector3 vecThrust = CalculateThrustVector(vectoredThrust ? thrustTransforms : null,
                                                        vectoredThrust ? thrustTransformMultipliers : null,
                                                        log);
            FloatCurve atmosphereCurve = engineMod.atmosphereCurve;
            bool atmChangeFlow = engineMod.atmChangeFlow;
            FloatCurve atmCurve = engineMod.useAtmCurve ? engineMod.atmCurve : null;
            FloatCurve velCurve = engineMod.useVelCurve ? engineMod.velCurve : null;
            float currentThrottle = engineMod.currentThrottle;
            float IspG = engineMod.g;
            bool throttleLocked = engineMod.throttleLocked || fullThrust;
            List<Propellant> propellants = engineMod.propellants;
            bool active = engineMod.isOperational;
            float resultingThrust = engineMod.resultingThrust;
            bool isFlamedOut = engineMod.flameout;
			
			EngineSim engineSim = pool.Borrow();

            engineSim.isp = 0.0;
            engineSim.maxMach = 0.0f;
            engineSim.actualThrust = 0.0;
            engineSim.partSim = theEngine;
            engineSim.isActive = active;
            engineSim.thrustVec = vecThrust;
            engineSim.isFlamedOut = isFlamedOut;
            engineSim.resourceConsumptions.Reset();
            engineSim.resourceFlowModes.Reset();
            engineSim.appliedForces.Clear();

            double flowRate = 0.0;
            if (engineSim.partSim.hasVessel)
            {
                if (log != null) log.buf.AppendLine("hasVessel is true"); 

                float flowModifier = GetFlowModifier(atmChangeFlow, atmCurve, engineSim.partSim.part.atmDensity, velCurve, machNumber, ref engineSim.maxMach);
                engineSim.isp = atmosphereCurve.Evaluate((float)atmosphere);
                engineSim.thrust = GetThrust(Mathf.Lerp(minFuelFlow, maxFuelFlow, GetThrustPercent(thrustPercentage)) * flowModifier, engineSim.isp);
                engineSim.actualThrust = engineSim.isActive ? resultingThrust : 0.0;
                if (log != null)
                {
                    log.buf.AppendFormat("flowMod = {0:g6}\n", flowModifier);
                    log.buf.AppendFormat("isp     = {0:g6}\n", engineSim.isp);
                    log.buf.AppendFormat("thrust  = {0:g6}\n", engineSim.thrust);
                    log.buf.AppendFormat("actual  = {0:g6}\n", engineSim.actualThrust);
                }

				if (throttleLocked)
                {
                    if (log != null) log.buf.AppendLine("throttleLocked is true, using thrust for flowRate");
                    flowRate = GetFlowRate(engineSim.thrust, engineSim.isp);
                }
                else
                {
                    if (currentThrottle > 0.0f && engineSim.partSim.isLanded == false)
                    {
						// TODO: This bit doesn't work for RF engines
						if (log != null) log.buf.AppendLine("throttled up and not landed, using actualThrust for flowRate");
                        flowRate = GetFlowRate(engineSim.actualThrust, engineSim.isp);
                    }
                    else
                    {
                        if (log != null) log.buf.AppendLine("throttled down or landed, using thrust for flowRate");
                        flowRate = GetFlowRate(engineSim.thrust, engineSim.isp);
                    }
                }
            }
            else
            {
                if (log != null) log.buf.AppendLine("hasVessel is false");
                float flowModifier = GetFlowModifier(atmChangeFlow, atmCurve, CelestialBodies.SelectedBody.GetDensity(BuildAdvanced.Altitude), velCurve, machNumber, ref engineSim.maxMach);
                engineSim.isp = atmosphereCurve.Evaluate((float)atmosphere);
                engineSim.thrust = GetThrust(Mathf.Lerp(minFuelFlow, maxFuelFlow, GetThrustPercent(thrustPercentage)) * flowModifier, engineSim.isp);
                engineSim.actualThrust = 0d;
                if (log != null)
                {
                    log.buf.AppendFormat("flowMod = {0:g6}\n", flowModifier);
                    log.buf.AppendFormat("isp     = {0:g6}\n", engineSim.isp);
                    log.buf.AppendFormat("thrust  = {0:g6}\n", engineSim.thrust);
                    log.buf.AppendFormat("actual  = {0:g6}\n", engineSim.actualThrust);
                }

                if (log != null) log.buf.AppendLine("no vessel, using thrust for flowRate");
                flowRate = GetFlowRate(engineSim.thrust, engineSim.isp);
            }

            if (log != null) log.buf.AppendFormat("flowRate = {0:g6}\n", flowRate);

            float flowMass = 0f;
            for (int i = 0; i < propellants.Count; ++i)
            {
                Propellant propellant = propellants[i];
                if (!propellant.ignoreForIsp)
                    flowMass += propellant.ratio * ResourceContainer.GetResourceDensity(propellant.id);
            }

            if (log != null) log.buf.AppendFormat("flowMass = {0:g6}\n", flowMass);

            for (int i = 0; i < propellants.Count; ++i)
            {
                Propellant propellant = propellants[i];

                if (propellant.name == "ElectricCharge" || propellant.name == "IntakeAir")
                {
                    continue;
                }

                double consumptionRate = propellant.ratio * flowRate / flowMass;
                if (log != null) log.buf.AppendFormat(
                        "Add consumption({0}, {1}:{2:d}) = {3:g6}\n",
                        ResourceContainer.GetResourceName(propellant.id),
                        theEngine.name,
                        theEngine.partId,
                        consumptionRate);
                engineSim.resourceConsumptions.Add(propellant.id, consumptionRate);
                engineSim.resourceFlowModes.Add(propellant.id, (double)propellant.GetFlowMode());
            }

            for (int i = 0; i < thrustTransforms.Count; i++)
            {
                Transform thrustTransform = thrustTransforms[i];
                Vector3d direction = thrustTransform.forward.normalized;
                Vector3d position = thrustTransform.position;

                AppliedForce appliedForce = AppliedForce.New(direction * engineSim.thrust * thrustTransformMultipliers[i], position);
                engineSim.appliedForces.Add(appliedForce);
            }

            return engineSim;
        }

		private static Vector3 CalculateThrustVector(List<Transform> thrustTransforms, List<float> thrustTransformMultipliers, LogMsg log)
		{
			if (thrustTransforms == null)
			{
				return Vector3.forward;
			}

			Vector3 thrustvec = Vector3.zero;
			for (int i = 0; i < thrustTransforms.Count; ++i)
			{
				Transform trans = thrustTransforms[i];

				if (log != null) log.buf.AppendFormat("Transform = ({0:g6}, {1:g6}, {2:g6})   length = {3:g6}\n", trans.forward.x, trans.forward.y, trans.forward.z, trans.forward.magnitude);

				thrustvec -= (trans.forward * thrustTransformMultipliers[i]);
			}

			if (log != null) log.buf.AppendFormat("ThrustVec  = ({0:g6}, {1:g6}, {2:g6})   length = {3:g6}\n", thrustvec.x, thrustvec.y, thrustvec.z, thrustvec.magnitude);

			thrustvec.Normalize();

			if (log != null) log.buf.AppendFormat("ThrustVecN = ({0:g6}, {1:g6}, {2:g6})   length = {3:g6}\n", thrustvec.x, thrustvec.y, thrustvec.z, thrustvec.magnitude);

			return thrustvec;
		}

        public ResourceContainer ResourceConsumptions
        {
            get
            {
                return resourceConsumptions;
            }
        }

        public static double GetExhaustVelocity(double isp)
        {
            return isp * Units.GRAVITY;
        }

        public static float GetFlowModifier(bool atmChangeFlow, FloatCurve atmCurve, double atmDensity, FloatCurve velCurve, float machNumber, ref float maxMach)
        {
            float flowModifier = 1.0f;
            if (atmChangeFlow)
            {
                flowModifier = (float)(atmDensity / 1.225);
                if (atmCurve != null)
                {
                    flowModifier = atmCurve.Evaluate(flowModifier);
                }
            }
            if (velCurve != null)
            {
                flowModifier = flowModifier * velCurve.Evaluate(machNumber);
                maxMach = velCurve.maxTime;
            }
            if (flowModifier < float.Epsilon)
            {
                flowModifier = float.Epsilon;
            }
            return flowModifier;
        }

        public static double GetFlowRate(double thrust, double isp)
        {
            return thrust / GetExhaustVelocity(isp);
        }

        public static float GetThrottlePercent(float currentThrottle, float thrustPercentage)
        {
            return currentThrottle * GetThrustPercent(thrustPercentage);
        }

        public static double GetThrust(double flowRate, double isp)
        {
            return flowRate * GetExhaustVelocity(isp);
        }

        public static float GetThrustPercent(float thrustPercentage)
        {
            return thrustPercentage * 0.01f;
        }

        public void DumpEngineToBuffer(StringBuilder buffer, String prefix)
        {
            buffer.Append(prefix);
            buffer.AppendFormat("[thrust = {0:g6}, actual = {1:g6}, isp = {2:g6}\n", thrust, actualThrust, isp);
        }

        // A dictionary to hold a set of parts for each resource
        Dictionary<int, HashSet<PartSim>> sourcePartSets = new Dictionary<int, HashSet<PartSim>>();

        Dictionary<int, HashSet<PartSim>> stagePartSets = new Dictionary<int, HashSet<PartSim>>();

        HashSet<PartSim> visited = new HashSet<PartSim>();

        public void DumpSourcePartSets(String msg)
        {
            MonoBehaviour.print("DumpSourcePartSets " + msg);
            foreach (int type in sourcePartSets.Keys)
            {
                MonoBehaviour.print("SourcePartSet for " + ResourceContainer.GetResourceName(type));
                HashSet<PartSim> sourcePartSet = sourcePartSets[type];
                if (sourcePartSet.Count > 0)
                {
                    foreach (PartSim partSim in sourcePartSet)
                    {
                        MonoBehaviour.print("Part " + partSim.name + ":" + partSim.partId);
                    }
                }
                else
                {
                    MonoBehaviour.print("No parts");
                }
            }
        }

        public bool SetResourceDrains(List<PartSim> allParts, List<PartSim> allFuelLines, HashSet<PartSim> drainingParts)
        {
            LogMsg log = null;
            //DumpSourcePartSets("before clear");
            foreach (HashSet<PartSim> sourcePartSet in sourcePartSets.Values)
            {
                sourcePartSet.Clear();
            }
            //DumpSourcePartSets("after clear");

            for (int index = 0; index < this.resourceConsumptions.Types.Count; index++)
            {
                int type = this.resourceConsumptions.Types[index];

                HashSet<PartSim> sourcePartSet;
                if (!sourcePartSets.TryGetValue(type, out sourcePartSet))
                {
                    sourcePartSet = new HashSet<PartSim>();
                    sourcePartSets.Add(type, sourcePartSet);
                }

                switch ((ResourceFlowMode)this.resourceFlowModes[type])
                {
                    case ResourceFlowMode.NO_FLOW:
                        if (partSim.resources[type] > SimManager.RESOURCE_MIN && partSim.resourceFlowStates[type] != 0)
                        {
                            //sourcePartSet = new HashSet<PartSim>();
                            //MonoBehaviour.print("SetResourceDrains(" + name + ":" + partId + ") setting sources to just this");
                            sourcePartSet.Add(partSim);
                        }
                        break;

                    case ResourceFlowMode.ALL_VESSEL:
                    case ResourceFlowMode.ALL_VESSEL_BALANCE:
                        for (int i = 0; i < allParts.Count; i++)
                        {
                            PartSim aPartSim = allParts[i];
                            if (aPartSim.resources[type] > SimManager.RESOURCE_MIN && aPartSim.resourceFlowStates[type] != 0)
                            {
                                sourcePartSet.Add(aPartSim);
                            }
                        }
                        break;

                    case ResourceFlowMode.STAGE_PRIORITY_FLOW:
                    case ResourceFlowMode.STAGE_PRIORITY_FLOW_BALANCE:

                        foreach (HashSet<PartSim> stagePartSet in stagePartSets.Values)
                        {
                            stagePartSet.Clear();
                        }
                        var maxStage = -1;

                        //Logger.Log(type);
                        for (int i = 0; i < allParts.Count; i++)
                        {
                            var aPartSim = allParts[i];
                            if (aPartSim.resources[type] <= SimManager.RESOURCE_MIN || aPartSim.resourceFlowStates[type] == 0)
                            {
                                continue;
                            }

                            int stage = aPartSim.DecouplerCount();
                            if (stage > maxStage)
                            {
                                maxStage = stage;
                            }

                            HashSet<PartSim> tempPartSet;
                            if (!stagePartSets.TryGetValue(stage, out tempPartSet))
                            {
                                tempPartSet = new HashSet<PartSim>();
                                stagePartSets.Add(stage, tempPartSet);
                            }
                            tempPartSet.Add(aPartSim);
                        }

                        for (int j = maxStage; j >= 0; j--)
                        {
                            HashSet<PartSim> stagePartSet;
                            if (stagePartSets.TryGetValue(j, out stagePartSet) && stagePartSet.Count > 0)
                            {
                                // We have to copy the contents of the set here rather than copying the set reference or 
                                // bad things (tm) happen
                                foreach (PartSim aPartSim in stagePartSet)
                                {
                                    sourcePartSet.Add(aPartSim);
                                }
                                break;
                            }
                        }
                        break;

                    case ResourceFlowMode.STACK_PRIORITY_SEARCH:
                        visited.Clear();

                        if (SimManager.logOutput)
                        {
                            log = new LogMsg();
                            log.buf.AppendLine("Find " + ResourceContainer.GetResourceName(type) + " sources for " + partSim.name + ":" + partSim.partId);
                        }

                        // TODO: check fuel flow as 'PhysicsGlobals.Stack_PriUsesSurf' changed to false to subdue error
                        partSim.GetSourceSet(type, false, allParts, visited, sourcePartSet, log, "");
                        if (SimManager.logOutput && log != null)
                        {
                            MonoBehaviour.print(log.buf);
                        }
                        break;

                    case ResourceFlowMode.STAGE_STACK_FLOW:
                    case ResourceFlowMode.STAGE_STACK_FLOW_BALANCE:
                        visited.Clear();

                        if (SimManager.logOutput)
                        {
                            log = new LogMsg();
                            log.buf.AppendLine("Find " + ResourceContainer.GetResourceName(type) + " sources for " + partSim.name + ":" + partSim.partId);
                        }
                        partSim.GetSourceSet(type, true, allParts, visited, sourcePartSet, log, "");
                        if (SimManager.logOutput && log != null)
                        {
                            MonoBehaviour.print(log.buf);
                        }
                        break;

                    default:
                        MonoBehaviour.print("SetResourceDrains(" + partSim.name + ":" + partSim.partId + ") Unexpected flow type for " + ResourceContainer.GetResourceName(type) + ")");
                        break;
                }

                if (SimManager.logOutput)
                {
                    if (sourcePartSet.Count > 0)
                    {
                        log = new LogMsg();
                        log.buf.AppendLine("Source parts for " + ResourceContainer.GetResourceName(type) + ":");
                        foreach (PartSim partSim in sourcePartSet)
                        {
                            log.buf.AppendLine(partSim.name + ":" + partSim.partId);
                        }
                        MonoBehaviour.print(log.buf);
                    }
                }

                //DumpSourcePartSets("after " + ResourceContainer.GetResourceName(type));
            }
            
            // If we don't have sources for all the needed resources then return false without setting up any drains
            for (int i = 0; i < this.resourceConsumptions.Types.Count; i++)
            {
                int type = this.resourceConsumptions.Types[i];
                HashSet<PartSim> sourcePartSet; 
                if (!sourcePartSets.TryGetValue(type, out sourcePartSet) || sourcePartSet.Count == 0)
                {
                    if (SimManager.logOutput)
                    {
                        MonoBehaviour.print("No source of " + ResourceContainer.GetResourceName(type));
                    }

                    isActive = false;
                    return false;
                }
            }

            // Now we set the drains on the members of the sets and update the draining parts set
            for (int i = 0; i < this.resourceConsumptions.Types.Count; i++)
            {
                int type = this.resourceConsumptions.Types[i];
                HashSet<PartSim> sourcePartSet = sourcePartSets[type];
                // Loop through the members of the set 
                double amount = resourceConsumptions[type] / sourcePartSet.Count;
                foreach (PartSim partSim in sourcePartSet)
                {
                    if (SimManager.logOutput)
                    {
                        MonoBehaviour.print(
                            "Adding drain of " + amount + " " + ResourceContainer.GetResourceName(type) + " to " + partSim.name + ":" +
                            partSim.partId);
                    }

                    partSim.resourceDrains.Add(type, amount);
                    drainingParts.Add(partSim);
                }
            }
            return true;
        }
    }
}