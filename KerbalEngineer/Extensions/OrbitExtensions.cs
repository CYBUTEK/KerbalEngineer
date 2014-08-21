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

using System;

using UnityEngine;

namespace KerbalEngineer.Extensions
{
    public static class OrbitExtensions
    {
        public const double Tau = Math.PI * 2.0;

        public static double GetTimeToTrueAnomaly(this Orbit orbit, double tA)
        {
            var time = orbit.GetDTforTrueAnomaly(tA * Mathf.Deg2Rad, orbit.period);
            return time < 0.0 ? time + orbit.period : time;
        }

        public static double GetTrueAnomalyOfAscendingNode(this Orbit orbit)
        {
            return 360.0 - orbit.argumentOfPeriapsis;
        }

        public static double GetTrueAnomalyOfDescendingNode(this Orbit orbit)
        {
            return 180.0 - orbit.argumentOfPeriapsis;
        }

        public static double GetTimeToAscendingNode(this Orbit orbit)
        {
            return GetTimeToTrueAnomaly(orbit, GetTrueAnomalyOfAscendingNode(orbit));
        }

        public static double GetTimeToDescendingNode(this Orbit orbit)
        {
            return GetTimeToTrueAnomaly(orbit, GetTrueAnomalyOfDescendingNode(orbit));
        }

        public static double GetTrueAnomalyFromVector(this Orbit orbit, Vector3d vector)
        {
            var normal = SwappedOrbitNormal(orbit);
            var projected = Vector3d.Exclude(normal, vector);

            var vectorToAn = QuaternionD.AngleAxis(-orbit.LAN, Planetarium.up) * Planetarium.right;
            var vectorToPe = orbit.PeR * (QuaternionD.AngleAxis(orbit.argumentOfPeriapsis, normal) * vectorToAn);
            var angleFromPe = Vector3d.Angle(vectorToPe, projected);

            if (Math.Abs(Vector3d.Angle(projected, Vector3d.Cross(normal, vectorToPe))) < 90.0)
            {
                return angleFromPe;
            }

            return GetTimeToTrueAnomaly(orbit, 360.0 - angleFromPe);
        }

        public static Vector3d SwappedOrbitNormal(this Orbit orbit)
        {
            var normal = orbit.GetOrbitNormal();
            return -new Vector3d(normal.x, normal.z, normal.y).normalized;
        }
    }
}