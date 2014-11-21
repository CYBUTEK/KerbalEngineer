﻿// 
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
using System.Diagnostics;
using System.Text;

using UnityEngine;

#endregion

namespace KerbalEngineer.VesselSimulator
{
    public class Simulation
    {
        private const double STD_GRAVITY = 9.82;
        private const double SECONDS_PER_DAY = 86400;
        private readonly Stopwatch _timer = new Stopwatch();
        private List<EngineSim> activeEngines;
        private List<EngineSim> allEngines;
        private List<PartSim> allFuelLines;
        private List<PartSim> allParts;
        private double atmosphere;
        private int currentStage;
        private double currentisp;
        private bool doingCurrent;
        private List<PartSim> dontStageParts;
        private HashSet<PartSim> drainingParts;
        private HashSet<int> drainingResources;
        private double gravity;

        private int lastStage;
        private List<Part> partList;
        private double simpleTotalThrust;
        private double stageStartMass;
        private double stageTime;
        private double stepEndMass;
        private double stepStartMass;
        private double totalStageActualThrust;
        private double totalStageFlowRate;
        private double totalStageIspFlowRate;
        private double totalStageThrust;
        private Vector3 vecActualThrust;
        private Vector3 vecStageDeltaV;
        private Vector3 vecThrust;
        private double velocity;
        public String vesselName;
        public VesselType vesselType;

        public Simulation()
        {
            if (SimManager.logOutput)
            {
                MonoBehaviour.print("Simulation created");
            }
        }

        private double ShipStartMass
        {
            get
            {
                double mass = 0d;

                foreach (PartSim partSim in this.allParts)
                {
                    mass += partSim.GetStartMass();
                }

                return mass;
            }
        }

        private double ShipMass
        {
            get
            {
                double mass = 0d;

                foreach (PartSim partSim in this.allParts)
                {
                    mass += partSim.GetMass();
                }

                return mass;
            }
        }

        // This function prepares the simulation by creating all the necessary data structures it will 
        // need during the simulation.  All required data is copied from the core game data structures 
        // so that the simulation itself can be run in a background thread without having issues with 
        // the core game changing the data while the simulation is running.
        public bool PrepareSimulation(List<Part> parts, double theGravity, double theAtmosphere = 0, double theVelocity = 0, bool dumpTree = false, bool vectoredThrust = false)
        {
            LogMsg log = null;
            if (SimManager.logOutput)
            {
                log = new LogMsg();
                log.buf.AppendLine("PrepareSimulation started");
                dumpTree = true;
            }
            this._timer.Start();

            // Store the parameters in members for ease of access in other functions
            this.partList = parts;
            this.gravity = theGravity;
            this.atmosphere = theAtmosphere;
            this.velocity = theVelocity;
            this.lastStage = Staging.lastStage;
            //MonoBehaviour.print("lastStage = " + lastStage);

            // Create the lists for our simulation parts
            this.allParts = new List<PartSim>();
            this.allFuelLines = new List<PartSim>();
            this.drainingParts = new HashSet<PartSim>();
            this.allEngines = new List<EngineSim>();
            this.activeEngines = new List<EngineSim>();
            this.drainingResources = new HashSet<int>();

            // A dictionary for fast lookup of Part->PartSim during the preparation phase
            Dictionary<Part, PartSim> partSimLookup = new Dictionary<Part, PartSim>();

            if (this.partList.Count > 0 && this.partList[0].vessel != null)
            {
                this.vesselName = this.partList[0].vessel.vesselName;
                this.vesselType = this.partList[0].vessel.vesselType;
            }

            // First we create a PartSim for each Part (giving each a unique id)
            int partId = 1;
            foreach (Part part in this.partList)
            {
                // If the part is already in the lookup dictionary then log it and skip to the next part
                if (partSimLookup.ContainsKey(part))
                {
                    if (log != null)
                    {
                        log.buf.AppendLine("Part " + part.name + " appears in vessel list more than once");
                    }
                    continue;
                }

                // Create the PartSim
                PartSim partSim = new PartSim(part, partId, this.atmosphere, log);

                // Add it to the Part lookup dictionary and the necessary lists
                partSimLookup.Add(part, partSim);
                this.allParts.Add(partSim);
                if (partSim.isFuelLine)
                {
                    this.allFuelLines.Add(partSim);
                }
                if (partSim.isEngine)
                {
                    partSim.CreateEngineSims(this.allEngines, this.atmosphere, this.velocity, vectoredThrust, log);
                }

                partId++;
            }

            this.UpdateActiveEngines();

            // Now that all the PartSims have been created we can do any set up that needs access to other parts
            // First we set up all the parent links
            foreach (PartSim partSim in this.allParts)
            {
                partSim.SetupParent(partSimLookup, log);
            }

            // Then, in the VAB/SPH, we add the parent of each fuel line to the fuelTargets list of their targets
            if (HighLogic.LoadedSceneIsEditor)
            {
                foreach (PartSim partSim in this.allFuelLines)
                {
                    if ((partSim.part as FuelLine).target != null)
                    {
                        PartSim targetSim;
                        if (partSimLookup.TryGetValue((partSim.part as FuelLine).target, out targetSim))
                        {
                            if (log != null)
                            {
                                log.buf.AppendLine("Fuel line target is " + targetSim.name + ":" + targetSim.partId);
                            }

                            targetSim.fuelTargets.Add(partSim.parent);
                        }
                        else
                        {
                            if (log != null)
                            {
                                log.buf.AppendLine("No PartSim for fuel line target (" + partSim.part.partInfo.name + ")");
                            }
                        }
                    }
                    else
                    {
                        if (log != null)
                        {
                            log.buf.AppendLine("Fuel line target is null");
                        }
                    }
                }
            }

            //MonoBehaviour.print("SetupAttachNodes and count stages");
            foreach (PartSim partSim in this.allParts)
            {
                partSim.SetupAttachNodes(partSimLookup, log);
                if (partSim.decoupledInStage >= this.lastStage)
                {
                    this.lastStage = partSim.decoupledInStage + 1;
                }
            }

            // And finally release the Part references from all the PartSims
            //MonoBehaviour.print("ReleaseParts");
            foreach (PartSim partSim in this.allParts)
            {
                partSim.ReleasePart();
            }

            // And dereference the core's part list
            this.partList = null;

            this._timer.Stop();
            if (log != null)
            {
                log.buf.AppendLine("PrepareSimulation: " + this._timer.ElapsedMilliseconds + "ms");
                log.Flush();
            }

            if (dumpTree)
            {
                this.Dump();
            }

            return true;
        }

