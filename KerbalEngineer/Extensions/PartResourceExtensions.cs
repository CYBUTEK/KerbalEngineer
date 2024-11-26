// 
//     Kerbal Engineer Redux
// 
// Extension methods are bad.

namespace KerbalEngineer.Extensions
{
    public static class PartResourceExtensions
    {
        #region Methods: public

        /// <summary>
        ///     Gets the cost of the resource.
        /// </summary>
        public static double GetCost(PartResource resource)
        {
            return resource.amount * resource.info.unitCost;
        }

        /// <summary>
        ///     Gets the definition object for the resource.
        /// </summary>
        public static PartResourceDefinition GetDefinition(PartResource resource)
        {
            return PartResourceLibrary.Instance.GetDefinition(resource.info.id);
        }

        /// <summary>
        ///     Gets the density of the resource.
        /// </summary>
        public static double GetDensity(PartResource resource)
        {
            return GetDefinition(resource).density;
        }

        /// <summary>
        ///     Gets the mass of the resource.
        /// </summary>
        public static double GetMass(PartResource resource)
        {
            return resource.amount * GetDensity(resource);
        }

        #endregion
    }
}