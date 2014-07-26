// 
//     Kerbal Engineer Redux
// 
//     Copyright (C) 2014 CYBUTEK
// 
//     This program is free software: you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     This program is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General Public License for more details.
// 
//     You should have received a copy of the GNU General Public License
//     along with this program.  If not, see <http://www.gnu.org/licenses/>.
// 

#region Using Directives

using System;
using System.Linq;

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
            try
            {
                if (!HighLogic.LoadedSceneIsFlight || FlightEngineerPartless.IsPartless)
                {
                    return;
                }

                if (this.vessel == FlightGlobals.ActiveVessel)
                {
                    // Checks for an existing instance of FlightEngineerCore, and if this part is the first part containing FlightEngineerModule within the vessel.
                    if (flightEngineerCore == null && this.part == this.vessel.parts.FirstOrDefault(p => p.Modules.Contains("FlightEngineerModule")))
                    {
                        this.flightEngineerCore = this.gameObject.AddComponent<FlightEngineerCore>();
                    }
                }
                else if (flightEngineerCore != null)
                {
                    // Using DestroyImmediate to force early destruction and keep saving/loading in synch when switching vessels.
                    DestroyImmediate(flightEngineerCore);
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "FlightEngineerModule->Update");
            }
        }

        #endregion

        #region Destruction

        /// <summary>
        ///     Force the destruction of the FlightEngineerCore on part destruction.
        /// </summary>
        private void OnDestroy()
        {
            try
            {
                if (flightEngineerCore != null)
                {
                    DestroyImmediate(flightEngineerCore);
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "FlightEngineerModule->OnDestroy");
            }
        }

        #endregion
    }
}