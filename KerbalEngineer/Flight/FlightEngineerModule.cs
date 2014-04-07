// Project:	KerbalEngineer
// Author:	CYBUTEK
// License:	Attribution-NonCommercial-ShareAlike 3.0 Unported

#region Using Directives

using System.Linq;

using KerbalEngineer.Flight.Readouts;

#endregion

namespace KerbalEngineer.Flight
{
    /// <summary>
    ///     Module that can be attached to parts, giving them FlightEngineerCore management.
    /// </summary>
    public sealed class FlightEngineerModule : PartModule
    {
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