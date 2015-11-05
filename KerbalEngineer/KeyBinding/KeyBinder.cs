// 
//     Kerbal Engineer Redux
// 
//     Copyright (C) 2015 CYBUTEK
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

namespace KerbalEngineer.KeyBinding
{
    using System.IO;
    using Helpers;
    using UnityEngine;

    public class KeyBinder
    {
        private static readonly string filePath = Path.Combine(EngineerGlobals.SettingsPath, "KeyBinds.xml");
        private static KeyBindingsObject bindings;

        static KeyBinder()
        {
            Load();
        }

        /// <summary>
        ///     Gets and sets the key bindings object.
        /// </summary>
        public static KeyBindingsObject Bindings
        {
            get
            {
                if (bindings == null)
                {
                    bindings = new KeyBindingsObject();
                }
                return bindings;
            }
            private set
            {
                if (value != null)
                {
                    bindings = value;
                }
            }
        }

        /// <summary>
        ///     Gets and sets the editor show/hide binding.
        /// </summary>
        public static KeyCode EditorShowHide
        {
            get
            {
                return Bindings.EditorShowHide;
            }
            set
            {
                Bindings.EditorShowHide = value;
                Save();
            }
        }

        /// <summary>
        ///     Gets and sets the flight show/hide binding.
        /// </summary>
        public static KeyCode FlightShowHide
        {
            get
            {
                return Bindings.FlightShowHide;
            }
            set
            {
                Bindings.FlightShowHide = value;
                Save();
            }
        }

        /// <summary>
        ///     Loads the key bindings from disk.
        /// </summary>
        public static void Load()
        {
            Bindings = XmlHelper.LoadObject<KeyBindingsObject>(filePath);
        }

        /// <summary>
        ///     Saves the key bindings to disk.
        /// </summary>
        public static void Save()
        {
            XmlHelper.SaveObject(filePath, Bindings);
        }
    }
}