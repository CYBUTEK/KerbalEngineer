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

using System.Collections.Generic;
using System.IO;

using KerbalEngineer.Settings;

#endregion

namespace KerbalEngineer.Flight.Presets
{
    public class PresetLibrary
    {
        #region Fields

        private static readonly List<Preset> presets = new List<Preset>();
        private static readonly string rootPath = Path.Combine(EngineerGlobals.AssemblyPath, "Presets");

        #endregion

        #region Constructors

        static PresetLibrary()
        {
            Load();
        }

        #endregion

        #region Properties

        public static List<Preset> Presets
        {
            get { return presets; }
        }

        #endregion

        #region Methods: public

        public static void Add(Preset preset)
        {
            Presets.Add(preset);
        }

        public static void Load()
        {
            if (!Directory.Exists(rootPath))
            {
                Directory.CreateDirectory(rootPath);
            }
            foreach (var file in Directory.GetFiles(rootPath))
            {
                var handler = SettingHandler.Load(file, new[] {typeof(Preset)});
                presets.Add(handler.Get<Preset>("preset", null));
            }
        }

        public static bool Remove(Preset preset)
        {
            if (File.Exists(Path.Combine(rootPath, preset.FileName)))
            {
                File.Delete(Path.Combine(rootPath, preset.FileName));
            }
            return Presets.Remove(preset);
        }

        public static void Save()
        {
            Presets.ForEach(Save);
        }

        public static void Save(Preset preset)
        {
            if (!Directory.Exists(rootPath))
            {
                Directory.CreateDirectory(rootPath);
            }

            if (!Presets.Contains(preset))
            {
                Presets.Add(preset);
            }

            var handler = new SettingHandler();
            handler.Set("preset", preset);
            handler.Save(Path.Combine("../Presets", preset.FileName));

            ScreenMessages.PostScreenMessage("Saved Preset: " + preset.Name, 2.0f, ScreenMessageStyle.UPPER_CENTER);
        }

        #endregion
    }
}