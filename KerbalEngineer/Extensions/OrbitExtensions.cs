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

using UnityEngine;

#endregion

namespace KerbalEngineer.Extensions
{
    public static class OrbitExtensions
    {
        #region Constants

        public const double Tau = Math.PI * 2.0;

        #endregion

        #region Methods: public

        public static double GetAngleToAscendingNode(this Orbit orbit)
        {
            return GetAngleToTrueAnomaly(orbit, GetTrueAnomalyOfAscendingNode(orbit));
        }

        public static double GetAngleToDescendingNode(this Orbit orbit)
        {
            return GetAngleToTrueAnomaly(orbit, GetTrueAnomalyOfDescendingNode(orbit));
        }

        public static double GetAngleToPrograde(this Orbit orbit)
        {
            if (orbit.referenceBody == CelestialBodies.SystemBody.CelestialBody)
            {
                return 0.0;
            }

            var angle = AngleBetweenVectors(orbit.getRelativePositionAtUT(Planetarium.GetUniversalTime()),
                                            Vector3d.Exclude(orbit.GetOrbitNormal(), orbit.referenceBody.orbit.getRelativePositionAtUT(Planetarium.GetUniversalTime())));

            angle = (angle + 90.0).ClampTo(0.0, 360.0);

            return orbit.inclination < 90.0 ? angle : 360.0 - angle;
        }

        public static double GetAngleToRetrograde(this Orbit orbit)
        {
            if (orbit.referenceBody == CelestialBodies.SystemBody.CelestialBody)
            {
                return 0.0;
            }

            var angle = AngleBetweenVectors(orbit.getRelativePositionAtUT(Planetarium.GetUniversalTime()),
                                            Vector3d.Exclude(orbit.GetOrbitNormal(), orbit.referenceBody.orbit.getRelativePositionAtUT(Planetarium.GetUniversalTime())));

            angle = (angle - 90.0).ClampTo(0.0, 360.0);

            return orbit.inclination < 90.0 ? angle : 360.0 - angle;
        }

        public static double GetAngleToTrueAnomaly(this Orbit orbit, double tA)
        {
            return (tA - orbit.trueAnomaly).ClampTo(0.0, 360.0);
        }

        public static double GetAngleToVector(this Orbit orbit, Vector3d vector)
        {
            return GetAngleToTrueAnomaly(orbit, GetTrueAnomalyFromVector(orbit, Vector3d.Exclude(orbit.GetOrbitNormal(), vector)));
        }

        public static double GetPhaseAngle(this Orbit orbit, Orbit target)
        {
            return AngleBetweenVectors(orbit.pos, Vector3d.Exclude(orbit.GetOrbitNormal(), target.pos));
        }

        public static double GetTimeToAscendingNode(this Orbit orbit)
        {
            return GetTimeToTrueAnomaly(orbit, GetTrueAnomalyOfAscendingNode(orbit));
        }

        public static double GetTimeToDescendingNode(this Orbit orbit)
        {
            return GetTimeToTrueAnomaly(orbit, GetTrueAnomalyOfDescendingNode(orbit));
        }

        public static double GetTimeToTrueAnomaly(this Orbit orbit, double tA)
        {
            var time = orbit.GetDTforTrueAnomaly(tA * Mathf.Deg2Rad, orbit.period);
            return time < 0.0 ? time + orbit.period : time;
        }

        public static double GetTimeToVector(this Orbit orbit, Vector3d vector)
        {
            return GetTimeToTrueAnomaly(orbit, GetTrueAnomalyFromVector(orbit, vector));
        }

        public static double GetTrueAnomalyFromVector(this Orbit orbit, Vector3d vector)
        {
            return orbit.GetTrueAnomalyOfZupVector(vector) * Mathf.Rad2Deg;
        }

        public static double GetTrueAnomalyOfAscendingNode(this Orbit orbit)
        {
            return 360.0 - orbit.argumentOfPeriapsis;
        }

        public static double GetTrueAnomalyOfDescendingNode(this Orbit orbit)
        {
            return 180.0 - orbit.argumentOfPeriapsis;
        }

        #endregion

        #region Methods: private

        private static double AngleBetweenVectors(Vector3d vector1, Vector3d vector2)
        {
            var angle = Vector3d.Angle(vector1, vector2);
            var rotated = QuaternionD.AngleAxis(90.0, Vector3d.forward) * vector1;

            if (Vector3d.Angle(rotated, vector2) > 90.0)
            {
                angle = 360.0 - angle;
            }

            return angle;
        }

        #endregion
    }
}