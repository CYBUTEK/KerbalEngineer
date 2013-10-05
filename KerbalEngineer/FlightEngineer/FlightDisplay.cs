// Name:    Kerbal Engineer Redux
// Author:  CYBUTEK
// License: Attribution-NonCommercial-ShareAlike 3.0 Unported

using KerbalEngineer.Extensions;
using KerbalEngineer.Settings;
using UnityEngine;

namespace KerbalEngineer.FlightEngineer
{
    public class FlightDisplay
    {
        #region Instance

        private static FlightDisplay _instance;
        /// <summary>
        /// Gets the current instance of the flight display.
        /// </summary>
        public static FlightDisplay Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new FlightDisplay();

                return _instance;
            }
        }

        #endregion

        #region Fields

        private GUIStyle _windowStyle;
        private int _windowID = EngineerGlobals.GetNextWindowID();

        private bool _hasInitStyles = false;

        #endregion

        #region Properties

        private Rect _windowPosition = new Rect(Screen.width / 2f - 125f, 100f, 250f, 0f);
        /// <summary>
        /// Gets and sets the window position.
        /// </summary>
        public Rect WindowPosition
        {
            get { return _windowPosition; }
            set { _windowPosition = value; }
        }

        private bool _requireResize = false;
        /// <summary>
        /// Gets and sets whether the display requires a resize.
        /// </summary>
        public bool RequireResize
        {
            get { return _requireResize; }
            set { _requireResize = value; }
        }

        #endregion

        #region Initialisation

        private void InitialiseStyles()
        {
            _hasInitStyles = true;

            _windowStyle = new GUIStyle(HighLogic.Skin.window);
            _windowStyle.margin = new RectOffset();
            _windowStyle.padding = new RectOffset(5, 5, 3, 5);
        }

        #endregion

        #region Update and Drawing

        public void Update()
        {
            // Update all visible fixed sections.
            foreach (Section section in SectionList.Instance.FixedSections)
                if (section.Visible)
                    section.Update();

            // Update all visible user sections.
            foreach (Section section in SectionList.Instance.UserSections)
                if (section.Visible)
                    section.Update();

            Surface.AtmosphericDetails.Instance.Update();
        }

        public void Draw()
        {
            if (!_hasInitStyles) InitialiseStyles();

            // Handle window resizing if something has changed within the GUI.
            if (_requireResize)
            {
                _requireResize = false;
                _windowPosition.width = 0f;
                _windowPosition.height = 0f;
            }

            if (SectionList.Instance.HasVisibleSections())
                _windowPosition = GUILayout.Window(_windowID, _windowPosition, Window, string.Empty, _windowStyle).ClampToScreen();
        }

        private void Window(int windowID)
        {
            // Draw all visible fixed sections.
            foreach (Section section in SectionList.Instance.FixedSections)
                if (section.Visible)
                    section.Draw();

            // Draw all visible user sections.
            foreach (Section section in SectionList.Instance.UserSections)
                if (section.Visible)
                    section.Draw();

            GUI.DragWindow();
        }

        #endregion

        #region Save and Load

        // Saves the settings associated with the flight display.
        public void Save()
        {
            try
            {
                SettingList list = new SettingList();
                list.AddSetting("x", _windowPosition.x);
                list.AddSetting("y", _windowPosition.y);
                SettingList.SaveToFile(EngineerGlobals.AssemblyPath + "Settings/FlightDisplay", list);
                
                MonoBehaviour.print("[KerbalEngineer/FlightDisplay]: Successfully saved settings.");
            }
            catch { MonoBehaviour.print("[KerbalEngineer/FlightDisplay]: Failed to save settings."); }
        }

        // Loads the settings associated with the flight display.
        public void Load()
        {
            try
            {
                SettingList list = SettingList.CreateFromFile(EngineerGlobals.AssemblyPath + "Settings/FlightDisplay");
                _windowPosition.x = (float)list.GetSetting("x", _windowPosition.x);
                _windowPosition.y = (float)list.GetSetting("y", _windowPosition.y);

                MonoBehaviour.print("[KerbalEngineer/FlightDisplay]: Successfully loaded settings.");
            }
            catch { MonoBehaviour.print("[KerbalEngineer/FlightDisplay]: Failed to load settings."); }
        }

        #endregion
    }
}
