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

namespace KerbalEngineer.Flight.Readouts.Rendezvous
{
    public class RendezvousProcessor : IUpdatable, IUpdateRequest
    {
        #region Instance

        private static readonly RendezvousProcessor instance = new RendezvousProcessor();

        /// <summary>
        ///     Gets the current instance of the rendezvous processor.
        /// </summary>
        public static RendezvousProcessor Instance
        {
            get { return instance; }
        }

        #endregion

        #region Fields

        private Orbit originOrbit;
        private Vector3d originPosition;
        private Orbit targetOrbit;
        private Vector3d targetPosition;

        #endregion

        #region Properties

        /// <summary>
        ///     Gets whether the details are ready to be shown.
        /// </summary>
        public static bool ShowDetails { get; private set; }

        /// <summary>
        ///     Gets the difference in angle from the origin position to the target position based on a common reference.
        /// </summary>
        public static double PhaseAngle { get; private set; }

        /// <summary>
        ///     Gets the difference in angle from the origin position to where it is most efficient to burn for an encounter.
        /// </summary>
        public static double InterceptAngle { get; private set; }

        /// <summary>
        ///     Gets the angular difference between the origin and target orbits.
        /// </summary>
        public static double RelativeInclination { get; private set; }

        /// <summary>
        ///     Gets the angle from the origin position to the ascending node.
        /// </summary>
        public static double AngleToAscendingNode { get; private set; }

        /// <summary>
        ///     Gets the angle from the origin position to the descending node.
        /// </summary>
        public static double AngleToDescendingNode { get; private set; }

        /// <summary>
        ///     Gets the target's altitude above its reference body.
        /// </summary>
        public static double AltitudeSeaLevel { get; private set; }

        /// <summary>
        ///     Gets the target's apoapsis above its reference body.
        /// </summary>
        public static double ApoapsisHeight { get; private set; }

        /// <summary>
        ///     Gets the target's periapsis above its reference body.
        /// </summary>
        public static double PeriapsisHeight { get; private set; }

        /// <summary>
        ///     Gets the target's time to apoapsis.
        /// </summary>
        public static double TimeToApoapsis { get; private set; }

        /// <summary>
        ///     Gets the target's time to periapsis.
        /// </summary>
        public static double TimeToPeriapsis { get; private set; }

        /// <summary>
        ///     Gets the distance from the origin position to the target position.
        /// </summary>
        public static double Distance { get; private set; }

        /// <summary>
        ///     Gets the orbital period of the target orbit.
        /// </summary>
        public static double OrbitalPeriod { get; private set; }

        #endregion

        #region IUpdatable Members

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

            this.targetPosition = this.targetOrbit.getRelativePositionAtUT(Planetarium.GetUniversalTime());
            this.originPosition = this.originOrbit.getRelativePositionAtUT(Planetarium.GetUniversalTime());

            PhaseAngle = this.CalcCurrentPhaseAngle();
            InterceptAngle = this.CalcInterceptAngle();
            RelativeInclination = Vector3d.Angle(this.originOrbit.GetOrbitNormal(), this.targetOrbit.GetOrbitNormal());
            AngleToAscendingNode = this.CalcAngleToAscendingNode();
            AngleToDescendingNode = this.CalcAngleToDescendingNode();
            AltitudeSeaLevel = this.targetOrbit.altitude;
            ApoapsisHeight = this.targetOrbit.ApA;
            PeriapsisHeight = this.targetOrbit.PeA;
            TimeToApoapsis = this.targetOrbit.timeToAp;
            TimeToPeriapsis = this.targetOrbit.timeToPe;

            Distance = Vector3d.Distance(this.targetPosition, this.originPosition);
            OrbitalPeriod = this.targetOrbit.period;
        }

        #endregion

        #region IUpdateRequest Members

        /// <summary>
        ///     Gets and sets whether the updatable object should be updated.
        /// </summary>
        public bool UpdateRequested { get; set; }

        #endregion

        /// <summary>
        ///     Request and update to calculate the details.
        /// </summary>
        public static void RequestUpdate()
        {
            instance.UpdateRequested = true;
        }

        #region Calculations

        private double CalcCurrentPhaseAngle()
        {
            return this.CalcPhaseAngle(this.originPosition, this.targetPosition);
        }

        private double CalcPhaseAngle(Vector3d originPos, Vector3d targetPos)
        {
            var phaseAngle = Vector3d.Angle(targetPos, originPos);
            if (Vector3d.Angle(Quaternion.AngleAxis(90.0f, Vector3d.forward) * originPos, targetPos) > 90.0)
            {
                phaseAngle = 360 - phaseAngle;
            }
            return (phaseAngle + 360) % 360;
        }

        private double CalcInterceptAngle()
        {
            var angle = 180.0 * (1.0 - Math.Pow((this.originOrbit.radius + this.targetOrbit.radius) / (2.0 * this.targetOrbit.radius), 1.5));
            if (angle < 0)
            {
                PhaseAngle -= 360;
                angle = (PhaseAngle - angle) + 360;
            }
            else
            {
                angle = PhaseAngle - angle;
            }
            if (angle < 0)
            {
                angle += 360;
            }
            return angle;
        }

        private double CalcAngleToAscendingNode()
        {
            var angleToNode = 0.0;
            if (this.originOrbit.inclination < 90.0)
            {
                angleToNode = this.CalcPhaseAngle(this.originPosition, this.GetAscendingNode());
            }
            else
            {
                angleToNode = 360 - this.CalcPhaseAngle(this.originPosition, this.GetAscendingNode());
            }
            return angleToNode;
        }

        private double CalcAngleToDescendingNode()
        {
            var angleToNode = 0.0;
            if (this.originOrbit.inclination < 90.0)
            {
                angleToNode = this.CalcPhaseAngle(this.originPosition, this.GetDescendingNode());
            }
            else
            {
                angleToNode = 360 - this.CalcPhaseAngle(this.originPosition, this.GetDescendingNode());
            }
            return angleToNode;
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