// Name:    Kerbal Engineer Redux
// Author:  CYBUTEK
// License: Attribution-NonCommercial-ShareAlike 3.0 Unported

using KerbalEngineer.Extensions;
using UnityEngine;

namespace KerbalEngineer.FlightEngineer
{
    public class SectionWindow : MonoBehaviour
    {
        #region Fields

        private Rect _position = new Rect(Screen.width / 2f - 125f, 100f, 250f, 0f);
        private GUIStyle _windowStyle;
        private int _windowID = EngineerGlobals.GetNextWindowID();

        private bool _hasInitStyles = false;

        #endregion

        #region Properties

        /// <summary>
        /// Gets and sets the X position of the window.
        /// </summary>
        public float PosX
        {
            get { return _position.x; }
            set { _position.x = value; }
        }

        /// <summary>
        /// Gets and sets the Y position of the window.
        /// </summary>
        public float PosY
        {
            get { return _position.y; }
            set { _position.y = value; }
        }

        private bool _visible = false;
        /// <summary>
        /// Gets and sets the visibility of the window.
        /// </summary>
        public bool Visible
        {
            get { return _visible; }
            set { _visible = value; }
        }

        private Section _section;
        /// <summary>
        /// Gets and sets the parent section.
        /// </summary>
        public Section Section
        {
            get { return _section; }
            set { _section = value; }
        }

        private bool _requireResize = false;
        /// <summary>
        /// Gets and sets whether the window requires a resize.
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
            _windowStyle.fixedWidth = 270f;
        }

        #endregion

        #region Update and Drawing

        private void Update()
        {
        }

        public void Draw()
        {
            if (_section.Visible && _visible)
            {
                if (!_hasInitStyles) InitialiseStyles();

                // Handle window resizing if something has changed within the GUI.
                if (_requireResize)
                {
                    _requireResize = false;
                    _position.width = 0f;
                    _position.height = 0f;
                }

                _position = GUILayout.Window(_windowID, _position, Window, string.Empty, _windowStyle).ClampToScreen();
            }
        }

        private void Window(int windowID)
        {
            _section.Draw();
            GUI.DragWindow();
        }

        #endregion
    }
}
