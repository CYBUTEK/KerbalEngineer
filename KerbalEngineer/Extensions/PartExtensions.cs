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
        /// Gets the total mass of the part including resources.
        /// </summary>
        public static double GetTotalMass(this Part part)
        {
            return (part.physicalSignificance == Part.PhysicalSignificance.FULL) ? part.mass + part.GetResourceMass() : 0d;
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
            return (part.IsEngine()) ? part.GetModule<ModuleEngines>("ModuleEngines").maxThrust : 0d;
        }

        /// <summary>
        /// Gets whether the part has fuel.
        /// </summary>
        public static bool EngineHasFuel(this Part part)
        {
            return part.HasModule("ModuleEngines") && !part.GetModule<ModuleEngines>("ModuleEngines").getFlameoutState;
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
        /// Gets whether the part is a solid rocket motor.
        /// </summary>
        public static bool IsSolidRocket(this Part part)
        {
            return part.IsEngine() && part.GetModule<ModuleEngines>("ModuleEngines").throttleLocked;
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
        public static bool IsPrimary(this Part part, List<Part> partsList, int moduleID)
        {
            foreach (Part vesselPart in partsList)
            {
                if (vesselPart.HasModule(moduleID))
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
    }
}
