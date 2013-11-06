// Name:    Kerbal Engineer Redux
// Author:  CYBUTEK
// License: Attribution-NonCommercial-ShareAlike 3.0 Unported

using System.Collections.Generic;
using KerbalEngineer.Settings;
using UnityEngine;

namespace KerbalEngineer.FlightEngineer
{
    public class Section
    {
        #region Properties

        protected GUIStyle TitleStyle { get; private set; }
        protected GUIStyle AreaStyle { get; private set; }
        protected GUIStyle LabelStyle { get; private set; }

        private List<Readout> _readouts = new List<Readout>();
        /// <summary>
        /// Gets and sets the readouts to be displayed.
        /// </summary>
        public List<Readout> Readouts
        {
            get { return _readouts; }
            set { _readouts = value; }
        }

        private bool _visible = false;
        /// <summary>
        /// Gets and sets whether the section is visible.
        /// </summary>
        public bool Visible
        {
            get { return _visible; }
            set
            {
                if (_visible != value)
                    FlightDisplay.Instance.RequireResize = true;

                _visible = value;
            }
        }

        private string _title = string.Empty;
        /// <summary>
        /// Gets and sets the section title.
        /// </summary>
        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }

        private string _shortTitle = string.Empty;
        /// <summary>
        /// Gets and sets the section short title.
        /// </summary>
        public string ShortTitle
        {
            get { return _shortTitle; }
            set { _shortTitle = value; }
        }

        private bool _isUser = false;
        /// <summary>
        /// Gets and sets whether the section was user created.
        /// </summary>
        public bool IsUser
        {
            get { return _isUser; }
            set { _isUser = value; }
        }

        private EditDisplay _editDisplay;
        /// <summary>
        /// Gets the edit display associated with the section.
        /// </summary>
        public EditDisplay EditDisplay
        {
            get { return _editDisplay; }
        }

        private List<ReadoutCategory> _categories = new List<ReadoutCategory>();
        /// <summary>
        /// Gets and sets the categories associated with the section.
        /// </summary>
        public List<ReadoutCategory> Categories
        {
            get { return _categories; }
            set { _categories = value; }
        }

        #endregion

        #region Initialisation

        public Section(bool isUserSection = false, bool isNewSection = true)
        {
            _editDisplay = HighLogic.fetch.gameObject.AddComponent<EditDisplay>();
            _editDisplay.Section = this;
            RenderingManager.AddToPostDrawQueue(0, _editDisplay.Draw);

            if (isUserSection)
            {
                _isUser = true;

                Title = "Custom " + (SectionList.Instance.UserSections.Count + 1);
                Categories.Add(ReadoutCategory.Orbital);
                Categories.Add(ReadoutCategory.Surface);
                Categories.Add(ReadoutCategory.Vessel);
                Categories.Add(ReadoutCategory.Rendezvous);
                Categories.Add(ReadoutCategory.Misc);
                Visible = true;

                if (isNewSection)
                    _editDisplay.Visible = true;
            }

            InitialiseStyles();
        }

        private void InitialiseStyles()
        {
            TitleStyle = new GUIStyle(HighLogic.Skin.label);
            TitleStyle.margin = new RectOffset();
            TitleStyle.padding = new RectOffset(3, 3, 3, 3);
            TitleStyle.normal.textColor = Color.white;
            TitleStyle.fontSize = 13;
            TitleStyle.fontStyle = FontStyle.Bold;
            TitleStyle.stretchWidth = true;

            AreaStyle = new GUIStyle(HighLogic.Skin.box);
            AreaStyle.margin = new RectOffset();
            AreaStyle.padding = new RectOffset(5, 5, 5, 5);

            LabelStyle = new GUIStyle(HighLogic.Skin.label);
            LabelStyle.normal.textColor = Color.white;
            LabelStyle.margin = new RectOffset();
            LabelStyle.padding = new RectOffset(3, 3, 3, 3);
            LabelStyle.alignment = TextAnchor.MiddleCenter;
            LabelStyle.fontSize = 12;
            LabelStyle.fontStyle = FontStyle.Bold;
            LabelStyle.stretchWidth = true;
        }

        #endregion

        #region Update and Draw

        public void Update()
        {
            foreach (Readout readout in _readouts)
                readout.Update();
        }

        public void Draw()
        {
            GUILayout.Label(_title.ToUpper(), TitleStyle);
            GUILayout.BeginVertical(AreaStyle);
            if (_readouts.Count > 0)
            {
                foreach (Readout readout in _readouts)
                    readout.Draw();
            }
            else
            {
                GUILayout.BeginHorizontal(GUILayout.Width(Readout.NameWidth + Readout.DataWidth));
                GUILayout.Label("No readouts installed!", LabelStyle);
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
        }

        #endregion

        #region Save and Load

        // Saves the settings associated with this section.
        public void Save()
        {
            List<string> readoutNames = new List<string>();

            foreach (Readout readout in _readouts)
                readoutNames.Add(readout.Name);

            try
            {
                SettingList list = new SettingList();
                list.AddSetting("visible", _visible);
                list.AddSetting("categories", _categories);
                list.AddSetting("readouts", readoutNames);
                SettingList.SaveToFile(EngineerGlobals.AssemblyPath + "Settings/Sections/" + _title, list);

                MonoBehaviour.print("[KerbalEngineer/FlightSection/" + _title + "]: Successfully saved settings.");
            }
            catch { MonoBehaviour.print("[KerbalEngineer/FlightSection/" + _title + "]: Failed to save settings."); }
        }

        // Loads the settings associated with this section.
        public void Load()
        {
            try
            {
                SettingList list = SettingList.CreateFromFile(EngineerGlobals.AssemblyPath + "Settings/Sections/" + _title);
                _visible = (bool)list.GetSetting("visible", _visible);
                _categories = list.GetSetting("categories", _categories) as List<ReadoutCategory>;

                _readouts.Clear();
                List<string> readoutNames = list.GetSetting("readouts", new List<string>()) as List<string>;
                foreach (string name in readoutNames)
                    Readouts.Add(ReadoutList.Instance.GetReadout(name));

                MonoBehaviour.print("[KerbalEngineer/FlightSection/" + _title + "]: Successfully loaded settings.");
            }
            catch { MonoBehaviour.print("[KerbalEngineer/FlightSection/" + _title + "]: Failed to load settings."); }
        }

        #endregion
    }
}