        // This function runs the simulation and returns a newly created array of Stage objects
        public Stage[] RunSimulation()
        {
            if (SimManager.logOutput)
            {
                MonoBehaviour.print("RunSimulation started");
            }
            this._timer.Start();

            LogMsg log = null;
            if (SimManager.logOutput)
            {
                log = new LogMsg();
            }

            // Start with the last stage to simulate
            // (this is in a member variable so it can be accessed by AllowedToStage and ActivateStage)
            this.currentStage = this.lastStage;

            // Work out which engines would be active if just doing the staging and if this is different to the 
            // currently active engines then generate an extra stage
            // Loop through all the engines
            bool anyActive = false;
            foreach (EngineSim engine in this.allEngines)
            {
                if (log != null)
                {
                    log.buf.AppendLine("Testing engine mod of " + engine.partSim.name + ":" + engine.partSim.partId);
                }
                bool bActive = engine.isActive;
                bool bStage = (engine.partSim.inverseStage >= this.currentStage);
                if (log != null)
                {
                    log.buf.AppendLine("bActive = " + bActive + "   bStage = " + bStage);
                }
                if (HighLogic.LoadedSceneIsFlight)
                {
                    if (bActive)
                    {
                        anyActive = true;
                    }
                    if (bActive != bStage)
                    {
                        // If the active state is different to the state due to staging
                        if (log != null)
                        {
                            log.buf.AppendLine("Need to do current active engines first");
                        }

                        this.doingCurrent = true;
                    }
                }
                else
                {
                    if (bStage)
                    {
                        if (log != null)
                        {
                            log.buf.AppendLine("Marking as active");
                        }

                        engine.isActive = true;
                    }
                }
            }

            // If we need to do current because of difference in engine activation and there actually are active engines
            // then we do the extra stage otherwise activate the next stage and don't treat it as current
            if (this.doingCurrent && anyActive)
            {
                this.currentStage++;
            }
            else
            {
                this.ActivateStage();
                this.doingCurrent = false;
            }

            // Create a list of lists of PartSims that prevent decoupling
            List<List<PartSim>> dontStagePartsLists = this.BuildDontStageLists(log);

            if (log != null)
            {
                log.Flush();
            }

            // Create the array of stages that will be returned
            Stage[] stages = new Stage[this.currentStage + 1];


            // Loop through the stages
            while (this.currentStage >= 0)
            {
                if (log != null)
                {
                    log.buf.AppendLine("Simulating stage " + this.currentStage);
                    log.buf.AppendLine("ShipMass = " + this.ShipMass);
                    log.Flush();
                    this._timer.Reset();
                    this._timer.Start();
                }

                // Update active engines and resource drains
                this.UpdateResourceDrains();

                // Create the Stage object for this stage
                Stage stage = new Stage();

                this.stageTime = 0d;
                this.vecStageDeltaV = Vector3.zero;
                this.stageStartMass = this.ShipMass;
                this.stepStartMass = this.stageStartMass;
                this.stepEndMass = 0;

                this.CalculateThrustAndISP();

                // Store various things in the Stage object
                stage.thrust = this.totalStageThrust;
                //MonoBehaviour.print("stage.thrust = " + stage.thrust);
                stage.thrustToWeight = this.totalStageThrust / (this.stageStartMass * this.gravity);
                stage.maxThrustToWeight = stage.thrustToWeight;
                //MonoBehaviour.print("StageMass = " + stageStartMass);
                //MonoBehaviour.print("Initial maxTWR = " + stage.maxThrustToWeight);
                stage.actualThrust = this.totalStageActualThrust;
                stage.actualThrustToWeight = this.totalStageActualThrust / (this.stageStartMass * this.gravity);

                // Calculate the cost and mass of this stage and add all engines and tanks that are decoupled
                // in the next stage to the dontStageParts list
                foreach (PartSim partSim in this.allParts)
                {
                    if (partSim.decoupledInStage == this.currentStage - 1)
                    {
                        stage.cost += partSim.cost;
                        stage.mass += partSim.GetStartMass();
                    }
                }

                this.dontStageParts = dontStagePartsLists[this.currentStage];

                if (log != null)
                {
                    log.buf.AppendLine("Stage setup took " + this._timer.ElapsedMilliseconds + "ms");

                    if (this.dontStageParts.Count > 0)
                    {
                        log.buf.AppendLine("Parts preventing staging:");
                        foreach (PartSim partSim in this.dontStageParts)
                        {
                            partSim.DumpPartToBuffer(log.buf, "");
                        }
                    }
                    else
                    {
                        log.buf.AppendLine("No parts preventing staging");
                    }

                    log.Flush();
                }

                // Now we will loop until we are allowed to stage
                int loopCounter = 0;
                while (!this.AllowedToStage())
                {
                    loopCounter++;
                    //MonoBehaviour.print("loop = " + loopCounter);

                    // Calculate how long each draining tank will take to drain and run for the minimum time
                    double resourceDrainTime = double.MaxValue;
                    PartSim partMinDrain = null;
                    foreach (PartSim partSim in this.drainingParts)
                    {
                        double time = partSim.TimeToDrainResource();
                        if (time < resourceDrainTime)
                        {
                            resourceDrainTime = time;
                            partMinDrain = partSim;
                        }
                    }

                    if (log != null)
                    {
                        MonoBehaviour.print("Drain time = " + resourceDrainTime + " (" + partMinDrain.name + ":" + partMinDrain.partId + ")");
                    }

                    foreach (PartSim partSim in this.drainingParts)
                    {
                        partSim.DrainResources(resourceDrainTime);
                    }

                    // Get the mass after draining
                    this.stepEndMass = this.ShipMass;
                    this.stageTime += resourceDrainTime;

                    double stepEndTWR = this.totalStageThrust / (this.stepEndMass * this.gravity);
                    //MonoBehaviour.print("After drain mass = " + stepEndMass);
                    //MonoBehaviour.print("currentThrust = " + totalStageThrust);
                    //MonoBehaviour.print("currentTWR = " + stepEndTWR);
                    if (stepEndTWR > stage.maxThrustToWeight)
                    {
                        stage.maxThrustToWeight = stepEndTWR;
                    }

                    //MonoBehaviour.print("newMaxTWR = " + stage.maxThrustToWeight);

                    // If we have drained anything and the masses make sense then add this step's deltaV to the stage total
                    if (resourceDrainTime > 0d && this.stepStartMass > this.stepEndMass && this.stepStartMass > 0d && this.stepEndMass > 0d)
                    {
                        this.vecStageDeltaV += this.vecThrust * (float)((this.currentisp * STD_GRAVITY * Math.Log(this.stepStartMass / this.stepEndMass)) / this.simpleTotalThrust);
                    }

                    // Update the active engines and resource drains for the next step
                    this.UpdateResourceDrains();

                    // Recalculate the current thrust and isp for the next step
                    this.CalculateThrustAndISP();

                    // Check if we actually changed anything
                    if (this.stepStartMass == this.stepEndMass)
                    {
                        //MonoBehaviour.print("No change in mass");
                        break;
                    }

                    // Check to stop rampant looping
                    if (loopCounter == 1000)
                    {
                        MonoBehaviour.print("exceeded loop count");
                        MonoBehaviour.print("stageStartMass = " + this.stageStartMass);
                        MonoBehaviour.print("stepStartMass = " + this.stepStartMass);
                        MonoBehaviour.print("StepEndMass   = " + this.stepEndMass);
                        break;
                    }

                    // The next step starts at the mass this one ended at
                    this.stepStartMass = this.stepEndMass;
                }

                // Store more values in the Stage object and stick it in the array

                // Store the magnitude of the deltaV vector
                stage.deltaV = this.vecStageDeltaV.magnitude;
                stage.resourceMass = this.stepStartMass - this.stepEndMass;

                // Recalculate effective stage isp from the stage deltaV (flip the standard deltaV calculation around)
                // Note: If the mass doesn't change then this is a divide by zero
                if (this.stageStartMass != this.stepStartMass)
                {
                    stage.isp = stage.deltaV / (STD_GRAVITY * Math.Log(this.stageStartMass / this.stepStartMass));
                }
                else
                {
                    stage.isp = 0;
                }

                // Zero stage time if more than a day (this should be moved into the window code)
                stage.time = (this.stageTime < SECONDS_PER_DAY) ? this.stageTime : 0d;
                stage.number = this.doingCurrent ? -1 : this.currentStage; // Set the stage number to -1 if doing current engines
                stage.totalPartCount = this.allParts.Count;
                stages[this.currentStage] = stage;

                // Now activate the next stage
                this.currentStage--;
                this.doingCurrent = false;

                if (log != null)
                {
                    // Log how long the stage took
                    this._timer.Stop();
                    MonoBehaviour.print("Simulating stage took " + this._timer.ElapsedMilliseconds + "ms");
                    stage.Dump();
                    this._timer.Reset();
                    this._timer.Start();
                }

                // Activate the next stage
                this.ActivateStage();

                if (log != null)
                {
                    // Log how long it took to activate
                    this._timer.Stop();
                    MonoBehaviour.print("ActivateStage took " + this._timer.ElapsedMilliseconds + "ms");
                }
            }

            // Now we add up the various total fields in the stages
            for (int i = 0; i < stages.Length; i++)
            {
                // For each stage we total up the cost, mass, deltaV and time for this stage and all the stages above
                for (int j = i; j >= 0; j--)
                {
                    stages[i].totalCost += stages[j].cost;
                    stages[i].totalMass += stages[j].mass;
                    stages[i].totalDeltaV += stages[j].deltaV;
                    stages[i].totalTime += stages[j].time;
                    stages[i].partCount = i > 0 ? stages[i].totalPartCount - stages[i - 1].totalPartCount : stages[i].totalPartCount;
                }
                // We also total up the deltaV for stage and all stages below
                for (int j = i; j < stages.Length; j++)
                {
                    stages[i].inverseTotalDeltaV += stages[j].deltaV;
                }

                // Zero the total time if the value will be huge (24 hours?) to avoid the display going weird
                // (this should be moved into the window code)
                if (stages[i].totalTime > SECONDS_PER_DAY)
                {
                    stages[i].totalTime = 0d;
                }
            }

            if (log != null)
            {
                this._timer.Stop();
                MonoBehaviour.print("RunSimulation: " + this._timer.ElapsedMilliseconds + "ms");
            }

            return stages;
        }

