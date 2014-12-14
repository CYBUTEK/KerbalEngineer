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



#endregion

namespace KerbalEngineer.Flight.Readouts.Vessel
{
    #region Using Directives

    using System;

    #endregion

    public class SuicideBurnProcessor : IUpdatable, IUpdateRequest
    {
        #region Fields

        private static readonly SuicideBurnProcessor instance = new SuicideBurnProcessor();
        private double acceleration;
        private double gravity;
        private double radarAltitude;

        #endregion

        #region Properties

        public static double Altitude { get; private set; }

        public static double DeltaV { get; private set; }

        public static double Distance { get; private set; }

        public static SuicideBurnProcessor Instance
        {
            get { return instance; }
        }

        public static bool ShowDetails { get; set; }

        public bool UpdateRequested { get; set; }

        #endregion

        #region Methods

        public static void RequestUpdate()
        {
            instance.UpdateRequested = true;
            SimulationProcessor.RequestUpdate();
        }

        public static void Reset()
        {
            FlightEngineerCore.Instance.AddUpdatable(SimulationProcessor.Instance);
            FlightEngineerCore.Instance.AddUpdatable(instance);
        }

        public void Update()
        {
            if (FlightGlobals.ship_orbit.PeA >= 0.0 || !SimulationProcessor.ShowDetails)
            {
                ShowDetails = false;
                return;
            }

            this.gravity = FlightGlobals.currentMainBody.gravParameter / Math.Pow(FlightGlobals.currentMainBody.Radius, 2.0);
            this.acceleration = SimulationProcessor.LastStage.thrust / SimulationProcessor.LastStage.totalMass;
            this.radarAltitude = FlightGlobals.ActiveVessel.terrainAltitude > 0.0
                ? FlightGlobals.ship_altitude - FlightGlobals.ActiveVessel.terrainAltitude
                : FlightGlobals.ship_altitude;

            DeltaV = Math.Sqrt((2 * this.gravity * this.radarAltitude) + Math.Pow(FlightGlobals.ship_verticalSpeed, 2.0));
            Altitude = Math.Pow(DeltaV, 2.0) / (2.0 * this.acceleration);
            Distance = this.radarAltitude - Altitude;

            ShowDetails = !Double.IsInfinity(Distance);
        }

        #endregion
    }
}