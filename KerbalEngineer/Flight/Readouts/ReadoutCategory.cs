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

#region Using Directives

using System;
using System.Collections.Generic;
using System.Linq;

#endregion

namespace KerbalEngineer.Flight.Readouts
{
    public class ReadoutCategory
    {
        #region Constructors

        public ReadoutCategory(string name)
        {
            this.Name = name;
        }

        public ReadoutCategory(string name, string description)
        {
            this.Name = name;
            this.Description = description;
        }

        static ReadoutCategory()
        {
            Categories = new List<ReadoutCategory>();
        }

        #endregion

        #region Properties

        public static List<ReadoutCategory> Categories { get; private set; }

        public static ReadoutCategory Selected { get; set; }

        public string Description { get; set; }

        public string Name { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Gets a category with the specified non-case sensitive name or creates it if required.
        /// </summary>
        public static ReadoutCategory GetCategory(string name)
        {
            if (Categories.Any(c => c.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase)))
            {
                return Categories.Find(c => c.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase));
            }

            var category = new ReadoutCategory(name);
            Categories.Add(category);
            return category;
        }

        public static void SetCategory(string name)
        {
            var category = GetCategory(name);
            category.Name = name;
        }

        public static void SetCategory(string name, string description)
        {
            var category = GetCategory(name);
            category.Name = name;
            category.Description = description;
        }

        public override string ToString()
        {
            return this.Name;
        }

        #endregion
    }
}