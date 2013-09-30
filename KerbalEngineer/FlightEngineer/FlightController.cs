// Name:    Kerbal Engineer Redux
// Author:  CYBUTEK
// License: Attribution-NonCommercial-ShareAlike 3.0 Unported

using System.IO;
using UnityEngine;

namespace KerbalEngineer.FlightEngineer
{
    public class FlightController
    {
        #region Instance

        private static FlightController _instance;
        /// <summary>
        /// Gets the current instance of the FlightButton object.
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

        private Rect _windowPosition = new Rect(Screen.width / 2 + 150f, 0f, 200f, 0f);
        private Rect _buttonPosition = new Rect(Screen.width / 2f + 200f, 0f, 100f, 17f);
        private GUIStyle _windowStyle, _buttonStyle;
        private Texture2D _windowTexture = new Texture2D(40, 40, TextureFormat.RGBA32, false);
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

        #region Initialisation

        private FlightController()
        {
            // Load textures directly from the PNG files. (Would of used GameDatabase but it compresses them so it looks shit!)
            _windowTexture.LoadImage(File.ReadAllBytes(EngineerGlobals.AssemblyPath + "GUI/FlightButton/Window.png"));
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
            _windowStyle.focused.background = _windowStyle.normal.background = _windowTexture;
            _windowStyle.margin = new RectOffset();
            _windowStyle.padding = new RectOffset();

            _buttonStyle = new GUIStyle(HighLogic.Skin.button);
            _buttonStyle.normal.textColor = Color.white;
        }

        #endregion

        #region Update and Drawing

        public void Update()
        {
            // Controls the sliding animation.
            if (_open && _openAmount < 1f) // Opening
            {
                _openAmount += (10f * _openAmount + 0.5f) * Time.deltaTime;

                if (_openAmount > 1f)
                    _openAmount = 1f;
            }
            else if (!_open && _openAmount > 0f) // Closing
            {
                _openAmount -= (10f * _openAmount + 0.5f) * Time.deltaTime;

                if (_openAmount < 0f)
                    _openAmount = 0f;
            }

            // Set the sliding positions.
            _windowPosition.y = -_windowPosition.height * (1f - _openAmount);
            _buttonPosition.y = _windowPosition.y + _windowPosition.height;
        }

        public void Draw()
        {
            if (!_hasInitStyles) InitialiseStyles();

            DrawButton();

            if (_windowPosition.y + _windowPosition.height > 0f || _windowPosition.height == 0f)
                _windowPosition = GUILayout.Window(_windowID, _windowPosition, DrawWindow, string.Empty, _windowStyle);
        }

        private void DrawWindow(int windowID)
        {
            GUILayout.Toggle(false, "Test", _buttonStyle);
        }

        private void DrawButton()
        {
            if (_clicked) // Button has been clicked whilst being hovered.
            {
                if (_open)
                    GUI.DrawTexture(_buttonPosition, _openDown);
                else
                    GUI.DrawTexture(_buttonPosition, _closedDown);

                if (_buttonPosition.Contains(Event.current.mousePosition)) // Mouse is hovering over the button.
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
                if (_buttonPosition.Contains(Event.current.mousePosition)) // Mouse is hovering over the button.
                {
                    // If the left mouse button has just been pressed, see the button as being clicked.
                    if (!_clicked && (Mouse.Left.GetButtonDown())) _clicked = true;

                    if (_clicked) // The button has just been clicked.
                    {
                        if (_open)
                            GUI.DrawTexture(_buttonPosition, _openDown);
                        else
                            GUI.DrawTexture(_buttonPosition, _closedDown);
                    }
                    else if (!Mouse.Left.GetButton()) // Mouse button is not down and is just hovering.
                    {
                        if (_open)
                            GUI.DrawTexture(_buttonPosition, _openHover);
                        else
                            GUI.DrawTexture(_buttonPosition, _closedHover);
                    }
                    else // Mouse button is down but no click was registered over the button.
                    {
                        if (_open)
                            GUI.DrawTexture(_buttonPosition, _openNormal);
                        else
                            GUI.DrawTexture(_buttonPosition, _closedNormal);
                    }
                }
                else // The mouse is not being hovered.
                {
                    if (_open)
                        GUI.DrawTexture(_buttonPosition, _openNormal);
                    else
                        GUI.DrawTexture(_buttonPosition, _closedNormal);
                }
            }

            // Check for an unclick event whilst the mouse is not hovering.
            if (_clicked && (Mouse.Left.GetButtonUp())) _clicked = false;
        }

        #endregion
    }
}
