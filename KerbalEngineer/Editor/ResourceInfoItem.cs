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

using KerbalEngineer.Extensions;

#endregion

namespace KerbalEngineer.Editor
{
    public class ResourceInfoItem
    {
        #region Constructors

        public ResourceInfoItem(PartResource resource)
        {
            this.Definition = resource.GetDefinition();
            this.Name = this.Definition.name;
            this.Amount = resource.amount;
        }

        #endregion

        #region Properties

        public double Amount { get; set; }

        public PartResourceDefinition Definition { get; set; }

        public double Mass
        {
            get { return this.Amount * this.Definition.density; }
        }

        public string Name { get; set; }

        #endregion
    }
}