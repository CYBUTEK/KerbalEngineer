// Name:    Kerbal Engineer Redux
// Author:  CYBUTEK
// License: Attribution-NonCommercial-ShareAlike 3.0 Unported

using System.Collections.Generic;
using System.IO;
using KerbalEngineer.Settings;
using UnityEngine;

namespace KerbalEngineer.FlightEngineer
{
    public class FlightController
    {
        #region Instance

        private static FlightController _instance;
        /// <summary>
        /// Gets the current instance of the flight controller.
        /// </summary>
        public static FlightController Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new FlightController();

                return _instance;
            }
        }

        #endregion

        #region Fields

        private Rect _windowPosition = new Rect(Screen.width / 2f + 150f, 0f, 200f, 0f);
        private Rect _handlePosition = new Rect(Screen.width / 2f + 200f, 0f, 100f, 17f);
        private GUIStyle _windowStyle, _buttonStyle;
        private Texture2D _closedNormal = new Texture2D(100, 17, TextureFormat.RGBA32, false);
        private Texture2D _closedHover = new Texture2D(100, 17, TextureFormat.RGBA32, false);
        private Texture2D _closedDown = new Texture2D(100, 17, TextureFormat.RGBA32, false);
        private Texture2D _openNormal = new Texture2D(100, 17, TextureFormat.RGBA32, false);
        private Texture2D _openHover = new Texture2D(100, 17, TextureFormat.RGBA32, false);
        private Texture2D _openDown = new Texture2D(100, 17, TextureFormat.RGBA32, false);
        private int _windowID = EngineerGlobals.GetNextWindowID();

        private bool _hasInitStyles = false;
        private bool _clicked = false;
        private bool _open = false;
        private float _openAmount = 0f;

        #endregion

        #region Properties

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

        private FlightController()
        {
            // Load textures directly from the PNG files. (Would of used GameDatabase but it compresses them so it looks shit!)
            _closedNormal.LoadImage(File.ReadAllBytes(EngineerGlobals.AssemblyPath + "GUI/FlightButton/ClosedNormal.png"));
            _closedHover.LoadImage(File.ReadAllBytes(EngineerGlobals.AssemblyPath + "GUI/FlightButton/ClosedHover.png"));
            _closedDown.LoadImage(File.ReadAllBytes(EngineerGlobals.AssemblyPath + "GUI/FlightButton/ClosedDown.png"));
            _openNormal.LoadImage(File.ReadAllBytes(EngineerGlobals.AssemblyPath + "GUI/FlightButton/OpenNormal.png"));
            _openHover.LoadImage(File.ReadAllBytes(EngineerGlobals.AssemblyPath + "GUI/FlightButton/OpenHover.png"));
            _openDown.LoadImage(File.ReadAllBytes(EngineerGlobals.AssemblyPath + "GUI/FlightButton/OpenDown.png"));
        }

        private void InitialiseStyles()
        {
            _hasInitStyles = true;

            _windowStyle = new GUIStyle(HighLogic.Skin.window);
            _windowStyle.margin = new RectOffset();
            _windowStyle.padding = new RectOffset(3, 3, 3, 3);
            _windowStyle.fixedWidth = _windowPosition.width;

            _buttonStyle = new GUIStyle(HighLogic.Skin.button);
            _buttonStyle.normal.textColor = Color.white;
            _buttonStyle.margin = new RectOffset();
            _buttonStyle.fixedHeight = 20f;
            _buttonStyle.fontSize = 11;
            _buttonStyle.fontStyle = FontStyle.Bold;
        }

        #endregion

        #region Update and Drawing

        public void Update()
        {
            // Controls the sliding animation.
            if (_open && _openAmount < 1f) // Opening
            {
                _openAmount += ((10f * (1f - _openAmount)) + 0.5f) * Time.deltaTime;

                if (_openAmount > 1f)
                    _openAmount = 1f;
            }
            else if (!_open && _openAmount > 0f) // Closing
            {
                _openAmount -= ((10f * _openAmount) + 0.5f) * Time.deltaTime;

                if (_openAmount < 0f)
                    _openAmount = 0f;
            }

            // Set the sliding positions.
            _windowPosition.y = -_windowPosition.height * (1f - _openAmount);
            _handlePosition.y = _windowPosition.y + _windowPosition.height;
        }

        public void Draw()
        {
            if (!_hasInitStyles) InitialiseStyles();

            // Handle window resizing if something has changed within the GUI.
            if (_requireResize)
            {
                _requireResize = false;
                _windowPosition.height = 0f;
            }

            DrawButton();

            if (_windowPosition.y + _windowPosition.height > 0f || _windowPosition.height == 0f)
                _windowPosition = GUILayout.Window(_windowID, _windowPosition, Window, string.Empty, _windowStyle);
        }

        private void Window(int windowID)
        {
            GUILayout.BeginHorizontal();

            // Draw fixed section display toggles.
            GUILayout.BeginVertical();
            foreach (Section section in SectionList.Instance.FixedSections)
                section.Visible = GUILayout.Toggle(section.Visible, section.Title.ToUpper(), _buttonStyle);
            GUILayout.EndVertical();

            // Draw fixed section edit toggles.
            GUILayout.BeginVertical(GUILayout.Width(50f));
            foreach (Section section in SectionList.Instance.FixedSections)
                section.EditDisplay.Visible = GUILayout.Toggle(section.EditDisplay.Visible, "EDIT", _buttonStyle);
            GUILayout.EndVertical();

            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();

            // Draw user section display toggles.
            GUILayout.BeginVertical();
            foreach (Section section in SectionList.Instance.UserSections)
            {
                section.Visible = GUILayout.Toggle(section.Visible, section.Title.ToUpper(), _buttonStyle);
            }
            GUILayout.EndVertical();

            // Draw user section edit toggles.
            GUILayout.BeginVertical(GUILayout.Width(50f));
            foreach (Section section in SectionList.Instance.UserSections)
                section.EditDisplay.Visible = GUILayout.Toggle(section.EditDisplay.Visible, "EDIT", _buttonStyle);
            GUILayout.EndVertical();

            GUILayout.EndHorizontal();

            if (GUILayout.Button("NEW USER SECTION", _buttonStyle))
                SectionList.Instance.UserSections.Add(new Section(true));
        }

        private void DrawButton()
        {
            if (_clicked) // Button has been clicked whilst being hovered.
            {
                if (_open)
                    GUI.DrawTexture(_handlePosition, _openDown);
                else
                    GUI.DrawTexture(_handlePosition, _closedDown);

                if (_handlePosition.Contains(Event.current.mousePosition)) // Mouse is hovering over the button.
                {
                    if (Mouse.Left.GetButtonUp()) // The mouse up event has been triggered whilst over the button.
                    {
                        _clicked = false;
                        _open = !_open;
                    }
                }
            }
            else // The button is not registering as being clicked.
            {
                if (_handlePosition.Contains(Event.current.mousePosition)) // Mouse is hovering over the button.
                {
                    // If the left mouse button has just been pressed, see the button as being clicked.
                    if (!_clicked && (Mouse.Left.GetButtonDown())) _clicked = true;

                    if (_clicked) // The button has just been clicked.
                    {
                        if (_open)
                            GUI.DrawTexture(_handlePosition, _openDown);
                        else
                            GUI.DrawTexture(_handlePosition, _closedDown);
                    }
                    else if (!Mouse.Left.GetButton()) // Mouse button is not down and is just hovering.
                    {
                        if (_open)
                            GUI.DrawTexture(_handlePosition, _openHover);
                        else
                            GUI.DrawTexture(_handlePosition, _closedHover);
                    }
                    else // Mouse button is down but no click was registered over the button.
                    {
                        if (_open)
                            GUI.DrawTexture(_handlePosition, _openNormal);
                        else
                            GUI.DrawTexture(_handlePosition, _closedNormal);
                    }
                }
                else // The mouse is not being hovered.
                {
                    if (_open)
                        GUI.DrawTexture(_handlePosition, _openNormal);
                    else
                        GUI.DrawTexture(_handlePosition, _closedNormal);
                }
            }

            // Check for an unclick event whilst the mouse is not hovering.
            if (_clicked && (Mouse.Left.GetButtonUp())) _clicked = false;
        }

        #endregion

        #region Save and Load

        // Saves the settings associated with the flight controller.
        public void Save()
        {
            try
            {
                SettingList list = new SettingList();
                list.AddSetting("open", _open);
                SettingList.SaveToFile(EngineerGlobals.AssemblyPath + "Settings/FlightController", list);

                MonoBehaviour.print("[KerbalEngineer/FlightController]: Successfully saved settings.");
            }
            catch { MonoBehaviour.print("[KerbalEngineer/FlightController]: Failed to save settings."); }
        }

        // Loads the settings associated with the flight controller.
        public void Load()
        {
            try
            {
                SettingList list = SettingList.CreateFromFile(EngineerGlobals.AssemblyPath + "Settings/FlightController");
                _open = (bool)list.GetSetting("open", _open);

                MonoBehaviour.print("[KerbalEngineer/FlightController]: Successfully loaded settings.");
            }
            catch { MonoBehaviour.print("[KerbalEngineer/FlightController]: Failed to load settings."); }
        }

        #endregion
    }
}
