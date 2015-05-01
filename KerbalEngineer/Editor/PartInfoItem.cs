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

namespace KerbalEngineer.Editor
{
    using VesselSimulator;

    public class PartInfoItem : Pool<PartInfoItem>
    {
        public string Name { get; set; }

        public string Value { get; set; }

        public static PartInfoItem Create(string name)
        {
            return GetPoolObject().Initialise(name);
        }

        public static PartInfoItem Create(string name, string value)
        {
            return GetPoolObject().Initialise(name, value);
        }

        public PartInfoItem Initialise(string name)
        {
            Name = name;
            Value = string.Empty;

            return this;
        }

        public PartInfoItem Initialise(string name, string value)
        {
            Name = name;
            Value = value;

            return this;
        }
    }
}