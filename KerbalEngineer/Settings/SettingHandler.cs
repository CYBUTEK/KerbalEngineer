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
using System.IO;
using System.Linq;
using System.Xml.Serialization;

#endregion

namespace KerbalEngineer.Settings
{
    /// <summary>
    ///     Handles the management of setting items.
    /// </summary>
    public class SettingHandler
    {
        #region Fields

        /// <summary>
        ///     Stores the root settings directory for where all files will be saved.
        /// </summary>
        private static string settingsDirectory;

        #endregion

        #region Constructors

        /// <summary>
        ///     Creates an empty handler for managing setting items.
        /// </summary>
        public SettingHandler()
        {
            if (settingsDirectory == null)
            {
                settingsDirectory = Path.Combine(EngineerGlobals.AssemblyPath, "Settings");
            }

            this.Items = new List<SettingItem>();
        }

        /// <summary>
        ///     Sets the root settings directory if statically loaded.
        /// </summary>
        static SettingHandler()
        {
            if (settingsDirectory == null)
            {
                settingsDirectory = Path.Combine(EngineerGlobals.AssemblyPath, "Settings");
            }
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the directory where settings files are saved/loaded.
        /// </summary>
        public static string SettingsDirectory
        {
            get { return settingsDirectory; }
        }

        /// <summary>
        ///     Gets and sets the list of items.
        /// </summary>
        public List<SettingItem> Items { get; set; }

        #endregion

        #region Get Methods

        /// <summary>
        ///     Gets a setting object from its name or returns the default object.
        /// </summary>
        public T Get<T>(string name, T defaultObject)
        {
            foreach (var item in this.Items)
            {
                if (item.Name == name)
                {
                    return (T)Convert.ChangeType(item.Value, typeof(T));
                }
            }
            return defaultObject;
        }

        /// <summary>
        ///     Gets a setting object from its name and inputs it into the output object.  Returns true if a setting was found,
        ///     false if not.
        /// </summary>
        public bool Get<T>(string name, ref T outputObject)
        {
            foreach (var item in this.Items)
            {
                if (item.Name == name)
                {
                    outputObject = (T)Convert.ChangeType(item.Value, typeof(T));
                    return true;
                }
            }
            return false;
        }

        #endregion

        #region Set Methods

        /// <summary>
        ///     Sets a setting object from its name or creates one if it does not already exist.
        /// </summary>
        public void Set(string name, object value)
        {
            foreach (var item in this.Items)
            {
                if (item.Name == name)
                {
                    item.Value = value;
                    return;
                }
            }
            this.Items.Add(new SettingItem(name, value));
        }

        #endregion

        #region GetSet Methods

        /// <summary>
        ///     Gets a setting from its name or return the default object. Will add the object to the handler if it does not exist.
        /// </summary>
        public T GetSet<T>(string name, T defaultObject)
        {
            foreach (var item in this.Items)
            {
                if (item.Name == name)
                {
                    return (T)Convert.ChangeType(item.Value, typeof(T));
                }
            }
            if (defaultObject != null)
            {
                this.Items.Add(new SettingItem(name, defaultObject));
            }
            return defaultObject;
        }

        /// <summary>
        ///     Gets a setting from its name and inputs it into the output object. Will add the object to the handler if it does
        ///     not exist.
        /// </summary>
        public bool GetSet<T>(string name, ref T outputObject)
        {
            foreach (var item in this.Items)
            {
                if (item.Name == name)
                {
                    outputObject = (T)Convert.ChangeType(item.Value, typeof(T));
                    return true;
                }
            }
            if (outputObject != null)
            {
                this.Items.Add(new SettingItem(name, outputObject));
            }
            return false;
        }

        #endregion

        #region Saving

        #region Methods: public

        /// <summary>
        ///     Saves all the items in the handler into the specified file.
        /// </summary>
        public void Save(string fileName)
        {
            fileName = Path.Combine(settingsDirectory, fileName);

            this.Serialise(fileName);
        }

        #endregion

        #region Methods: private

        /// <summary>
        ///     Creates a directory if it does not already exist.
        /// </summary>
        private void CreateDirectory(string fileName)
        {
            var filePath = new FileInfo(fileName).DirectoryName;
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }
        }

        /// <summary>
        ///     Serialises all the items into an xml file.
        /// </summary>
        private void Serialise(string fileName)
        {
            this.CreateDirectory(fileName);
            using (var stream = new FileStream(fileName, FileMode.Create))
            {
                new XmlSerializer(typeof(List<SettingItem>), this.Items.Select(s => s.Value.GetType()).ToArray()).Serialize(stream, this.Items);
                stream.Close();
            }
        }

        #endregion

        #endregion

        #region Loading

        #region Methods: public

        /// <summary>
        ///     Deletes all the settings files.
        /// </summary>
        public static void DeleteSettings()
        {
            try
            {
                foreach (var file in Directory.GetFiles(settingsDirectory))
                {
                    File.Delete(file);
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }

        /// <summary>
        ///     Gets whether a settings file exists.
        /// </summary>
        public static bool Exists(string fileName)
        {
            return File.Exists(Path.Combine(settingsDirectory, fileName));
        }

        /// <summary>
        ///     Returns a SettingHandler object created from the specified file. (Optional extra types are required for
        ///     non-primitive items.)
        /// </summary>
        public static SettingHandler Load(string fileName, Type[] extraTypes = null)
        {
            fileName = Path.Combine(settingsDirectory, fileName);

            return Deserialise(fileName, extraTypes);
        }

        #endregion

        #region Methods: private

        /// <summary>
        ///     Returns a SettingHandler object containing items deserialized from the specified file.
        /// </summary>
        private static SettingHandler Deserialise(string fileName, Type[] extraTypes)
        {
            if (!File.Exists(fileName))
            {
                return new SettingHandler();
            }

            var handler = new SettingHandler();
            using (var stream = new FileStream(fileName, FileMode.Open))
            {
                handler.Items = new XmlSerializer(typeof(List<SettingItem>), extraTypes).Deserialize(stream) as List<SettingItem>;
                stream.Close();
            }
            return handler;
        }

        #endregion

        #endregion
    }
}