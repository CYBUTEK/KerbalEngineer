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
using System.Reflection;

using KerbalEngineer.Extensions;

#endregion

namespace KerbalEngineer.Flight.Readouts.Surface
{
    using UnityEngine;

    public class AtmosphericProcessor : IUpdatable, IUpdateRequest
    {
        #region Instance

        #region Fields

        private static readonly AtmosphericProcessor instance = new AtmosphericProcessor();

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the current instance of the atmospheric processor.
        /// </summary>
        public static AtmosphericProcessor Instance
        {
            get
            {
                return instance;
            }
        }

        #endregion

        #endregion

        #region Fields

        private MethodInfo farTerminalVelocity;
        private bool hasCheckedAeroMods;

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the deceleration caused by drag.
        /// </summary>
        public static double Deceleration { get; private set; }

        /// <summary>
        ///     Gets the difference between current velocity and terminal velocity.
        /// </summary>
        public static double Efficiency { get; private set; }

        /// <summary>
        ///     Gets whether FAR is installed.
        /// </summary>
        public static bool FarInstalled { get; private set; }

        /// <summary>
        ///     Gets whether NEAR is installed.
        /// </summary>
        public static bool NearInstalled { get; private set; }

        /// <summary>
        ///     Gets whether the details are ready to be shown.
        /// </summary>
        public static bool ShowDetails { get; private set; }

        /// <summary>
        ///     Gets the terminal velocity of the active vessel.
        /// </summary>
        public static double TerminalVelocity { get; private set; }

        #endregion

        #region IUpdatable Members

        /// <summary>
        ///     Updates the details by recalculating if requested.
        /// </summary>
        public void Update()
        {
            try
            {
                if (!this.hasCheckedAeroMods)
                {
                    this.CheckAeroMods();
                }

                if (FlightGlobals.ActiveVessel.atmDensity < double.Epsilon || NearInstalled)
                {
                    ShowDetails = false;
                    return;
                }

                ShowDetails = true;

                if (FarInstalled)
                {
                    TerminalVelocity = (double)this.farTerminalVelocity.Invoke(null, null);
                }
                else
                {
                    var m = FlightGlobals.ActiveVessel.parts.Sum(part => part.GetWetMass()) * 1000.0;
                    var g = FlightGlobals.getGeeForceAtPosition(FlightGlobals.ship_position).magnitude;
                    var a = FlightGlobals.ActiveVessel.parts.Sum(part => part.DragCubes.AreaDrag) * PhysicsGlobals.DragCubeMultiplier;
                    var p = FlightGlobals.ActiveVessel.atmDensity;
                    var c = PhysicsGlobals.DragMultiplier;

                    TerminalVelocity = Math.Sqrt((2.0 * m * g) / (p * a * c));
                }

                Efficiency = FlightGlobals.ship_srfSpeed / TerminalVelocity;
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "AtmosphericProcessor->Update");
            }
        }

        #endregion

        #region IUpdateRequest Members

        /// <summary>
        ///     Gets and sets whether the updatable object should be updated.
        /// </summary>
        public bool UpdateRequested { get; set; }

        #endregion

        #region Methods: public

        /// <summary>
        ///     Request an update to calculate the details.
        /// </summary>
        public static void RequestUpdate()
        {
            instance.UpdateRequested = true;
        }

        #endregion

        #region Private Methods

        private void CheckAeroMods()
        {
            try
            {
                this.hasCheckedAeroMods = true;

                foreach (var loadedAssembly in AssemblyLoader.loadedAssemblies)
                {
                    switch (loadedAssembly.name)
                    {
                        case "FerramAerospaceResearch":
                            this.farTerminalVelocity = loadedAssembly.assembly.GetType("ferram4.FARAPI").GetMethod("GetActiveControlSys_TermVel");
                            FarInstalled = true;
                            Logger.Log("FAR detected!");
                            break;

                        case "NEAR":
                            NearInstalled = true;
                            Logger.Log("NEAR detected! Turning off atmospheric details!");
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "AtmosphericProcessor->CheckAeroMods");
            }
        }

        #endregion
    }
}