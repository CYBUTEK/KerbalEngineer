// Name:    Kerbal Engineer Redux
// Author:  CYBUTEK
// License: Attribution-NonCommercial-ShareAlike 3.0 Unported

using System.Collections.Generic;
using KerbalEngineer.Settings;
using KerbalEngineer.Simulation;
using UnityEngine;

namespace KerbalEngineer.FlightEngineer
{
    public class SectionList
    {
        #region Static Fields

        private static bool _hasLoadedSections = false;

        #endregion

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

        private bool _requireResize = false;
        /// <summary>
        /// Gets and sets whether to resize all displays.
        /// </summary>
        public bool RequireResize
        {
            get { return _requireResize; }
            set { _requireResize = value; }
        }

        private bool _hasVisibleSections = false;
        /// <summary>
        /// Gets whether there are visible sections.
        /// </summary>
        public bool HasVisibleSections
        {
            get { return _hasVisibleSections; }
        }

        private bool _hasAttachedSections = false;
        /// <summary>
        /// Gets whether there are attached sections.
        /// </summary>
        public bool HasAttachedSections
        {
            get { return _hasAttachedSections; }
        }

        #endregion

        #region Initialisation

        private SectionList()
        {
            _fixedSections.Add(new Section() { Title = "Orbital", Readouts = ReadoutList.Instance.GetCategory(ReadoutCategory.Orbital), Categories = new List<ReadoutCategory> { ReadoutCategory.Orbital }, ShortTitle = "ORBT" });
            _fixedSections.Add(new Section() { Title = "Surface", Readouts = ReadoutList.Instance.GetCategory(ReadoutCategory.Surface), Categories = new List<ReadoutCategory> { ReadoutCategory.Surface }, ShortTitle = "SURF" });
            _fixedSections.Add(new Section() { Title = "Vessel", Readouts = ReadoutList.Instance.GetCategory(ReadoutCategory.Vessel), Categories = new List<ReadoutCategory> { ReadoutCategory.Vessel }, ShortTitle = "VESL" });
            _fixedSections.Add(new Section() { Title = "Rendezvous", Readouts = ReadoutList.Instance.GetCategory(ReadoutCategory.Rendezvous), Categories = new List<ReadoutCategory> { ReadoutCategory.Rendezvous }, ShortTitle = "RDZV" });
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the fixed section with the provided name.
        /// </summary>
        public Section GetFixedSection(string name)
        {
            foreach (Section section in _fixedSections)
                if (section.Title == name)
                    return section;

            return null;
        }

        /// <summary>
        /// Gets the user section with the provided name.
        /// </summary>
        public Section GetUserSection(string name)
        {
            foreach (Section section in _userSections)
                if (section.Title == name)
                    return section;

            return null;
        }

        #endregion

        #region Update

        public void Update()
        {
            if (_hasLoadedSections) _hasLoadedSections = false;

            // If a resize is required propagate it to the handling objects.
            if (_requireResize)
            {
                _requireResize = false;
                FlightDisplay.Instance.RequireResize = true;

                foreach (Section section in _fixedSections)
                    section.Window.RequireResize = true;

                foreach (Section section in _userSections)
                    section.Window.RequireResize = true;
            }

            _hasVisibleSections = false;
            _hasAttachedSections = false;

            // Update all visible fixed sections.
            foreach (Section section in _fixedSections)
            {
                if (section.Visible)
                {
                    if (!_hasVisibleSections) _hasVisibleSections = true;
                    if (!section.Window.Visible && !_hasAttachedSections) _hasAttachedSections = true;
                    section.Update();
                }
            }

            // Update all visible user sections.
            foreach (Section section in _userSections)
            {
                if (section.Visible)
                {
                    if (!_hasVisibleSections) _hasVisibleSections = true;
                    if (!section.Window.Visible && !_hasAttachedSections) _hasAttachedSections = true;
                    section.Update();
                }
            }

            Surface.AtmosphericDetails.Instance.Update();
            SimulationManager.Instance.Gravity = FlightGlobals.getGeeForceAtPosition(FlightGlobals.ActiveVessel.GetWorldPos3D()).magnitude;
            SimulationManager.Instance.Atmosphere = FlightGlobals.getAtmDensity(FlightGlobals.ActiveVessel.staticPressure);
            SimulationManager.Instance.TryStartSimulation();
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
                if (!_hasLoadedSections)
                {
                    _hasLoadedSections = true;

                    SettingList list = SettingList.CreateFromFile(EngineerGlobals.AssemblyPath + "Settings/FlightSections");

                    List<string> fixedSectionNames = list.GetSetting("fixed_sections", new List<string>()) as List<string>;
                    List<string> userSectionNames = list.GetSetting("user_sections", new List<string>()) as List<string>;

                    // Load fixed sections.
                    foreach (string name in fixedSectionNames)
                    {
                        Section section = GetFixedSection(name);

                        if (section == null)
                        {
                            section = new Section(true, false) { Title = name };
                            _fixedSections.Add(section);
                        }
                        else
                        {
                            RenderingManager.AddToPostDrawQueue(0, section.Window.Draw);
                            RenderingManager.AddToPostDrawQueue(0, section.EditDisplay.Draw);
                        }

                        section.Load();
                    }

                    // Load user sections.
                    foreach (string name in userSectionNames)
                    {
                        Section section = GetUserSection(name);

                        if (section == null)
                        {
                            section = new Section(true, false) { Title = name };
                            _userSections.Add(section);
                        }
                        else
                        {
                            RenderingManager.AddToPostDrawQueue(0, section.Window.Draw);
                            RenderingManager.AddToPostDrawQueue(0, section.EditDisplay.Draw);
                        }

                        section.Load();
                    }

                    MonoBehaviour.print("[KerbalEngineer/FlightSections]: Successfully loaded settings.");
                }  
            }
            catch { MonoBehaviour.print("[KerbalEngineer/FlightSections]: Failed to load settings."); }
        }

        #endregion
    }
}
