// Name:    Kerbal Engineer Redux
// Author:  CYBUTEK
// License: Attribution-NonCommercial-ShareAlike 3.0 Unported

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace KerbalEngineer.Extensions
{
    public static class PartExtensions
    {
        /// <summary>
        /// Gets whether the part contains a PartModule.
        /// </summary>
        public static bool HasModule<T>(this Part part)
        {
            return part.Modules.OfType<T>().Count() > 0;
        }

        /// <summary>
        /// Gets whether the part contains a PartModule.
        /// </summary>
        public static bool HasModule(this Part part, string className)
        {
            return part.Modules.Contains(className);
        }

        /// <summary>
        /// Gets whether the part contains a PartModule.
        /// </summary>
        public static bool HasModule(this Part part, int moduleID)
        {
            return part.Modules.Contains(moduleID);
        }

        /// <summary>
        /// Gets a typed PartModule.
        /// </summary>
        public static T GetModule<T>(this Part part, string className) where T : PartModule
        {
            return (T)Convert.ChangeType(part.Modules[className], typeof(T));
        }

        /// <summary>
        /// Gets a typed PartModule.
        /// </summary>
        public static T GetModule<T>(this Part part, int classID) where T : PartModule
        {
            return (T)Convert.ChangeType(part.Modules[classID], typeof(T));
        }

        /// <summary>
        /// Gets a ModuleEngines typed PartModule.
        /// </summary>
        public static ModuleEngines GetModuleEngines(this Part part)
        {
            return part.GetModule<ModuleEngines>("ModuleEngines");
        }

        /// <summary>
        /// Gets a ModuleGimbal typed PartModule.
        /// </summary>
        public static ModuleGimbal GetModuleGimbal(this Part part)
        {
            return part.GetModule<ModuleGimbal>("ModuleGimbal");
        }

        /// <summary>
        /// Gets a ModuleDeployableSolarPanel typed PartModule.
        /// </summary>
        public static ModuleDeployableSolarPanel GetModuleDeployableSolarPanel(this Part part)
        {
            return part.GetModule<ModuleDeployableSolarPanel>("ModuleDeployableSolarPanel");
        }

        /// <summary>
        /// Gets a ModuleAlternator typed PartModule.
        /// </summary>
        public static ModuleAlternator GetModuleAlternator(this Part part)
        {
            return part.GetModule<ModuleAlternator>("ModuleAlternator");
        }

        /// <summary>
        /// Gets a ModuleGenerator typed PartModule.
        /// </summary>
        public static ModuleGenerator GetModuleGenerator(this Part part)
        {
            return part.GetModule<ModuleGenerator>("ModuleGenerator");
        }

        /// <summary>
        /// Gets a ModuleParachute typed PartModule.
        /// </summary>
        public static ModuleParachute GetModuleParachute(this Part part)
        {
            return part.GetModule<ModuleParachute>("ModuleParachute");
        }

        /// <summary>
        /// Gets the total mass of the part including resources.
        /// </summary>
        public static double GetWetMass(this Part part)
        {
            return (part.physicalSignificance == Part.PhysicalSignificance.FULL) ? part.mass + part.GetResourceMass() : part.GetResourceMass();
        }

        /// <summary>
        /// Gets the dry mass of the part.
        /// </summary>
        public static double GetDryMass(this Part part)
        {
            return (part.physicalSignificance == Part.PhysicalSignificance.FULL) ? part.mass : 0d;
        }

        /// <summary>
        /// Gets the maximum thrust of the part if it's an engine.
        /// </summary>
        public static double GetMaxThrust(this Part part)
        {
            return (part.IsEngine()) ? part.GetModuleEngines().maxThrust : 0d;
        }

        public static double GetSpecificImpulse(this Part part, float atmosphere)
        {
            return (part.IsEngine()) ? part.GetModuleEngines().atmosphereCurve.Evaluate(atmosphere) : 0d;
        }

        /// <summary>
        /// Gets whether the part has fuel.
        /// </summary>
        public static bool EngineHasFuel(this Part part)
        {
            return part.IsEngine() && !part.GetModuleEngines().getFlameoutState;
        }

        /// <summary>
        /// Gets whether the part contains resources.
        /// </summary>
        public static bool ContainsResources(this Part part)
        {
            return part.Resources.list.Count(p => p.amount > 0d) > 0;
        }

        public static bool ContainsResource(this Part part, int resourceID)
        {
            return part.Resources.Contains(resourceID);
        }

        /// <summary>
        /// Gets whether the part is a decoupler.
        /// </summary>
        public static bool IsDecoupler(this Part part)
        {
            return part.HasModule("ModuleDecouple") || part.HasModule("ModuleAnchoredDecoupler");
        }

        /// <summary>
        /// Gets whether the part is decoupled in a specified stage.
        /// </summary>
        public static bool IsDecoupledInStage(this Part part, int stage)
        {
            if ((part.IsDecoupler() || part.IsLaunchClamp()) && part.inverseStage == stage) return true;
            if (part.parent == null) return false;
            return part.parent.IsDecoupledInStage(stage);
        }

        /// <summary>
        /// Gets whether the part is a launch clamp.
        /// </summary>
        public static bool IsLaunchClamp(this Part part)
        {
            return part.HasModule("LaunchClamp");
        }

        /// <summary>
        /// Gets whether the part is an active engine.
        /// </summary>
        public static bool IsEngine(this Part part)
        {
            return part.HasModule("ModuleEngines");
        }

        /// <summary>
        /// Gets whether the part is a deployable solar panel.
        /// </summary>
        public static bool IsSolarPanel(this Part part)
        {
            return part.HasModule("ModuleDeployableSolarPanel");
        }

        /// <summary>
        /// Gets whether the part is a generator.
        /// </summary>
        public static bool IsGenerator(this Part part)
        {
            return part.HasModule("ModuleGenerator");
        }

        /// <summary>
        /// Gets whether the part is a command module.
        /// </summary>
        public static bool IsCommandModule(this Part part)
        {
            return part.HasModule("ModuleCommand");
        }

        /// <summary>
        /// Gets whether the part is a parachute.
        /// </summary>
        public static bool IsParachute(this Part part)
        {
            return part.HasModule("ModuleParachute");
        }

        /// <summary>
        /// Gets whether the part is a solid rocket motor.
        /// </summary>
        public static bool IsSolidRocket(this Part part)
        {
            return part.IsEngine() && part.GetModuleEngines().throttleLocked;
        }

        /// <summary>
        /// Gets whether the part is a sepratron.
        /// </summary>
        public static bool IsSepratron(this Part part)
        {
            return (part.IsSolidRocket() && part.ActivatesEvenIfDisconnected && part.IsDecoupledInStage(part.inverseStage));
        }

        /// <summary>
        /// Gets whether the part is a fuel line.
        /// </summary>
        public static bool IsFuelLine(this Part part)
        {
            return (part is FuelLine);
        }

        /// <summary>
        /// Gets whether the part is considered a primary part on the vessel.
        /// </summary>
        public static bool IsPrimary(this Part part, List<Part> partsList, PartModule module)
        {
            foreach (Part vesselPart in partsList)
            {
                if (vesselPart.HasModule(module.ClassID))
                {
                    if (vesselPart == part)
                    {
                        return true;
                    }
                    else
                    {
                        break;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Gets whether the part has a one shot animation.
        /// </summary>
        public static bool HasOneShotAnimation(this Part part)
        {
            if (part.HasModule("ModuleAnimateGeneric"))
            {
                return part.GetModule<ModuleAnimateGeneric>("ModuleAnimateGeneric").isOneShot;
            }

            return false;
        }
    }
}
