// Project:	KerbalEngineer
// Author:	CYBUTEK
// License:	Attribution-NonCommercial-ShareAlike 3.0 Unported

#region Using Directives

using KerbalEngineer.Extensions;

#endregion

namespace KerbalEngineer.FlightEngineer
{
    public class FlightEngineer : PartModule
    {
        #region Initialisation

        public void Start()
        {
            if (HighLogic.LoadedSceneIsFlight)
            {
                RenderingManager.AddToPostDrawQueue(0, this.OnDraw);
            }
        }

        #endregion

        #region Update and Drawing

        public void Update()
        {
            if (HighLogic.LoadedSceneIsFlight && FlightGlobals.ActiveVessel == this.vessel && this.part.IsPrimary(this.vessel.parts, this))
            {
                SectionList.Instance.Update();
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

        /// <summary>
        ///     Saves the settings when the module is told to save.
        /// </summary>
        public override void OnSave(ConfigNode node)
        {
            if (!HighLogic.LoadedSceneIsFlight)
            {
                return;
            }

            try
            {
                SectionList.Instance.Save();
                FlightController.Instance.Save();
                FlightDisplay.Instance.Save();
            }
            catch { }
        }

        /// <summary>
        ///     Loads the settings when the module is told to load.
        /// </summary>
        public override void OnLoad(ConfigNode node)
        {
            if (!HighLogic.LoadedSceneIsFlight)
            {
                return;
            }

            try
            {
                SectionList.Instance.Load();
                FlightController.Instance.Load();
                FlightDisplay.Instance.Load();
            }
            catch { }
        }

        #endregion
    }
}