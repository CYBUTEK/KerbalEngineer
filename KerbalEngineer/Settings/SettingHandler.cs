// Project:	KerbalEngineer
// Author:	CYBUTEK
// License:	Attribution-NonCommercial-ShareAlike 3.0 Unported

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
        #region Static Fields

        /// <summary>
        ///     Stores the root settings directory for where all files will be saved.
        /// </summary>
        private static string settingsDirectory;

        #endregion

        #region Constructors

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

        #endregion

        #region Properties

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

        /// <summary>
        ///     Saves all the items in the handler into the specified file.
        /// </summary>
        public void Save(string fileName)
        {
            fileName = Path.Combine(settingsDirectory, fileName);

            this.Serialise(fileName);
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

        #endregion

        #region Loading

        /// <summary>
        ///     Returns a SettingHandler object created from the specified file. (Optional extra types are required for
        ///     non-primitive items.)
        /// </summary>
        public static SettingHandler Load(string fileName, Type[] extraTypes = null)
        {
            fileName = Path.Combine(settingsDirectory, fileName);

            return Deserialise(fileName, extraTypes);
        }

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
    }
}