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

using KerbalEngineer.Helpers;

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
            return GetAngleToPrograde(orbit, Planetarium.GetUniversalTime());
        }

        public static double GetAngleToPrograde(this Orbit orbit, double universalTime)
        {
            if (orbit.referenceBody == CelestialBodies.SystemBody.CelestialBody)
            {
                return 0.0;
            }

            Vector3d orbitVector = orbit.getRelativePositionAtUT(universalTime);
            orbitVector.z = 0.0;

            Vector3d bodyVector = orbit.referenceBody.orbit.getOrbitalVelocityAtUT(universalTime);
            bodyVector.z = 0.0;

            double angle = AngleHelper.GetAngleBetweenVectors(bodyVector, orbitVector);

            return AngleHelper.Clamp360(orbit.inclination < 90.0 ? angle : 360.0 - angle);
        }

        public static double GetAngleToRetrograde(this Orbit orbit)
        {
            return GetAngleToRetrograde(orbit, Planetarium.GetUniversalTime());
        }

        public static double GetAngleToRetrograde(this Orbit orbit, double universalTime)
        {
            if (orbit.referenceBody == CelestialBodies.SystemBody.CelestialBody)
            {
                return 0.0;
            }

            Vector3d orbitVector = orbit.getRelativePositionAtUT(universalTime);
            orbitVector.z = 0.0;

            Vector3d bodyVector = orbit.referenceBody.orbit.getOrbitalVelocityAtUT(universalTime);
            bodyVector.z = 0.0;

            double angle = AngleHelper.GetAngleBetweenVectors(-bodyVector, orbitVector);

            return AngleHelper.Clamp360(orbit.inclination < 90.0 ? angle : 360.0 - angle);
        }

        public static double GetAngleToTrueAnomaly(this Orbit orbit, double trueAnomaly)
        {
            return AngleHelper.Clamp360(trueAnomaly - (orbit.trueAnomaly * Units.RAD_TO_DEG));
        }

        public static double GetAngleToVector(this Orbit orbit, Vector3d vector)
        {
            return GetAngleToTrueAnomaly(orbit, GetTrueAnomalyFromVector(orbit, Vector3d.Exclude(orbit.GetOrbitNormal(), vector)));
        }

        public static double GetPhaseAngle(this Orbit orbit, Orbit target)
        {
            var angle = AngleHelper.GetAngleBetweenVectors(Vector3d.Exclude(orbit.GetOrbitNormal(), target.pos), orbit.pos);
            return (orbit.semiMajorAxis < target.semiMajorAxis) ? angle : angle - 360.0;
        }

        public static double GetRelativeInclination(this Orbit orbit, Orbit target)
        {
            return Vector3d.Angle(orbit.GetOrbitNormal(), target.GetOrbitNormal());
        }

        public static double GetTimeToAscendingNode(this Orbit orbit)
        {
            return GetTimeToTrueAnomaly(orbit, GetTrueAnomalyOfAscendingNode(orbit));
        }

        public static double GetTimeToDescendingNode(this Orbit orbit)
        {
            return GetTimeToTrueAnomaly(orbit, GetTrueAnomalyOfDescendingNode(orbit));
        }

        public static double GetTimeToTrueAnomaly(this Orbit orbit, double trueAnomaly)
        {
            var time = orbit.GetDTforTrueAnomaly(trueAnomaly * Mathf.Deg2Rad, orbit.period);
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
    }
}