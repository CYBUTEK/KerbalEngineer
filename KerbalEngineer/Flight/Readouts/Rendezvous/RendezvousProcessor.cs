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
    using UnityEngine;

    public class RendezvousProcessor : IUpdatable, IUpdateRequest
    {
        private static readonly RendezvousProcessor instance = new RendezvousProcessor();

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
        ///     Gets time for landed ship to intersect target orbital plane in system.
        /// </summary>
        public static double[] TimeToPlane { get; private set; } = new double[2];

        /// <summary>
        ///     Gets angle for landed ship to intersect target orbital plane in system.
        /// </summary>
        public static double[] AngleToPlane { get; private set; }


        /// <summary>
        ///     If the target body is ascending at the point of planar intersect.
        /// </summary>
        public static bool TimeToPlaneisAsc { get; private set; }

        /// <summary>
        ///     Is the Ship landed.
        /// </summary>
        public static bool isLanded { get; private set; }

        /// <summary>
        ///     Is the Ship and target landed on same body.
        /// </summary>
        public static bool landedSamePlanet { get; private set; }

        /// <summary>
        ///    The current ship's reference body rotation period.
        /// </summary>
        public static double bodyRotationPeriod { get; private set; }

        /// <summary>
        ///    The display string of what's actually being calculated.
        /// </summary>
        public static string targetDisplay { get; private set; }

        /// <summary>
        ///    The display string of what's actually being calculated.
        /// </summary>
        public static string sourceDisplay { get; private set; }

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

        public static bool overrideANDN = false;

        public static bool overrideANDNRev = false;

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

            var actualSourceOrbit = FlightGlobals.ship_orbit;
            var actualTargetOrbit = FlightGlobals.fetch.VesselTarget.GetOrbit();

            isLanded = FlightGlobals.ActiveVessel.LandedOrSplashed;
            bool targetLanded = (FlightGlobals.fetch.VesselTarget is global::Vessel && ((global::Vessel)FlightGlobals.fetch.VesselTarget).LandedOrSplashed);
            landedSamePlanet = isLanded && targetLanded && actualSourceOrbit.referenceBody == actualTargetOrbit.referenceBody;

            var originOrbit = isLanded ? actualSourceOrbit.referenceBody.orbit : actualSourceOrbit;
            var targetOrbit = targetLanded ? actualTargetOrbit.referenceBody.orbit : actualTargetOrbit;

            if((!isLanded && !targetLanded) || (actualSourceOrbit.referenceBody != actualTargetOrbit.referenceBody))
                findConcentricParents(ref originOrbit, ref targetOrbit);

            overrideANDN = isLanded && actualSourceOrbit.referenceBody == targetOrbit.referenceBody;
            overrideANDNRev = targetLanded && !isLanded && actualTargetOrbit.referenceBody == actualSourceOrbit.referenceBody && FlightGlobals.ActiveVessel.targetObject.GetVessel() != null;

            if (landedSamePlanet)
            { //this should only occur when landed targeting something else landed on same body.
                RelativeInclination = 0;
                RelativeVelocity = FlightGlobals.ship_tgtSpeed;
                RelativeSpeed = 0;
                PhaseAngle = 0;
                InterceptAngle = 0;

                TimeToAscendingNode = 0;
                TimeToDescendingNode = 0;
                AngleToAscendingNode = 0;
                AngleToDescendingNode = 0;

                AltitudeSeaLevel = FlightGlobals.fetch.VesselTarget.GetVessel().altitude;
                ApoapsisHeight = 0;
                PeriapsisHeight = 0;
                TimeToApoapsis = 0;
                TimeToPeriapsis = 0;
                SemiMajorAxis = 0;
                SemiMinorAxis = 0;

                AngleToPlane[0] = 0;
                TimeToPlane[0] = 0;
                AngleToPlane[1] = 0;
                TimeToPlane[1] = 0;

                bodyRotationPeriod = FlightGlobals.ship_orbit.referenceBody.rotationPeriod;

                Distance = Vector3d.Distance(FlightGlobals.fetch.VesselTarget.GetVessel().GetWorldPos3D(), FlightGlobals.ActiveVessel.GetWorldPos3D());

                OrbitalPeriod = bodyRotationPeriod;

                TimeToRendezvous = 0;
                RelativeRadialVelocity = 0;
            }
            else
            {
                RelativeInclination = originOrbit.GetRelativeInclination(targetOrbit);
                RelativeVelocity = FlightGlobals.ship_tgtSpeed;
                RelativeSpeed = FlightGlobals.ship_obtSpeed - targetOrbit.orbitalSpeed;
                PhaseAngle = originOrbit.GetPhaseAngle(targetOrbit);
                InterceptAngle = CalcInterceptAngle(targetOrbit, originOrbit);

                TimeToAscendingNode = originOrbit.GetTimeToVector(GetAscendingNode(targetOrbit, originOrbit));
                TimeToDescendingNode = originOrbit.GetTimeToVector(GetDescendingNode(targetOrbit, originOrbit));
                AngleToAscendingNode = originOrbit.GetAngleToVector(GetAscendingNode(targetOrbit, originOrbit));
                AngleToDescendingNode = originOrbit.GetAngleToVector(GetDescendingNode(targetOrbit, originOrbit));

                AltitudeSeaLevel = targetOrbit.altitude;
                ApoapsisHeight = targetOrbit.ApA;
                PeriapsisHeight = targetOrbit.PeA;
                TimeToApoapsis = targetOrbit.timeToAp;
                TimeToPeriapsis = targetOrbit.timeToPe;
                SemiMajorAxis = targetOrbit.semiMajorAxis;
                SemiMinorAxis = targetOrbit.semiMinorAxis;
                OrbitalPeriod = targetOrbit.period;

                Distance = Vector3d.Distance(targetOrbit.pos, originOrbit.pos);

                bodyRotationPeriod = FlightGlobals.ship_orbit.referenceBody.rotationPeriod;

                // beware that the order/sign of coordinates is inconsistent across different exposed variables
                // in particular, v below does not equal to FlightGlobals.ship_tgtVelocity
                Vector3d x = targetOrbit.pos - originOrbit.pos;
                Vector3d v = targetOrbit.vel - originOrbit.vel;
                double xv = Vector3d.Dot(x, v);
                TimeToRendezvous = -xv / Vector3d.SqrMagnitude(v);
                RelativeRadialVelocity = xv / Vector3d.Magnitude(x);

                if (overrideANDN) //calc launch time
                {
                    AngleToPlane = CalcAngleToPlane(FlightGlobals.ship_orbit.referenceBody, FlightGlobals.ship_latitude, FlightGlobals.ship_longitude, targetOrbit);
                    TimeToPlane[0] = (AngleToPlane[0] / 360) * FlightGlobals.ship_orbit.referenceBody.rotationPeriod;
                    TimeToPlane[1] = (AngleToPlane[1] / 360) * FlightGlobals.ship_orbit.referenceBody.rotationPeriod;
                    RelativeInclination = targetOrbit.inclination;
                    RelativeRadialVelocity = 0;
                    TimeToRendezvous = 0;
                    PhaseAngle = 0;
                    InterceptAngle = 0;
                }
                else if(overrideANDNRev)
                { //calc land time.
                    global::Vessel tgt = FlightGlobals.ActiveVessel.targetObject.GetVessel();

                    AngleToPlane = CalcAngleToPlane(FlightGlobals.ship_orbit.referenceBody, tgt.latitude, tgt.longitude, originOrbit);

                    TimeToPlane[0] = (AngleToPlane[0] / 360) * FlightGlobals.ship_orbit.referenceBody.rotationPeriod;
                    TimeToPlane[1] = (AngleToPlane[1] / 360) * FlightGlobals.ship_orbit.referenceBody.rotationPeriod;

                    RelativeInclination = originOrbit.inclination;
                    Distance = Vector3d.Distance(FlightGlobals.fetch.VesselTarget.GetVessel().GetWorldPos3D(), FlightGlobals.ActiveVessel.GetWorldPos3D());
                    AltitudeSeaLevel = tgt.altitude;
                    PhaseAngle = 0;
                    InterceptAngle = 0;
                    ApoapsisHeight = 0;
                    PeriapsisHeight = 0;
                    TimeToApoapsis = 0;
                    TimeToPeriapsis = 0;
                    SemiMajorAxis = 0;
                    SemiMinorAxis = 0;
                    OrbitalPeriod = 0;
                    RelativeRadialVelocity = 0;
                    TimeToRendezvous = 0;
                }


            }

            if (actualTargetOrbit != targetOrbit)
            {
                targetDisplay = findNameForOrbit(targetOrbit, FlightGlobals.ActiveVessel.targetObject);
            }
            else
            {
                targetDisplay = null;
            }

            if (actualSourceOrbit != originOrbit)
            {
                sourceDisplay = findNameForOrbit(originOrbit, FlightGlobals.ActiveVessel);
            }
            else
            {
                sourceDisplay = null;
            }

        }

        public string findNameForOrbit(Orbit orbit, ITargetable start)
        {
            if (start.GetOrbit() == orbit || start.GetOrbit() == null)
                return start.GetName();
            else
                return (findNameForOrbit(orbit, start.GetOrbit().referenceBody));
        }


        public void findConcentricParents(ref Orbit source, ref Orbit target)
        {
            Orbit o = target;
            while (true)
            {
                while (true)
                {
                    //Debug.Log("Compare " + source.referenceBody.GetName() + " to " + target.referenceBody.GetName());

                    if (source.referenceBody == target.referenceBody)
                        return;
                    if (target.referenceBody.orbit == null) //the sun!
                        break;
                    target = target.referenceBody.orbit;
                }

                if (source.referenceBody.orbit == null) //the sun!
                    return;

                source = source.referenceBody.orbit;
                target = o;
            }
        }

        private double CalcInterceptAngle(Orbit targetOrbit, Orbit originOrbit)
        {
            double originRadius = (originOrbit.semiMinorAxis + originOrbit.semiMajorAxis) * 0.5;
            double targetRadius = (targetOrbit.semiMinorAxis + targetOrbit.semiMajorAxis) * 0.5;
            double angle = 180.0 * (1.0 - Math.Pow((originRadius + targetRadius) / (2.0 * targetRadius), 1.5));
            angle = PhaseAngle - angle;
            return RelativeInclination < 90.0 ? AngleHelper.Clamp360(angle) : AngleHelper.Clamp360(360.0 - (180.0 - angle));
        }

        private Vector3d GetAscendingNode(Orbit targetOrbit, Orbit originOrbit)
        {
            return Vector3d.Cross(targetOrbit.GetOrbitNormal(), originOrbit.GetOrbitNormal());
        }

        private Vector3d GetDescendingNode(Orbit targetOrbit, Orbit originOrbit)
        {
            return Vector3d.Cross(originOrbit.GetOrbitNormal(), targetOrbit.GetOrbitNormal());
        }

        //From MechJeb2
        //Computes the time required for the given launch location to rotate under the target orbital plane. 
        //If the latitude is too high for the launch location to ever actually rotate under the target plane,
        //returns the time of closest approach to the target plane.
        //I have a wonderful proof of this formula which this comment is too short to contain.
        private double[] CalcAngleToPlane(CelestialBody launchBody, double launchLatitude, double launchLongitude, Orbit target)
        {
            double[] o = new double[2];

            double inc = Math.Abs(Vector3d.Angle(SwappedOrbitNormal(target), launchBody.angularVelocity));
            Vector3d b = Vector3d.Exclude(launchBody.angularVelocity, SwappedOrbitNormal(target)).normalized; // I don't understand the sign here, but this seems to work
            b *= launchBody.Radius * Math.Sin(Math.PI / 180 * launchLatitude) / Math.Tan(Math.PI / 180 * inc);
            Vector3d c = Vector3d.Cross(SwappedOrbitNormal(target), launchBody.angularVelocity).normalized;
            double cMagnitudeSquared = Math.Pow(launchBody.Radius * Math.Cos(Math.PI / 180 * launchLatitude), 2) - b.sqrMagnitude;
            if (cMagnitudeSquared < 0) cMagnitudeSquared = 0;
            c *= Math.Sqrt(cMagnitudeSquared);
            Vector3d a1 = b + c;
            Vector3d a2 = b - c;

            Vector3d longitudeVector = launchBody.GetSurfaceNVector(0, launchLongitude);

            double angle1 = Math.Abs(Vector3d.Angle(longitudeVector, a1));
            if (Vector3d.Dot(Vector3d.Cross(longitudeVector, a1), launchBody.angularVelocity) < 0) angle1 = 360 - angle1;

            double angle2 = Math.Abs(Vector3d.Angle(longitudeVector, a2));
            if (Vector3d.Dot(Vector3d.Cross(longitudeVector, a2), launchBody.angularVelocity) < 0) angle2 = 360 - angle2;

            o[0] = Math.Min(angle1, angle2);
            o[1] = Math.Max(angle1, angle2) - 360;

           return o;
        }

        //normalized vector perpendicular to the orbital plane
        //convention: as you look down along the orbit normal, the satellite revolves counterclockwise
        public static Vector3d SwappedOrbitNormal(Orbit o)
        {
            return -SwapYZ(o.GetOrbitNormal()).normalized;
        }

        //can probably be replaced with Vector3d.xzy?
        public static Vector3d SwapYZ(Vector3d v)
        {
            return v.xzy;
        }
    }
}