        private List<List<PartSim>> BuildDontStageLists(LogMsg log)
        {
            if (log != null)
            {
                log.buf.AppendLine("Creating list with capacity of " + (this.currentStage + 1));
            }
            List<List<PartSim>> lists = new List<List<PartSim>>();
            for (int i = 0; i <= this.currentStage; i++)
            {
                lists.Add(new List<PartSim>());
            }

            foreach (PartSim partSim in this.allParts)
            {
                if (partSim.isEngine || !partSim.Resources.Empty)
                {
                    if (log != null)
                    {
                        log.buf.AppendLine(partSim.name + ":" + partSim.partId + " is engine or tank, decoupled = " + partSim.decoupledInStage);
                    }

                    if (partSim.decoupledInStage < -1 || partSim.decoupledInStage > this.currentStage - 1)
                    {
                        if (log != null)
                        {
                            log.buf.AppendLine("decoupledInStage out of range");
                        }
                    }
                    else
                    {
                        lists[partSim.decoupledInStage + 1].Add(partSim);
                    }
                }
            }

            for (int i = 1; i <= this.lastStage; i++)
            {
                if (lists[i].Count == 0)
                {
                    lists[i] = lists[i - 1];
                }
            }

            return lists;
        }

        // This function simply rebuilds the active engines by testing the isActive flag of all the engines
        private void UpdateActiveEngines()
        {
            this.activeEngines.Clear();
            foreach (EngineSim engine in this.allEngines)
            {
                if (engine.isActive)
                {
                    this.activeEngines.Add(engine);
                }
            }
        }

