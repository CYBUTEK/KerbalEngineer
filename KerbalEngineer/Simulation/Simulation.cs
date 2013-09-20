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
using KerbalEngineer.Extensions;
using UnityEngine;

namespace KerbalEngineer.Simulation
{
    public class Simulation
    {
        List<PartSim> partSims;
        List<Part> partList;

        int currentStage = 0;
        bool firstSimulation = true;

        public const double STD_GRAVITY = 9.81d;

        public bool FirstSimulation
        {
            get
            {
                if (firstSimulation)
                {
                    firstSimulation = false;
                    return true;
                }

                return false;
            }
        }

        public Simulation(List<Part> parts)
        {
            this.partList = parts;
        }

        public Stage[] RunSimulation(double gravity, double atmosphere = 0)
        {
            currentStage = Staging.lastStage;
            Stage[] stages = new Stage[currentStage + 1];

            BuildVessel(this.partList, atmosphere);

            while (currentStage >= 0)
            {
                Stage stage = new Stage(currentStage);
                double stageTime = 0d;
                double stageDeltaV = 0d;
                double totalStageThrust = 0d;
                double totalStageActualThrust = 0d;

                double totalStageFlowRate = 0d;
                double totalStageIspFlowRate = 0d;

                foreach (PartSim engine in ActiveEngines)
                {
                    totalStageActualThrust += engine.actualThrust;
                    totalStageThrust += engine.thrust;

                    totalStageFlowRate += engine.ResourceConsumptions.Mass;
                    totalStageIspFlowRate += engine.ResourceConsumptions.Mass * engine.isp;
                }

                if (totalStageFlowRate > 0d && totalStageIspFlowRate > 0d)
                {
                    stage.isp = totalStageIspFlowRate / totalStageFlowRate;
                }

                stage.thrust = totalStageThrust;
                stage.thrustToWeight = (double)(totalStageThrust / (ShipMass * gravity));
                stage.actualThrust = totalStageActualThrust;
                stage.actualThrustToWeight = (double)(totalStageActualThrust / (ShipMass * gravity));

                foreach (PartSim partSim in partSims)
                {
                    if (partSim.decoupledInStage == currentStage - 1)
                    {
                        stage.cost += partSim.part.partInfo.cost;
                        stage.mass += partSim.GetStartMass(currentStage);
                    }
                }

                int loopCounter = 0;
                while (!AllowedToStage())
                {
                    loopCounter++;

                    List<PartSim> engines = ActiveEngines;
                    totalStageThrust = 0d;
                    foreach (PartSim engine in engines)
                    {
                        if (engine.actualThrust > 0d)
                        {
                            totalStageThrust += engine.actualThrust;
                        }
                        else
                        {
                            totalStageThrust += engine.thrust;
                        }
                    }

                    SetResourceDrainRates();

                    double resourceDrainTime = double.MaxValue;
                    foreach (PartSim partSim in partSims)
                    {
                        double time = 0d;
                        time = partSim.TimeToDrainResource();
                        if (time < resourceDrainTime)
                        {
                            resourceDrainTime = time;
                        }
                    }

                    double startMass = ShipMass;
                    foreach (PartSim partSim in partSims)
                    {
                        partSim.DrainResources(resourceDrainTime);
                    }
                    double endMass = ShipMass;
                    stageTime += resourceDrainTime;

                    if (resourceDrainTime > 0d && startMass > endMass && startMass > 0d && endMass > 0d)
                    {
                        stageDeltaV += (stage.isp * STD_GRAVITY) * Math.Log(startMass / endMass);
                    }

                    if (loopCounter == 1000)
                    {
                        break;
                    }
                }

                stage.partCount = partSims.Count;
                stage.deltaV = stageDeltaV;
                if (stageTime < 9999)
                {
                    stage.time = stageTime;
                }
                else
                {
                    stage.time = 0d;
                }
                stages[currentStage] = stage;

                currentStage--;
                ActivateStage();
            }

            for (int i = 0; i < stages.Length; i++)
            {
                for (int j = i; j >= 0; j--)
                {
                    stages[i].totalCost += stages[j].cost;
                    stages[i].totalMass += stages[j].mass;
                    stages[i].totalDeltaV += stages[j].deltaV;
                    stages[i].totalTime += stages[j].time;
                }
                for (int j = i; j < stages.Length; j++)
                {
                    stages[i].inverseTotalDeltaV += stages[j].deltaV;
                }

                if (stages[i].totalTime > 9999d)
                {
                    stages[i].totalTime = 0d;
                }
            }

            return stages;
        }

        private void BuildVessel(List<Part> parts, double atmosphere)
        {
            partSims = new List<PartSim>();
            Hashtable partSimLookup = new Hashtable();
            foreach (Part part in parts)
            {
                PartSim partSim = new PartSim(part, atmosphere);

                if (partSim.decoupledInStage < currentStage)
                {
                    partSim.SetResourceConsumptions();
                    partSims.Add(partSim);
                    partSimLookup.Add(part, partSim);
                }
            }

            foreach (PartSim partSim in partSims)
            {
                partSim.SetSourceNodes(partSimLookup);
            }
        }

        private bool AllowedToStage()
        {
            List<PartSim> engines = ActiveEngines;

            if (engines.Count == 0)
            {
                return true;
            }

            foreach (PartSim partSim in partSims)
            {
                if (partSim.decoupledInStage == (currentStage - 1) && !partSim.part.IsSepratron())
                {    

                    if (!partSim.Resources.Empty || engines.Contains(partSim))
                    {
                        return false;
                    }
                }
            }

            if (currentStage > 0)
            {
                return true;
            }

            return false;
        }

        private void SetResourceDrainRates()
        {
            foreach (PartSim partSim in partSims)
            {
                partSim.ResourceDrains.Reset();
            }

            List<PartSim> engines = ActiveEngines;

            foreach (PartSim engine in engines)
            {
                engine.SetResourceDrainRates(partSims);
            }
        }

        private void ActivateStage()
        {
            List<PartSim> decoupledParts = new List<PartSim>();

            foreach (PartSim partSim in partSims)
            {
                if (partSim.decoupledInStage == currentStage)
                {
                    decoupledParts.Add(partSim);
                }
            }

            foreach (PartSim partSim in decoupledParts)
            {
                partSims.Remove(partSim);
            }

            foreach (PartSim partSim in partSims)
            {
                foreach (PartSim decoupledPart in decoupledParts)
                {
                    partSim.RemoveSourcePart(decoupledPart);
                }
            }
        }

        private List<PartSim> ActiveEngines
        {
            get
            {
                List<PartSim> engines = new List<PartSim>();
                {
                    foreach (PartSim partSim in partSims)
                    {
                        if (partSim.part.IsEngine() && partSim.InverseStage >= currentStage && partSim.CanDrawNeededResources(partSims))
                        {
                            engines.Add(partSim);
                        }
                    }
                }

                return engines;
            }
        }

        private bool StageHasSolids
        {
            get
            {
                foreach (PartSim engine in ActiveEngines)
                {
                    if (engine.IsSolidMotor)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        private double ShipStartMass
        {
            get
            {
                double mass = 0d;

                foreach (PartSim partSim in partSims)
                {
                    mass += partSim.GetStartMass(currentStage);
                }

                return mass;
            }
        }

        private double ShipMass
        {
            get
            {
                double mass = 0d;

                foreach (PartSim partSim in partSims)
                {
                    mass += partSim.GetMass(currentStage);
                }

                return mass;
            }
        }
    }
}
