// Name:    Kerbal Engineer Redux
// Author:  CYBUTEK
// License: Attribution-NonCommercial-ShareAlike 3.0 Unported

using UnityEngine;

namespace KerbalEngineer.Extensions
{
    public static class PartResourceExtensions
    {
        /// <summary>
        /// Gets the definition object for the resource.
        /// </summary>
        public static PartResourceDefinition GetDefinition(this PartResource value)
        {
            return PartResourceLibrary.Instance.GetDefinition(value.info.id);
        }

        /// <summary>
        /// Gets the density of the resource.
        /// </summary>
        public static double GetDensity(this PartResource value)
        {
            return value.GetDefinition().density;
        }

        /// <summary>
        /// Gets the mass of the resource.
        /// </summary>
        public static double GetMass(this PartResource value)
        {
            return value.amount * value.GetDensity();
        }
    }
}
