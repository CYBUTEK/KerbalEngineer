// Name:    Kerbal Engineer Redux
// Author:  CYBUTEK
// License: Attribution-NonCommercial-ShareAlike 3.0 Unported

using KerbalEngineer.Extensions;
using UnityEngine;

namespace KerbalEngineer.FlightEngineer
{
    public class FlightEngineer : PartModule
    {
        #region Initialisation

        public void Start()
        {
            if (HighLogic.LoadedSceneIsFlight)
                RenderingManager.AddToPostDrawQueue(0, OnDraw);
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
                FlightController.Instance.Draw();
            }
        }

        #endregion
    }
}
