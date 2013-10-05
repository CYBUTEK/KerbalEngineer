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
                RenderingManager.AddToPostDrawQueue(0, OnDraw);
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

        // Saves the settings when the module is set to save.
        public override void OnSave(ConfigNode node)
        {
            try
            {
                if (HighLogic.LoadedSceneIsFlight)
                {
                    SectionList.Instance.Save();
                    FlightController.Instance.Save();
                    FlightDisplay.Instance.Save();
                }
            }
            catch { }
        }

        // Loads the settings when this object is created.
        public override void OnLoad(ConfigNode node)
        {
            try
            {
                if (HighLogic.LoadedSceneIsFlight)
                {
                    SectionList.Instance.Load();
                    FlightController.Instance.Load();
                    FlightDisplay.Instance.Load();
                }
            }
            catch { }
        }

        #endregion
    }
}