        private void CalculateThrustAndISP()
        {
            // Reset all the values
            this.vecThrust = Vector3.zero;
            this.vecActualThrust = Vector3.zero;
            this.simpleTotalThrust = 0d;
            this.totalStageThrust = 0d;
            this.totalStageActualThrust = 0d;
            this.totalStageFlowRate = 0d;
            this.totalStageIspFlowRate = 0d;

            // Loop through all the active engines totalling the thrust, actual thrust and mass flow rates
            // The thrust is totalled as vectors
            foreach (EngineSim engine in this.activeEngines)
            {
                this.simpleTotalThrust += engine.thrust;
                this.vecThrust += ((float)engine.thrust * engine.thrustVec);
                this.vecActualThrust += ((float)engine.actualThrust * engine.thrustVec);

                this.totalStageFlowRate += engine.ResourceConsumptions.Mass;
                this.totalStageIspFlowRate += engine.ResourceConsumptions.Mass * engine.isp;
            }

            //MonoBehaviour.print("vecThrust = " + vecThrust.ToString() + "   magnitude = " + vecThrust.magnitude);

            this.totalStageThrust = this.vecThrust.magnitude;
            this.totalStageActualThrust = this.vecActualThrust.magnitude;

            // Calculate the effective isp at this point
            if (this.totalStageFlowRate > 0d && this.totalStageIspFlowRate > 0d)
            {
                this.currentisp = this.totalStageIspFlowRate / this.totalStageFlowRate;
            }
            else
            {
                this.currentisp = 0;
            }
        }

