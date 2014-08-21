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

#endregion

namespace KerbalEngineer.Settings
{
    /// <summary>
    ///     A serialisable object for storing an item's name and value.
    /// </summary>
    public class SettingItem
    {
        #region Constructors

        /// <summary>
        ///     Creates and empty item object.
        /// </summary>
        public SettingItem() { }

        /// <summary>
        ///     Creates an item object containing a name and value.
        /// </summary>
        public SettingItem(string name, object value)
        {
            this.Name = name;
            this.Value = value;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets and sets the name of the item.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Gets and sets the object value of the item.
        /// </summary>
        public object Value { get; set; }

        #endregion
    }
}