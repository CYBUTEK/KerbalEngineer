// 
//     Kerbal Engineer Redux
//
// Extensions methods are bad. 

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

        public static double GetAngleToAscendingNode(Orbit orbit)
        {
            return GetAngleToTrueAnomaly(orbit, GetTrueAnomalyOfAscendingNode(orbit));
        }

        public static double GetAngleToDescendingNode(Orbit orbit)
        {
            return GetAngleToTrueAnomaly(orbit, GetTrueAnomalyOfDescendingNode(orbit));
        }

        public static double GetAngleToPrograde(Orbit orbit)
        {
            return GetAngleToPrograde(orbit, Planetarium.GetUniversalTime());
        }

        public static double GetAngleToPrograde(Orbit orbit, double universalTime)
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

        public static double GetAngleToRetrograde(Orbit orbit)
        {
            return GetAngleToRetrograde(orbit, Planetarium.GetUniversalTime());
        }

        public static double GetAngleToRetrograde(Orbit orbit, double universalTime)
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

        public static double GetAngleToTrueAnomaly(Orbit orbit, double trueAnomaly)
        {
            return AngleHelper.Clamp360(trueAnomaly - (orbit.trueAnomaly * Units.RAD_TO_DEG));
        }

        public static double GetAngleToVector(Orbit orbit, Vector3d vector)
        {
            return GetAngleToTrueAnomaly(orbit, GetTrueAnomalyFromVector(orbit, Vector3d.Exclude(orbit.GetOrbitNormal(), vector)));
        }

        public static double GetPhaseAngle(Orbit orbit, Orbit target)
        {
            var angle = AngleHelper.GetAngleBetweenVectors(Vector3d.Exclude(orbit.GetOrbitNormal(), target.pos), orbit.pos);
            return (orbit.semiMajorAxis < target.semiMajorAxis) ? angle : angle - 360.0;
        }

        public static double GetRelativeInclination(Orbit orbit, Orbit target)
        {
            return Vector3d.Angle(orbit.GetOrbitNormal(), target.GetOrbitNormal());
        }

        public static double GetTimeToAscendingNode(Orbit orbit)
        {
            return GetTimeToTrueAnomaly(orbit, GetTrueAnomalyOfAscendingNode(orbit));
        }

        public static double GetTimeToDescendingNode(Orbit orbit)
        {
            return GetTimeToTrueAnomaly(orbit, GetTrueAnomalyOfDescendingNode(orbit));
        }

        public static double GetTimeToTrueAnomaly(Orbit orbit, double trueAnomaly)
        {
            var time = orbit.GetDTforTrueAnomaly(trueAnomaly * Mathf.Deg2Rad, orbit.period);
            return time < 0.0 ? time + orbit.period : time;
        }

        public static double GetTimeToVector(Orbit orbit, Vector3d vector)
        {
            return GetTimeToTrueAnomaly(orbit, GetTrueAnomalyFromVector(orbit, vector));
        }

        public static double GetTrueAnomalyFromVector(Orbit orbit, Vector3d vector)
        {
            return orbit.GetTrueAnomalyOfZupVector(vector) * Mathf.Rad2Deg;
        }

        public static double GetTrueAnomalyOfAscendingNode(Orbit orbit)
        {
            return 360.0 - orbit.argumentOfPeriapsis;
        }

        public static double GetTrueAnomalyOfDescendingNode(Orbit orbit)
        {
            return 180.0 - orbit.argumentOfPeriapsis;
        }

        #endregion
    }
}