//
//     Kerbal Engineer Redux
//
//     Copyright (C) 2017 CYBUTEK
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


using System;

namespace KerbalEngineer.Flight.Readouts.Surface
{

    public class SurfaceDistanceProcessor : IUpdatable, IUpdateRequest
    {
        /// <summary>
        ///     Gets the current instance of the rendezvous processor.
        /// </summary>
        public static SurfaceDistanceProcessor Instance
        {
            get
            {
                return instance;
            }
        }


        /// <summary>
        ///     Gets whether the details are ready to be shown.
        /// </summary>
        public static bool ShowDetails { get; private set; }


        /// <summary>
        ///     Gets and sets whether the updatable object should be updated.
        /// </summary>
        public bool UpdateRequested { get; set; }

        private static readonly SurfaceDistanceProcessor instance = new SurfaceDistanceProcessor();

        /// <summary>
        ///     Gets the great-circle distance from the current vessel to the target on the surface.
        /// </summary>
        public static double SurfaceDistanceToTarget { get; private set; }

        /// <summary>
        ///     Gets the initial heading on the great-circle from the origin position to the target position on the surface.
        /// </summary>
        public static double SurfaceHeadingToTarget { get; private set; }

        /// <summary>
        ///     Gets the great-circle distance from the current vessel to the waypoint position on the surface.
        /// </summary>
        public static double SurfaceDistanceToWaypoint { get; private set; }

        /// <summary>
        ///     Gets the initial heading on the great-circle current vessel to the waypoint position on the surface.
        /// </summary>
        public static double SurfaceHeadingToWaypoint { get; private set; }

        /// <summary>
        ///     Request and update to calculate the details.
        /// </summary>
        public static void RequestUpdate()
        {
            instance.UpdateRequested = true;
        }

        /// <summary>
        ///     Updates the details by recalculating if requested.
        /// </summary>
        public void Update()
        {
            // get vessel and navigation waypoints
            global::Vessel targetVessel = FlightGlobals.fetch?.VesselTarget?.GetVessel();
            FinePrint.Waypoint navigationWaypoint = FlightGlobals.ActiveVessel?.navigationWaypoint;

            // if neither a vessel nor a waypoint is targeted, no point in showing details
            if (targetVessel == null && navigationWaypoint == null || FlightGlobals.ActiveVessel == null)
            {
                ShowDetails = false;
                return;
            }

            ShowDetails = true;

            double originLat = FlightGlobals.ActiveVessel.latitude;
            double originLon = FlightGlobals.ActiveVessel.longitude;

            if (targetVessel != null && targetVessel.mainBody == FlightGlobals.ActiveVessel.mainBody)
            {
                double targetLat = targetVessel.mainBody.GetLatitude(targetVessel.GetWorldPos3D());
                double targetLon = targetVessel.mainBody.GetLongitude(targetVessel.GetWorldPos3D());

                SurfaceDistanceToTarget = CalcSurfaceDistance(FlightGlobals.ActiveVessel.mainBody.Radius,
                    originLat, originLon,
                    targetLat, targetLon);

                SurfaceHeadingToTarget = CalcSurfaceHeadingToTarget(originLat,
                    originLon, targetLat,
                    targetLon);
            }

            if (navigationWaypoint != null)
            {
                SurfaceDistanceToWaypoint = CalcSurfaceDistance(FlightGlobals.ActiveVessel.mainBody.Radius,
                    originLat, originLon,
                    navigationWaypoint.latitude, navigationWaypoint.longitude);

                SurfaceHeadingToWaypoint = CalcSurfaceHeadingToTarget(originLat,
                    originLon, navigationWaypoint.latitude,
                    navigationWaypoint.longitude);
            }
        }



        /// <summary>
        /// Calculate the shortest great-circle distance between two points on a sphere which are given by latitude and longitude.
        ///
        ///
        /// https://en.wikipedia.org/wiki/Haversine_formula
        /// </summary>
        /// <param name="bodyRadius"></param> Radius of the sphere in meters
        /// <param name="originLatidue"></param>Latitude of the origin of the distance
        /// <param name="originLongitude"></param>Longitude of the origin of the distance
        /// <param name="destinationLatitude"></param>Latitude of the destination of the distance
        /// <param name="destinationLongitude"></param>Longitude of the destination of the distance
        /// <returns>Distance between origin and source in meters</returns>
        private static double CalcSurfaceDistance(
            double bodyRadius,
            double originLatitude, double originLongitude,
            double targetLatitude, double targetLongitude)
        {
            double sin1 = Math.Sin(Math.PI / 180.0 * (originLatitude - targetLatitude) / 2);
            double sin2 = Math.Sin(Math.PI / 180.0 * (originLongitude - targetLongitude) / 2);
            double cos1 = Math.Cos(Math.PI / 180.0 * targetLatitude);
            double cos2 = Math.Cos(Math.PI / 180.0 * originLatitude);

            return 2 * bodyRadius * Math.Asin(Math.Sqrt(sin1 * sin1 + cos1 * cos2 * sin2 * sin2));
        }

        private static double CalcSurfaceHeadingToTarget(
            double originLatitude,
            double originLongitude, double targetLatitude,
            double targetLongitude)
        {

            double y = Math.Sin(targetLongitude - originLongitude) * Math.Cos(targetLatitude);
            double x = (Math.Cos(originLatitude) * Math.Sin(targetLatitude)) - (Math.Sin(originLatitude) * Math.Cos(targetLatitude) * Math.Cos(targetLongitude - originLongitude));
            double requiredHeading = Math.Atan2(y, x) * 180.0 / Math.PI;
            return (requiredHeading + 360.0) % 360.0;
        }
    }
}