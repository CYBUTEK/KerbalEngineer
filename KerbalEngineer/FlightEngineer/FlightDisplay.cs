// Name:    Kerbal Engineer Redux
// Author:  CYBUTEK
// License: Attribution-NonCommercial-ShareAlike 3.0 Unported

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

        private Rect _windowPosition = new Rect(Screen.width / 2f - 125f, 100f, 250f, 0f);
        private GUIStyle _windowStyle;
        private int _windowID = EngineerGlobals.GetNextWindowID();

        private bool _hasInitStyles = false;

        #endregion

        #region Initialisation

        private FlightDisplay()
        {

        }

        private void InitialiseStyles()
        {
            _hasInitStyles = true;

            _windowStyle = new GUIStyle(HighLogic.Skin.window);
            _windowStyle.margin = new RectOffset();
            _windowStyle.padding = new RectOffset(3, 3, 3, 3);
        }

        #endregion

        #region Update and Drawing

        public void Update()
        {
        }

        public void Draw()
        {
            if (!_hasInitStyles) InitialiseStyles();

            _windowPosition = GUILayout.Window(_windowID, _windowPosition, Window, string.Empty, _windowStyle);
        }

        private void Window(int windowID)
        {
            Orbital.Apoapsis.Instance.Draw();

            GUI.DragWindow();
        }

        #endregion
    }
}
