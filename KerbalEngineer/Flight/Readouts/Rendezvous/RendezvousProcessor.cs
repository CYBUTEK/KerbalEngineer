// 
//     Kerbal Engineer Redux
// 
//     Copyright (C) 2015 CYBUTEK
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

namespace KerbalEngineer.Flight.Readouts.Rendezvous
{
    using System;
    using Extensions;
    using Helpers;

    public class RendezvousProcessor : IUpdatable, IUpdateRequest
    {
        private static readonly RendezvousProcessor instance = new RendezvousProcessor();

        private Orbit originOrbit;
        private Orbit targetOrbit;

        /// <summary>
        ///     Gets the target's altitude above its reference body.
        /// </summary>
        public static double AltitudeSeaLevel { get; private set; }

        /// <summary>
        ///     Gets the angle from the origin position to the ascending node.
        /// </summary>
        public static double AngleToAscendingNode { get; private set; }

        /// <summary>
        ///     Gets the angle from the origin position to the descending node.
        /// </summary>
        public static double AngleToDescendingNode { get; private set; }

        /// <summary>
        ///     Gets the target's apoapsis above its reference body.
        /// </summary>
        public static double ApoapsisHeight { get; private set; }

        /// <summary>
        ///     Gets the distance from the origin position to the target position.
        /// </summary>
        public static double Distance { get; private set; }

        /// <summary>
        ///     Gets the current instance of the rendezvous processor.
        /// </summary>
        public static RendezvousProcessor Instance
        {
            get
            {
                return instance;
            }
        }

        /// <summary>
        ///     Gets the difference in angle from the origin position to where it is most efficient to burn for an encounter.
        /// </summary>
        public static double InterceptAngle { get; private set; }

        /// <summary>
        ///     Gets the orbital period of the target orbit.
        /// </summary>
        public static double OrbitalPeriod { get; private set; }

        /// <summary>
        ///     Gets the target's periapsis above its reference body.
        /// </summary>
        public static double PeriapsisHeight { get; private set; }

        /// <summary>
        ///     Gets the difference in angle from the origin position to the target position based on a common reference.
        /// </summary>
        public static double PhaseAngle { get; private set; }

        /// <summary>
        ///     Gets the angular difference between the origin and target orbits.
        /// </summary>
        public static double RelativeInclination { get; private set; }

        /// <summary>
        ///     Gets the relative orbital speed between the vessel and target.
        /// </summary>
        public static double RelativeSpeed { get; private set; }

        /// <summary>
        ///     Gets the relative orbital velocity between the vessel and target.
        /// </summary>
        public static double RelativeVelocity { get; private set; }

        /// <summary>
        ///     Gets the semi-major axis of the target orbit.
        /// </summary>
        public static double SemiMajorAxis { get; private set; }

        /// <summary>
        ///     Gets the semi-minor axis of the target orbit.
        /// </summary>
        public static double SemiMinorAxis { get; private set; }

        /// <summary>
        ///     Gets whether the details are ready to be shown.
        /// </summary>
        public static bool ShowDetails { get; private set; }

        /// <summary>
        ///     Gets the target's time to apoapsis.
        /// </summary>
        public static double TimeToApoapsis { get; private set; }

        /// <summary>
        ///     Gets the time it will take to reach the ascending node.
        /// </summary>
        public static double TimeToAscendingNode { get; private set; }

        /// <summary>
        ///     Gets the time it will take to reach the descending node.
        /// </summary>
        public static double TimeToDescendingNode { get; private set; }

        /// <summary>
        ///     Gets the target's time to periapsis.
        /// </summary>
        public static double TimeToPeriapsis { get; private set; }

        /// <summary>
        ///     Gets the relative radial velocity.
        /// </summary>
        public static double RelativeRadialVelocity { get; private set; }

        /// <summary>
        ///     Gets approximate (linearly) time to the minimum distance between objects.
        /// </summary>
        public static double TimeToRendezvous { get; private set; }

