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
using KerbalEngineer.Helpers;

#endregion

// The calculations and functional code in this processor were generously developed by mic_e.

namespace KerbalEngineer.Flight.Readouts.Surface
{
    public class ImpactProcessor : IUpdatable, IUpdateRequest
    {
        #region Instance

        #region Fields

        private static readonly ImpactProcessor instance = new ImpactProcessor();

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the current instance of the impact processor.
        /// </summary>
        public static ImpactProcessor Instance
        {
            get { return instance; }
        }

        #endregion

        #endregion

        #region Fields

        private double impactAltitude;
        private bool impactHappening;
        private double impactLatitude;
        private double impactLongitude;
        private double impactTime;

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the altitude of the impact coordinates.
        /// </summary>
        public static double Altitude { get; private set; }

        /// <summary>
        ///     Gets the biome of the impact coordinates.
        /// </summary>
        public static string Biome { get; private set; }

        /// <summary>
        ///     Gets the latitude of the impact coordinates.
        /// </summary>
        public static double Latitude { get; private set; }

        /// <summary>
        ///     Gets the longitude of the impact coordinates.
        /// </summary>
        public static double Longitude { get; private set; }

        /// <summary>
        ///     Gets whether the details are ready to be shown.
        /// </summary>
        public static bool ShowDetails { get; private set; }

        /// <summary>
        ///     Gets the time to impact.
        /// </summary>
        public static double Time { get; private set; }

        #endregion

        #region IUpdatable Members

        public void Update()
        {
            this.impactHappening = false;

            if (FlightGlobals.ActiveVessel.mainBody.pqsController != null)
            {
                //do impact site calculations
                this.impactHappening = true;
                this.impactTime = 0;
                this.impactLongitude = 0;
                this.impactLatitude = 0;
                this.impactAltitude = 0;
                var e = FlightGlobals.ActiveVessel.orbit.eccentricity;
                //get current position direction vector
                var currentpos = this.RadiusDirection(FlightGlobals.ActiveVessel.orbit.trueAnomaly * Units.RAD_TO_DEG);
                //calculate longitude in inertial reference frame from that
                var currentirflong = 180 * Math.Atan2(currentpos.x, currentpos.y) / Math.PI;

                //experimentally determined; even for very flat trajectories, the errors go into the sub-millimeter area after 5 iterations or so
                const int impactiterations = 6;

                //do a few iterations of impact site calculations
                for (var i = 0; i < impactiterations; i++)
                {
                    if (FlightGlobals.ActiveVessel.orbit.PeA >= this.impactAltitude)
                    {
                        //periapsis must be lower than impact alt
                        this.impactHappening = false;
                    }
                    if ((FlightGlobals.ActiveVessel.orbit.eccentricity < 1) && (FlightGlobals.ActiveVessel.orbit.ApA <= this.impactAltitude))
                    {
                        //apoapsis must be higher than impact alt
                        this.impactHappening = false;
                    }
                    if ((FlightGlobals.ActiveVessel.orbit.eccentricity >= 1) && (FlightGlobals.ActiveVessel.orbit.timeToPe <= 0))
                    {
                        //if currently escaping, we still need to be before periapsis
                        this.impactHappening = false;
                    }
                    if (!this.impactHappening)
                    {
                        this.impactTime = 0;
                        this.impactLongitude = 0;
                        this.impactLatitude = 0;
                        this.impactAltitude = 0;
                        break;
                    }

                    double impacttheta = 0;
                    if (e > 0)
                    {
                        //in this step, we are using the calculated impact altitude of the last step, to refine the impact site position
                        impacttheta = -180 * Math.Acos((FlightGlobals.ActiveVessel.orbit.PeR * (1 + e) / (FlightGlobals.ActiveVessel.mainBody.Radius + this.impactAltitude) - 1) / e) / Math.PI;
                    }

                    //calculate time to impact
                    this.impactTime = FlightGlobals.ActiveVessel.orbit.timeToPe - this.TimeToPeriapsis(impacttheta);
                    //calculate position vector of impact site
                    var impactpos = this.RadiusDirection(impacttheta);
                    //calculate longitude of impact site in inertial reference frame
                    var impactirflong = 180 * Math.Atan2(impactpos.x, impactpos.y) / Math.PI;
                    var deltairflong = impactirflong - currentirflong;
                    //get body rotation until impact
                    var bodyrot = 360 * this.impactTime / FlightGlobals.ActiveVessel.mainBody.rotationPeriod;
                    //get current longitude in body coordinates
                    var currentlong = FlightGlobals.ActiveVessel.longitude;
                    //finally, calculate the impact longitude in body coordinates
                    this.impactLongitude = this.NormAngle(currentlong - deltairflong - bodyrot);
                    //calculate impact latitude from impact position
                    this.impactLatitude = 180 * Math.Asin(impactpos.z / impactpos.magnitude) / Math.PI;
                    //calculate the actual altitude of the impact site
                    //altitude for long/lat code stolen from some ISA MapSat forum post; who knows why this works, but it seems to.
                    var rad = QuaternionD.AngleAxis(this.impactLongitude, Vector3d.down) * QuaternionD.AngleAxis(this.impactLatitude, Vector3d.forward) * Vector3d.right;
                    this.impactAltitude = FlightGlobals.ActiveVessel.mainBody.pqsController.GetSurfaceHeight(rad) - FlightGlobals.ActiveVessel.mainBody.pqsController.radius;
                    if ((this.impactAltitude < 0) && FlightGlobals.ActiveVessel.mainBody.ocean)
                    {
                        this.impactAltitude = 0;
                    }
                }
            }

            // Set accessable properties.
            if (this.impactHappening)
            {
                ShowDetails = true;
                Time = this.impactTime;
                Longitude = this.impactLongitude;
                Latitude = this.impactLatitude;
                Altitude = this.impactAltitude;
                Biome = ScienceUtil.GetExperimentBiome(FlightGlobals.ActiveVessel.mainBody, this.impactLatitude, this.impactLongitude);
            }
            else
            {
                ShowDetails = false;
            }
        }

