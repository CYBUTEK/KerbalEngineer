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
    using System.Collections.Generic;
    using VesselSimulator;

    public class PartInfoItem
    {
        private static readonly Pool<PartInfoItem> pool = new Pool<PartInfoItem>(Create, Reset);

        public string Name { get; set; }

        public string Value { get; set; }

        private static PartInfoItem Create()
        {
            return new PartInfoItem();
        }

        public void Release()
        {
            pool.Release(this);
        }

        public static void Release(List<PartInfoItem> objList)
        {
            for (int i = 0; i < objList.Count; ++i)
            {
                objList[i].Release();
            }
        }

        private static void Reset(PartInfoItem obj)
        {
            obj.Name = string.Empty;
            obj.Value = string.Empty;
        }

        public static PartInfoItem Create(string name)
        {
            return New(name);
        }

        public static PartInfoItem Create(string name, string value)
        {
            return New(name, value);
        }

        public static PartInfoItem New(string name)
        {
            PartInfoItem obj = pool.Borrow();
            
            obj.Name = name;
            obj.Value = string.Empty;

            return obj;
        }

        public static PartInfoItem New(string name, string value)
        {
            PartInfoItem obj = pool.Borrow();

            obj.Name = name;
            obj.Value = value;

            return obj;
        }
    }
}