        // This function does all the hard work of working out which engines are burning, which tanks are being drained 
        // and setting the drain rates
        private void UpdateResourceDrains()
        {
            // Update the active engines
            this.UpdateActiveEngines();

            // Empty the draining resources set
            this.drainingResources.Clear();

            // Reset the resource drains of all draining parts
            foreach (PartSim partSim in this.drainingParts)
            {
                partSim.ResourceDrains.Reset();
            }

            // Empty the draining parts set
            this.drainingParts.Clear();

            // Loop through all the active engine modules
            foreach (EngineSim engine in this.activeEngines)
            {
                // Set the resource drains for this engine
                if (engine.SetResourceDrains(this.allParts, this.allFuelLines, this.drainingParts))
                {
                    // If it is active then add the consumed resource types to the set
                    foreach (int type in engine.ResourceConsumptions.Types)
                    {
                        this.drainingResources.Add(type);
                    }
                }
            }

            // Update the active engines again to remove any engines that have no fuel supply
            this.UpdateActiveEngines();

            if (SimManager.logOutput)
            {
                StringBuilder buffer = new StringBuilder(1024);
                buffer.AppendFormat("Active engines = {0:d}\n", this.activeEngines.Count);
                int i = 0;
                foreach (EngineSim engine in this.activeEngines)
                {
                    engine.DumpEngineToBuffer(buffer, "Engine " + (i++) + ":");
                }
                MonoBehaviour.print(buffer);
            }
        }

