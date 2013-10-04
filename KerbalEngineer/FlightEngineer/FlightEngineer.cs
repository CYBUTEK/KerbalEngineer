// Name:    Kerbal Engineer Redux
// Author:  CYBUTEK
// License: Attribution-NonCommercial-ShareAlike 3.0 Unported

using KerbalEngineer.Extensions;
using KerbalEngineer.Settings;
using UnityEngine;

namespace KerbalEngineer.FlightEngineer
{
    public class FlightEngineer : PartModule
    {
        #region Initialisation

        public void Start()
        {
            if (HighLogic.LoadedSceneIsFlight)
            {
                Load();
                RenderingManager.AddToPostDrawQueue(0, OnDraw);
            }
        }

        #endregion

        #region Update and Drawing

        public void Update()
        {
            if (HighLogic.LoadedSceneIsFlight && FlightGlobals.ActiveVessel == this.vessel && this.part.IsPrimary(this.vessel.parts, this))
            {
                FlightController.Instance.Update();
                FlightDisplay.Instance.Update();
            }
        }

        public void OnDraw()
        {
            if (HighLogic.LoadedSceneIsFlight && FlightGlobals.ActiveVessel == this.vessel && this.part.IsPrimary(this.vessel.parts, this))
            {
                FlightController.Instance.Draw();
                FlightDisplay.Instance.Draw();
            }
        }

        #endregion

        #region Save and Load

        // Runs when the part module is asked to save.
        public override void OnSave(ConfigNode node)
        {
            if (HighLogic.LoadedSceneIsFlight && FlightGlobals.ActiveVessel == this.vessel && this.part.IsPrimary(this.vessel.parts, this))
            {
                SectionList.Instance.Save();
                FlightController.Instance.Save();
                FlightDisplay.Instance.Save();
            }
        }

        // Runs when the part module is asked to load.
        public void Load()
        {
            if (HighLogic.LoadedSceneIsFlight && FlightGlobals.ActiveVessel == this.vessel && this.part.IsPrimary(this.vessel.parts, this))
            {
                SectionList.Instance.Load();
                FlightController.Instance.Load();
                FlightDisplay.Instance.Load();
            }
        }

        #endregion
    }
}
