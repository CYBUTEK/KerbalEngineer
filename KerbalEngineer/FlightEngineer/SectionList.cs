// Name:    Kerbal Engineer Redux
// Author:  CYBUTEK
// License: Attribution-NonCommercial-ShareAlike 3.0 Unported

using System.Collections.Generic;
using KerbalEngineer.Settings;
using UnityEngine;

namespace KerbalEngineer.FlightEngineer
{
    public class SectionList
    {
        #region Instance

        private static SectionList _instance;
        /// <summary>
        /// Gets the current instance of the section list.
        /// </summary>
        public static SectionList Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new SectionList();

                return _instance;
            }
        }

        #endregion

        #region Properties

        private List<Section> _fixedSections = new List<Section>();
        /// <summary>
        /// Gets and sets the available fixed sections.
        /// </summary>
        public List<Section> FixedSections
        {
            get { return _fixedSections; }
            set { _fixedSections = value; }
        }

        private List<Section> _userSections = new List<Section>();
        /// <summary>
        /// Gets and sets the available user sections.
        /// </summary>
        public List<Section> UserSections
        {
            get { return _userSections; }
            set { _userSections = value; }
        }

        #endregion

        #region Initialisation

        private SectionList()
        {
            _fixedSections.Add(new SectionOrbital());
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets whether there are sections that are set to be visible.
        /// </summary>
        public bool HasVisibleSections()
        {
            foreach (Section section in _fixedSections)
                if (section.Visible)
                    return true;

            foreach (Section section in _userSections)
                if (section.Visible)
                    return true;

            return false;
        }

        public Section GetFixedSection(string name)
        {
            foreach (Section section in _fixedSections)
                if (section.Title == name)
                    return section;

            return null;
        }

        #endregion

        #region Save and Load

        // Saves the settings associated with the section list.
        public void Save()
        {
            List<string> fixedSectionNames = new List<string>();
            List<string> userSectionNames = new List<string>();

            foreach (Section section in _fixedSections)
            {
                fixedSectionNames.Add(section.Title);
                section.Save();
            }

            foreach (Section section in _userSections)
            {
                userSectionNames.Add(section.Title);
                section.Save();
            }

            try
            {
                SettingList list = new SettingList();
                list.AddSetting("fixed_sections", fixedSectionNames);
                list.AddSetting("user_sections", userSectionNames);
                SettingList.SaveToFile(EngineerGlobals.AssemblyPath + "Settings/FlightSections", list);

                MonoBehaviour.print("[KerbalEngineer/FlightSections]: Successfully saved settings.");
            }
            catch { MonoBehaviour.print("[KerbalEngineer/FlightSections]: Failed to save settings."); }
        }

        // Loads the settings associated with the section list.
        public void Load()
        {
            try
            {
                SettingList list = SettingList.CreateFromFile(EngineerGlobals.AssemblyPath + "Settings/FlightSections");

                List<string> fixedSectionNames = list.GetSetting("fixed_sections", new List<string>()) as List<string>;
                List<string> userSectionNames = list.GetSetting("user_sections", new List<string>()) as List<string>;

                foreach (string name in fixedSectionNames)
                    GetFixedSection(name).Load();

                MonoBehaviour.print("[KerbalEngineer/FlightSections]: Successfully loaded settings.");
            }
            catch { MonoBehaviour.print("[KerbalEngineer/FlightSections]: Failed to load settings."); }
        }

        #endregion
    }
}