        // This function works out if it is time to stage
        private bool AllowedToStage()
        {
            StringBuilder buffer = null;
            if (SimManager.logOutput)
            {
                buffer = new StringBuilder(1024);
                buffer.AppendLine("AllowedToStage");
                buffer.AppendFormat("currentStage = {0:d}\n", this.currentStage);
            }

            if (this.activeEngines.Count > 0)
            {
                foreach (PartSim partSim in this.dontStageParts)
                {
                    if (SimManager.logOutput)
                    {
                        partSim.DumpPartToBuffer(buffer, "Testing: ");
                    }
                    //buffer.AppendFormat("isSepratron = {0}\n", partSim.isSepratron ? "true" : "false");

                    if (!partSim.isSepratron && !partSim.Resources.EmptyOf(this.drainingResources))
                    {
                        if (SimManager.logOutput)
                        {
                            partSim.DumpPartToBuffer(buffer, "Decoupled part not empty => false: ");
                            MonoBehaviour.print(buffer);
                        }

                        return false;
                    }

                    if (partSim.isEngine)
                    {
                        foreach (EngineSim engine in this.activeEngines)
                        {
                            if (engine.partSim == partSim)
                            {
                                if (SimManager.logOutput)
                                {
                                    partSim.DumpPartToBuffer(buffer, "Decoupled part is active engine => false: ");
                                    MonoBehaviour.print(buffer);
                                }
                                return false;
                            }
                        }
                    }
                }
            }

            if (this.currentStage == 0 && this.doingCurrent)
            {
                if (SimManager.logOutput)
                {
                    buffer.AppendLine("Current stage == 0 && doingCurrent => false");
                    MonoBehaviour.print(buffer);
                }
                return false;
            }

            if (SimManager.logOutput)
            {
                buffer.AppendLine("Returning true");
                MonoBehaviour.print(buffer);
            }
            return true;
        }

        // This function activates the next stage
        // currentStage must be updated before calling this function
        private void ActivateStage()
        {
            // Build a set of all the parts that will be decoupled
            HashSet<PartSim> decoupledParts = new HashSet<PartSim>();
            foreach (PartSim partSim in this.allParts)
            {
                if (partSim.decoupledInStage >= this.currentStage)
                {
                    decoupledParts.Add(partSim);
                }
            }

            foreach (PartSim partSim in decoupledParts)
            {
                // Remove it from the all parts list
                this.allParts.Remove(partSim);
                if (partSim.isEngine)
                {
                    // If it is an engine then loop through all the engine modules and remove all the ones from this engine part
                    for (int i = this.allEngines.Count - 1; i >= 0; i--)
                    {
                        if (this.allEngines[i].partSim == partSim)
                        {
                            this.allEngines.RemoveAt(i);
                        }
                    }
                }
                // If it is a fuel line then remove it from the list of all fuel lines
                if (partSim.isFuelLine)
                {
                    this.allFuelLines.Remove(partSim);
                }
            }

            // Loop through all the (remaining) parts
            foreach (PartSim partSim in this.allParts)
            {
                // Ask the part to remove all the parts that are decoupled
                partSim.RemoveAttachedParts(decoupledParts);
            }

            // Now we loop through all the engines and activate those that are ignited in this stage
            foreach (EngineSim engine in this.allEngines)
            {
                if (engine.partSim.inverseStage == this.currentStage)
                {
                    engine.isActive = true;
                }
            }
        }

        public void Dump()
        {
            StringBuilder buffer = new StringBuilder(1024);
            buffer.AppendFormat("Part count = {0:d}\n", this.allParts.Count);

            // Output a nice tree view of the rocket
            if (this.allParts.Count > 0)
            {
                PartSim root = this.allParts[0];
                while (root.parent != null)
                {
                    root = root.parent;
                }

                if (root.hasVessel)
                {
                    buffer.AppendFormat("vesselName = '{0}'  vesselType = {1}\n", this.vesselName, SimManager.GetVesselTypeString(this.vesselType));
                }

                root.DumpPartToBuffer(buffer, "", this.allParts);
            }

            MonoBehaviour.print(buffer);
        }
    }
}