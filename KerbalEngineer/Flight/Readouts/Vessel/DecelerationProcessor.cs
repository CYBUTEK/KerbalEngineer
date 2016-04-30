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
using KerbalEngineer.Flight.Readouts.Vessel;

using UnityEngine;

#endregion

namespace KerbalEngineer.Flight.Readouts.Vessel
{
    using Helpers;
    using System;

    // Shamelessly Stolen From Maneuver Node Processor by Harry Young

    public class DecelerationProcessor : IUpdatable, IUpdateRequest
	{
		#region Properties

		private static readonly DecelerationProcessor instance = new DecelerationProcessor();

        // Burn Based Information
		public static double DecelerationTime { get; private set; }
        public static double DecelerationDeltaV { get; private set; }
        public static double HorizontalDistance { get; private set; }
        public static double VerticalDistance { get; private set; }
        public static double TotalDistance { get; private set; }
        // Internal Calculation Steps
        private static double ProgradeDeltaV;
        private static double RadialDeltaV;
        private double m_Gravity;
        private double m_RadarAltitude;

        // Additional DeltaV Information
        public static int FinalStage { get; private set; }
        public static double AvailableDeltaV { get; private set; }
        public static bool HasDeltaV { get; private set; }

        // Landing Point Readouts
        public static double Altitude { get; private set; }
        public static string Biome { get; private set; }
        public static double Latitude { get; private set; }
        public static double Longitude { get; private set; }
        public static double AltitudeOverGround { get; private set; }
        public static string Slope { get; private set; }
        // Internal Calculation Steps
        private double impactAltitude;
        private double impactLatitude;
        private double impactLongitude;
        private Quaternion surfaceRotation;

        // Execution Info
        public static bool ShowDetails { get; set; }
		public bool UpdateRequested { get; set; }

		#endregion

		#region Methods: public

		public static void RequestUpdate()
		{
			instance.UpdateRequested = true;
			SimulationProcessor.RequestUpdate();
		}

		public static void Reset()
		{
			FlightEngineerCore.Instance.AddUpdatable(SimulationProcessor.Instance);
			FlightEngineerCore.Instance.AddUpdatable(instance);
		}

        public void Update()
        {
            //Check Background Logistics
            if (FlightGlobals.currentMainBody == null || FlightGlobals.ActiveVessel == null || SimulationProcessor.LastStage == null || !SimulationProcessor.ShowDetails)
            {
                ShowDetails = false;
                return;
            }
            // Check if we are actually moving
            if (ScienceUtil.GetExperimentSituation(FlightGlobals.ActiveVessel) == ExperimentSituations.SrfLanded || ScienceUtil.GetExperimentSituation(FlightGlobals.ActiveVessel) == ExperimentSituations.SrfSplashed)
            {
                ShowDetails = false;
                return;
            }

            var dtime = 0.0;

            #region Burn Calculations

            // Calculate Required DeltaV and Required Parameters
            m_Gravity = FlightGlobals.currentMainBody.gravParameter / Math.Pow(FlightGlobals.currentMainBody.Radius, 2.0);
			m_RadarAltitude = FlightGlobals.ActiveVessel.terrainAltitude > 0.0
				? FlightGlobals.ship_altitude - FlightGlobals.ActiveVessel.terrainAltitude
				: FlightGlobals.ship_altitude;
			ProgradeDeltaV = FlightGlobals.ActiveVessel.horizontalSrfSpeed;
			RadialDeltaV = Math.Sqrt((2 * m_Gravity * m_RadarAltitude) + Math.Pow(FlightGlobals.ship_verticalSpeed, 2.0));
			DecelerationDeltaV = Math.Sqrt(Math.Pow(ProgradeDeltaV, 2.0)+Math.Pow(RadialDeltaV, 2.0));

            // Get Burn Time
			HasDeltaV = GetSuicideBurnTime(DecelerationDeltaV, ref dtime);
            DecelerationTime = dtime;

            // Get Distance Traveled during Burn. Since we kill velocity I can assume average Velocity over burn.
            HorizontalDistance = 0.5 * ProgradeDeltaV * DecelerationTime;
            VerticalDistance = 0.5 * RadialDeltaV * DecelerationTime;
            TotalDistance = 0.5 * DecelerationDeltaV * DecelerationTime;

            #endregion

            #region Landing Point Calculations

            // I now know my horizontal Distance and I know my Heading, so I can calculate a Landing Point, get it's slope and Altitude and from that predict the radar altitude after the burn.
            if (FlightGlobals.ActiveVessel.mainBody.pqsController != null)
            {
                //do impact site calculations, special thanks to Muddr for Pointing me to http://www.movable-type.co.uk/scripts/latlong.html
                surfaceRotation = GetSurfaceRotation();
                var incl = surfaceRotation.eulerAngles.y;
                var currentlat = FlightGlobals.ActiveVessel.latitude;
                var currentlon = FlightGlobals.ActiveVessel.longitude;
                var angdst = 360 * HorizontalDistance / (2 * Math.PI * FlightGlobals.currentMainBody.Radius);
                var bodyrot = 360 * DecelerationTime / FlightGlobals.ActiveVessel.mainBody.rotationPeriod;

                impactLatitude = currentlat + Math.Sin(incl) * angdst;
                impactLongitude = currentlon + Math.Cos(incl) * angdst;

                //calculate the actual altitude of the impact site
                //altitude for long/lat code stolen from some ISA MapSat forum post; who knows why this works, but it seems to.
                var rad = QuaternionD.AngleAxis(this.impactLongitude, Vector3d.down) * QuaternionD.AngleAxis(this.impactLatitude, Vector3d.forward) * Vector3d.right;
                this.impactAltitude = FlightGlobals.ActiveVessel.mainBody.pqsController.GetSurfaceHeight(rad) - FlightGlobals.ActiveVessel.mainBody.pqsController.radius;
                if ((this.impactAltitude < 0) && FlightGlobals.ActiveVessel.mainBody.ocean)
                {
                    this.impactAltitude = 0;
                }
            }

            // Set accessable properties.
            Longitude = this.impactLongitude;
            Latitude = this.impactLatitude;
            Altitude = this.impactAltitude;
            Slope = GetSlopeAngleAndHeadingLanding(FlightGlobals.currentMainBody.GetRelSurfacePosition(impactLatitude, impactLongitude, impactAltitude));
            AltitudeOverGround = FlightGlobals.ship_altitude - Altitude - VerticalDistance;
            Biome = ScienceUtil.GetExperimentBiome(FlightGlobals.ActiveVessel.mainBody, this.impactLatitude, this.impactLongitude);

            #endregion

            ShowDetails = true;

        }