        /// <summary>
        ///     Gets and sets whether the updatable object should be updated.
        /// </summary>
        public bool UpdateRequested { get; set; }

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
            if (FlightGlobals.fetch == null ||
                FlightGlobals.fetch.VesselTarget == null ||
                FlightGlobals.ActiveVessel == null ||
                FlightGlobals.ActiveVessel.targetObject == null ||
                FlightGlobals.ActiveVessel.targetObject.GetOrbit() == null ||
                FlightGlobals.ship_orbit == null ||
                FlightGlobals.ship_orbit.referenceBody == null)
            {
                ShowDetails = false;
                return;
            }

            ShowDetails = true;

            targetOrbit = FlightGlobals.fetch.VesselTarget.GetOrbit();
            originOrbit = (FlightGlobals.ship_orbit.referenceBody == Planetarium.fetch.Sun ||
                           FlightGlobals.ship_orbit.referenceBody == FlightGlobals.ActiveVessel.targetObject.GetOrbit().referenceBody)
                ? FlightGlobals.ship_orbit
                : FlightGlobals.ship_orbit.referenceBody.orbit;

            RelativeInclination = originOrbit.GetRelativeInclination(targetOrbit);
            RelativeVelocity = FlightGlobals.ship_tgtSpeed;
            RelativeSpeed = FlightGlobals.ship_obtSpeed - targetOrbit.orbitalSpeed;
            PhaseAngle = originOrbit.GetPhaseAngle(targetOrbit);
            InterceptAngle = CalcInterceptAngle();
            TimeToAscendingNode = originOrbit.GetTimeToVector(GetAscendingNode());
            TimeToDescendingNode = originOrbit.GetTimeToVector(GetDescendingNode());
            AngleToAscendingNode = originOrbit.GetAngleToVector(GetAscendingNode());
            AngleToDescendingNode = originOrbit.GetAngleToVector(GetDescendingNode());
            AltitudeSeaLevel = targetOrbit.altitude;
            ApoapsisHeight = targetOrbit.ApA;
            PeriapsisHeight = targetOrbit.PeA;
            TimeToApoapsis = targetOrbit.timeToAp;
            TimeToPeriapsis = targetOrbit.timeToPe;
            SemiMajorAxis = targetOrbit.semiMajorAxis;
            SemiMinorAxis = targetOrbit.semiMinorAxis;

            Distance = Vector3d.Distance(targetOrbit.pos, originOrbit.pos);
            OrbitalPeriod = targetOrbit.period;

            // beware that the order/sign of coordinates is inconsistent across different exposed variables
            // in particular, v below does not equal to FlightGlobals.ship_tgtVelocity
            Vector3d x = targetOrbit.pos - originOrbit.pos;
            Vector3d v = targetOrbit.vel - originOrbit.vel;
            double xv = Vector3d.Dot(x, v);
            TimeToRendezvous = - xv / Vector3d.SqrMagnitude(v);
            RelativeRadialVelocity = xv / Vector3d.Magnitude(x);
        }

        private double CalcInterceptAngle()
        {
            double originRadius = (originOrbit.semiMinorAxis + originOrbit.semiMajorAxis) * 0.5;
            double targetRadius = (targetOrbit.semiMinorAxis + targetOrbit.semiMajorAxis) * 0.5;
            double angle = 180.0 * (1.0 - Math.Pow((originRadius + targetRadius) / (2.0 * targetRadius), 1.5));
            angle = PhaseAngle - angle;
            return RelativeInclination < 90.0 ? AngleHelper.Clamp360(angle) : AngleHelper.Clamp360(360.0 - (180.0 - angle));
        }

        private Vector3d GetAscendingNode()
        {
            return Vector3d.Cross(targetOrbit.GetOrbitNormal(), originOrbit.GetOrbitNormal());
        }

        private Vector3d GetDescendingNode()
        {
            return Vector3d.Cross(originOrbit.GetOrbitNormal(), targetOrbit.GetOrbitNormal());
        }
    }
}