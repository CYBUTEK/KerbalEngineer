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

        private bool _hasInitStyles = false;

        #endregion

        #region Initialisation

        private FlightDisplay()
        {

        }

        private void InitialiseStyles()
        {
            _hasInitStyles = true;
        }

        #endregion

        #region Update and Drawing

        public void Update()
        {
        }

        public void Draw()
        {
            if (!_hasInitStyles) InitialiseStyles();


        }

        #endregion
    }
}