        #endregion

        #region Methods: private Additional Calculations

        //Suicide Burn time
        private static bool GetSuicideBurnTime(double deltaV, ref double burnTime)
		{
			for (var i = SimulationProcessor.Stages.Length - 1; i > -1; i--)
			{
				var stage = SimulationProcessor.Stages[i];
				var stageDeltaV = stage.deltaV;
				var startMass = stage.totalMass;

				//ProcessStageDrain
				if (deltaV <= Double.Epsilon)
				{
					break;
				}
				if (stageDeltaV <= Double.Epsilon)
				{
					continue;
				}

				FinalStage = i;

				double deltaVDrain = deltaV.Clamp(0.0, stageDeltaV);

				var exhaustVelocity = stage.isp * Units.GRAVITY;
				var flowRate = stage.thrust / exhaustVelocity;
				var endMass = Math.Exp(Math.Log(startMass) - deltaVDrain / exhaustVelocity);
				var deltaMass = (startMass - endMass) * Math.Exp(-(deltaVDrain * 0.001) / exhaustVelocity);
				burnTime += deltaMass / flowRate;

				deltaV -= deltaVDrain;
				stageDeltaV -= deltaVDrain;
				startMass -= deltaMass;
			}
			return deltaV <= Double.Epsilon;
        }

        // Helper to get locas Surface Rotation, Required to get Heading, Taken From attitudeprocessor
        private Quaternion GetSurfaceRotation()
        {
            // This code was derived from MechJeb2's implementation for getting the vessel's surface relative rotation.
            var centreOfMass = FlightGlobals.ActiveVessel.findWorldCenterOfMass();
            var up = (centreOfMass - FlightGlobals.ActiveVessel.mainBody.position).normalized;
            var north = Vector3.ProjectOnPlane((FlightGlobals.ActiveVessel.mainBody.position + FlightGlobals.ActiveVessel.mainBody.transform.up * (float)FlightGlobals.ActiveVessel.mainBody.Radius) - centreOfMass, up).normalized;

            return Quaternion.Inverse(Quaternion.Euler(90.0f, 0.0f, 0.0f) * Quaternion.Inverse(FlightGlobals.ActiveVessel.transform.rotation) * Quaternion.LookRotation(north, up));
        }
        
        // Slope at landing Point
        private string GetSlopeAngleAndHeadingLanding(Vector3d LandingPoint)
        {
            try
            {
                var result = "--° @ ---°";
                var mainBody = FlightGlobals.ActiveVessel.mainBody;
                var rad = (LandingPoint - mainBody.position).normalized;
                RaycastHit hit;
                if (Physics.Raycast(LandingPoint, -rad, out hit, Mathf.Infinity, 1 << 15)) // Just "Local Scenery" please
                {
                    var norm = hit.normal;
                    norm = norm.normalized;
                    var raddotnorm = Vector3d.Dot(rad, norm);
                    if (raddotnorm > 1.0)
                    {
                        raddotnorm = 1.0;
                    }
                    else if (raddotnorm < 0.0)
                    {
                        raddotnorm = 0.0;
                    }
                    var slope = Math.Acos(raddotnorm) * 180 / Math.PI;
                    result = Units.ToAngle(slope, 1);
                    if (slope < 0.05)
                    {
                        result += " @ ---°";
                    }
                    else
                    {
                        var side = Vector3d.Cross(rad, norm).normalized;
                        var east = Vector3d.Cross(rad, Vector3d.up).normalized;
                        var north = Vector3d.Cross(rad, east).normalized;
                        var sidedoteast = Vector3d.Dot(side, east);
                        var direction = Math.Acos(sidedoteast) * 180 / Math.PI;
                        var sidedotnorth = Vector3d.Dot(side, north);
                        if (sidedotnorth < 0)
                        {
                            direction = 360 - direction;
                        }
                        result += " @ " + Units.ToAngle(direction, 1);
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "Surface->Slope->GetSlopeAngleAndHeading");
                return "--° @ ---°";
            }
        }

        #endregion
    }
}