        #endregion

        #region IUpdateRequest Members

        public bool UpdateRequested { get; set; }

        #endregion

        #region Methods: public

        public static void RequestUpdate()
        {
            instance.UpdateRequested = true;
        }

        #endregion

        #region Calculations

        #region Methods: public

        public static double ACosh(double x)
        {
            return (Math.Log(x + Math.Sqrt((x * x) - 1.0)));
        }

        #endregion

        #region Methods: private

        private double NormAngle(double ang)
        {
            if (ang > 180)
            {
                ang -= 360 * Math.Ceiling((ang - 180) / 360);
            }
            if (ang <= -180)
            {
                ang -= 360 * Math.Floor((ang + 180) / 360);
            }

            return ang;
        }

        private Vector3d RadiusDirection(double theta)
        {
            theta = Math.PI * theta / 180;
            var omega = Math.PI * FlightGlobals.ActiveVessel.orbit.argumentOfPeriapsis / 180;
            var incl = Math.PI * FlightGlobals.ActiveVessel.orbit.inclination / 180;

            var costheta = Math.Cos(theta);
            var sintheta = Math.Sin(theta);
            var cosomega = Math.Cos(omega);
            var sinomega = Math.Sin(omega);
            var cosincl = Math.Cos(incl);
            var sinincl = Math.Sin(incl);

            Vector3d result;

            result.x = cosomega * costheta - sinomega * sintheta;
            result.y = cosincl * (sinomega * costheta + cosomega * sintheta);
            result.z = sinincl * (sinomega * costheta + cosomega * sintheta);

            return result;
        }

        private double TimeToPeriapsis(double theta)
        {
            var e = FlightGlobals.ActiveVessel.orbit.eccentricity;
            var a = FlightGlobals.ActiveVessel.orbit.semiMajorAxis;
            var rp = FlightGlobals.ActiveVessel.orbit.PeR;
            var mu = FlightGlobals.ActiveVessel.mainBody.gravParameter;

            if (e == 1.0)
            {
                var D = Math.Tan(Math.PI * theta / 360.0);
                var M = D + D * D * D / 3.0;
                return (Math.Sqrt(2.0 * rp * rp * rp / mu) * M);
            }
            if (a > 0)
            {
                var cosTheta = Math.Cos(Math.PI * theta / 180.0);
                var cosE = (e + cosTheta) / (1.0 + e * cosTheta);
                var radE = Math.Acos(cosE);
                var M = radE - e * Math.Sin(radE);
                return (Math.Sqrt(a * a * a / mu) * M);
            }
            if (a < 0)
            {
                var cosTheta = Math.Cos(Math.PI * theta / 180.0);
                var coshF = (e + cosTheta) / (1.0 + e * cosTheta);
                var radF = ACosh(coshF);
                var M = e * Math.Sinh(radF) - radF;
                return (Math.Sqrt(-a * a * a / mu) * M);
            }

            return 0;
        }

        #endregion

        #endregion
    }
}