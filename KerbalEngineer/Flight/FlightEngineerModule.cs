// Project:	KerbalEngineer
// Author:	CYBUTEK
// License:	Attribution-NonCommercial-ShareAlike 3.0 Unported

#region Using Directives

using System.Linq;

using KerbalEngineer.Simulation;

#endregion

namespace KerbalEngineer.Flight
{
    /// <summary>
    ///     Module that can be attached to parts, giving them FlightEngineerCore management.
    /// </summary>
    public sealed class FlightEngineerModule : PartModule
    {
        #region KSP Fields

        /// <summary>
        ///     The minimum time in ms from the start of one simulation to the start of the next.
        /// </summary>
        [KSPField(isPersistant = true, guiActive = true, guiActiveEditor = false, guiName = "Sim time limit"),
         UI_FloatRange(minValue = 0.0f, maxValue = 1000.0f, stepIncrement = 10.0f, scene = UI_Scene.Flight)] public float minFESimTime = 200.0f;

        #endregion

        #region Fields

        /// <summary>
        ///     Contains the current FlightEngineerCore through the lifespan of this part.
        /// </summary>
        private FlightEngineerCore flightEngineerCore;

        #endregion

        #region Updating

        /// <summary>
        ///     Logic to create and destroy the FlightEngineerCore.
        /// </summary>
        private void Update()
        {
            if (!HighLogic.LoadedSceneIsFlight)
            {
                return;
            }

            SimManager.minSimTime = (long)this.minFESimTime;

            if (this.vessel == FlightGlobals.ActiveVessel)
            {
                // Checks for an existing instance of FlightEngineerCore, and if this part is the first part containing FlightEngineerModule within the vessel.
                if (this.flightEngineerCore == null && this.part == this.vessel.parts.FirstOrDefault(p => p.Modules.Contains("FlightEngineerModule")))
                {
                    this.flightEngineerCore = this.gameObject.AddComponent<FlightEngineerCore>();
                }
            }
            else if (this.flightEngineerCore != null)
            {
                // Using DestroyImmediate to force early destruction and keep saving/loading in synch when switching vessels.
                DestroyImmediate(this.flightEngineerCore);
            }
        }

        #endregion

        #region Destruction

        /// <summary>
        ///     Force the destruction of the FlightEngineerCore on part destruction.
        /// </summary>
        private void OnDestroy()
        {
            if (this.flightEngineerCore != null)
            {
                DestroyImmediate(this.flightEngineerCore);
            }
        }

        #endregion
    }
}