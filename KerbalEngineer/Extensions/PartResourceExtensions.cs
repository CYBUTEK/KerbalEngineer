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

namespace KerbalEngineer.Extensions
{
    public static class PartResourceExtensions
    {
        #region Methods: public

        /// <summary>
        ///     Gets the cost of the resource.
        /// </summary>
        public static double GetCost(this PartResource resource)
        {
            return resource.amount * resource.info.unitCost;
        }

        /// <summary>
        ///     Gets the definition object for the resource.
        /// </summary>
        public static PartResourceDefinition GetDefinition(this PartResource resource)
        {
            return PartResourceLibrary.Instance.GetDefinition(resource.info.id);
        }

        /// <summary>
        ///     Gets the density of the resource.
        /// </summary>
        public static double GetDensity(this PartResource resource)
        {
            return resource.GetDefinition().density;
        }

        /// <summary>
        ///     Gets the mass of the resource.
        /// </summary>
        public static double GetMass(this PartResource resource)
        {
            return resource.amount * resource.GetDensity();
        }

        #endregion
    }
}