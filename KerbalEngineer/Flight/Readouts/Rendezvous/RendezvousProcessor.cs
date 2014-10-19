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

using KerbalEngineer.Extensions;
using KerbalEngineer.Helpers;

#endregion

namespace KerbalEngineer.Flight.Readouts.Rendezvous
{
    public class RendezvousProcessor : IUpdatable, IUpdateRequest
    {
        #region Fields

        private static readonly RendezvousProcessor instance = new RendezvousProcessor();

        private Orbit originOrbit;
        private Orbit targetOrbit;

        #endregion

        #region Properties

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
            get { return instance; }
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
        ///     Gets and sets whether the updatable object should be updated.
        /// </summary>
        public bool UpdateRequested { get; set; }

        #endregion

        #region Methods: public

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
            if (FlightGlobals.fetch.VesselTarget == null)
            {
                ShowDetails = false;
                return;
            }

            ShowDetails = true;

            this.targetOrbit = FlightGlobals.fetch.VesselTarget.GetOrbit();
            this.originOrbit = (FlightGlobals.ship_orbit.referenceBody == Planetarium.fetch.Sun || FlightGlobals.ship_orbit.referenceBody == FlightGlobals.ActiveVessel.targetObject.GetOrbit().referenceBody)
                ? FlightGlobals.ship_orbit
                : FlightGlobals.ship_orbit.referenceBody.orbit;

            RelativeInclination = this.originOrbit.GetRelativeInclination(this.targetOrbit);
            RelativeVelocity = FlightGlobals.ship_tgtSpeed;
            RelativeSpeed = FlightGlobals.ship_obtSpeed - this.targetOrbit.orbitalSpeed;
            PhaseAngle = this.originOrbit.GetPhaseAngle(this.targetOrbit);
            InterceptAngle = this.CalcInterceptAngle();
            TimeToAscendingNode = this.originOrbit.GetTimeToVector(this.GetAscendingNode());
            TimeToDescendingNode = this.originOrbit.GetTimeToVector(this.GetDescendingNode());
            AngleToAscendingNode = this.originOrbit.GetAngleToVector(this.GetAscendingNode());
            AngleToDescendingNode = this.originOrbit.GetAngleToVector(this.GetDescendingNode());
            AltitudeSeaLevel = this.targetOrbit.altitude;
            ApoapsisHeight = this.targetOrbit.ApA;
            PeriapsisHeight = this.targetOrbit.PeA;
            TimeToApoapsis = this.targetOrbit.timeToAp;
            TimeToPeriapsis = this.targetOrbit.timeToPe;
            SemiMajorAxis = this.targetOrbit.semiMajorAxis;
            SemiMinorAxis = this.targetOrbit.semiMinorAxis;

            Distance = Vector3d.Distance(this.targetOrbit.pos, this.originOrbit.pos);
            OrbitalPeriod = this.targetOrbit.period;
        }

        #endregion

        #region Methods: private

        private double CalcInterceptAngle()
        {
            var originRadius = (this.originOrbit.semiMinorAxis + this.originOrbit.semiMajorAxis) * 0.5;
            var targetRadius = (this.targetOrbit.semiMinorAxis + this.targetOrbit.semiMajorAxis) * 0.5;
            var angle = 180.0 * (1.0 - Math.Pow((originRadius + targetRadius) / (2.0 * targetRadius), 1.5));
            angle = PhaseAngle - angle;
            return RelativeInclination < 90.0 ? AngleHelper.Clamp360(angle) : AngleHelper.Clamp360(360.0 - (180.0 - angle));
        }

        private Vector3d GetAscendingNode()
        {
            return Vector3d.Cross(this.targetOrbit.GetOrbitNormal(), this.originOrbit.GetOrbitNormal());
        }

        private Vector3d GetDescendingNode()
        {
            return Vector3d.Cross(this.originOrbit.GetOrbitNormal(), this.targetOrbit.GetOrbitNormal());
        }

        #endregion
    }
}