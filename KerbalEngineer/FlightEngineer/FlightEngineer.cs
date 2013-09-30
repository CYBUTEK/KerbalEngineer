// Name:    Kerbal Engineer Redux
// Author:  CYBUTEK
// License: Attribution-NonCommercial-ShareAlike 3.0 Unported

using System.IO;
using KerbalEngineer.Extensions;
using UnityEngine;

namespace KerbalEngineer.FlightEngineer
{
    public class FlightEngineer : PartModule
    {
        #region Fields

        private bool _hasInitStyles = false;

        #endregion

        #region Initialisation

        public void Start()
        {
            if (HighLogic.LoadedSceneIsFlight)
                RenderingManager.AddToPostDrawQueue(0, OnDraw);
        }

        private void InitialiseStyles()
        {
            _hasInitStyles = true;
        }

        #endregion

        #region Update and Drawing

        public void Update()
        {
            if (FlightGlobals.ActiveVessel == this.vessel && this.part.IsPrimary(this.vessel.parts, this))
            {
                FlightController.Instance.Update();
            }
        }

        public void OnDraw()
        {
            if (FlightGlobals.ActiveVessel == this.vessel && this.part.IsPrimary(this.vessel.parts, this))
            {
                if (!_hasInitStyles) InitialiseStyles();

                FlightController.Instance.Draw();
            }
        }

        #endregion
    }
}
