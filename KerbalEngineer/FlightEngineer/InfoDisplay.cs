// Name:    Kerbal Engineer Redux
// Author:  CYBUTEK
// License: Attribution-NonCommercial-ShareAlike 3.0 Unported

using KerbalEngineer.Extensions;
using UnityEngine;

namespace KerbalEngineer.FlightEngineer
{
    public class InfoDisplay : MonoBehaviour
    {
        #region Instance

        private static InfoDisplay _instance;
        /// <summary>
        /// Gets the current instance of the InfoDisplay object.
        /// </summary>
        public static InfoDisplay Instance
        {
            get
            {
                if (_instance == null)
                    _instance = HighLogic.fetch.gameObject.AddComponent<InfoDisplay>();

                return _instance;
            }
        }

        #endregion

        #region Fields

        private Rect _windowPosition = new Rect(Screen.width / 2 - 150f, Screen.height / 2 - 100f, 300f, 200f);
        private int _windowID = EngineerGlobals.GetNextWindowID();
        private GUIStyle _windowStyle, _infoStyle, _buttonStyle, _labelStyle;
        private Vector2 _scrollPosition = Vector2.zero;

        private bool _hasInitStyles = false;

        #endregion

        #region Properties

        private bool _visible = false;
        /// <summary>
        /// Gets and sets whether the display is visible.
        /// </summary>
        public bool Visible
        {
            get { return _visible; }
            set { _visible = value; }
        }


        private Readout _readout;
        /// <summary>
        /// Gets and sets the information to be displayed.
        /// </summary>
        public Readout Readout
        {
            get { return _readout; }
            set { _readout = value; }
        }

        #endregion

        #region Initialisation

        // Runs when the object is created.
        private void Start()
        {
            RenderingManager.AddToPostDrawQueue(0, Draw);
        }

        // Initialises the gui styles upon request.
        private void InitialiseStyles()
        {
            _hasInitStyles = true;

            _windowStyle = new GUIStyle(HighLogic.Skin.window);

            _infoStyle = new GUIStyle();
            _infoStyle.margin = new RectOffset(5, 5, 5, 5);

            _buttonStyle = new GUIStyle(HighLogic.Skin.button);
            _buttonStyle.normal.textColor = Color.white;
            _buttonStyle.fontSize = 11;
            _buttonStyle.fontStyle = FontStyle.Bold;
            _buttonStyle.stretchWidth = true;

            _labelStyle = new GUIStyle(HighLogic.Skin.label);
            _labelStyle.normal.textColor = Color.white;
            _labelStyle.alignment = TextAnchor.MiddleLeft;
            _labelStyle.fontSize = 12;
            _labelStyle.fontStyle = FontStyle.Bold;
            _labelStyle.stretchWidth = true;
        }

        #endregion

        #region Drawing

        // Runs when the object is called to draw.
        private void Draw()
        {
            if (_visible)
            {
                if (!_hasInitStyles) InitialiseStyles();

                _windowPosition = GUILayout.Window(_windowID, _windowPosition, Window, "READOUT INFORMATION", _windowStyle).ClampToScreen();
            }
        }

        private void Window(int windowID)
        {
            GUI.skin = HighLogic.Skin;
            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition, false, true);
            GUI.skin = null;

            if (_readout != null)
            {
                GUILayout.Label(_readout.Name, _labelStyle);

                GUILayout.BeginHorizontal(_infoStyle);
                GUILayout.Label(_readout.Description, _labelStyle);
                GUILayout.EndHorizontal();
            }

            GUILayout.EndScrollView();

            if (GUILayout.Button("CLOSE", _buttonStyle))
                _visible = false;

            GUI.DragWindow();
        }

        #endregion
    }
}
