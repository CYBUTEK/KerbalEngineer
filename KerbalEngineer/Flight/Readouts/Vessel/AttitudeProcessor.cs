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

namespace KerbalEngineer.Flight.Readouts.Vessel
{
    #region Using Directives

    using UnityEngine;

    #endregion

    public class AttitudeProcessor : IUpdatable, IUpdateRequest
    {
        #region Fields

        private static readonly AttitudeProcessor instance = new AttitudeProcessor();

        private Vector3 centreOfMass = Vector3.zero;

        private double heading;
        private double headingRate;
        private Vector3 north = Vector3.zero;
        private double pitch;
        private double pitchRate;
        private double previousHeading;
        private double previousPitch;
        private double previousRoll;
        private double roll;
        private double rollRate;
        private Quaternion surfaceRotation;
        private Vector3 up = Vector3.zero;

        #endregion

        #region Properties

        public static double Heading
        {
            get { return instance.heading; }
        }

        public static double HeadingRate
        {
            get { return instance.headingRate; }
        }

        public static AttitudeProcessor Instance
        {
            get { return instance; }
        }

        public static double Pitch
        {
            get { return instance.pitch; }
        }

        public static double PitchRate
        {
            get { return instance.pitchRate; }
        }

        public static double Roll
        {
            get { return instance.roll; }
        }

        public static double RollRate
        {
            get { return instance.rollRate; }
        }

        public bool UpdateRequested { get; set; }

        #endregion

        #region Methods

        public static void RequestUpdate()
        {
            instance.UpdateRequested = true;
        }

        public void Update()
        {
            this.surfaceRotation = this.GetSurfaceRotation();

            this.previousHeading = this.heading;
            this.previousPitch = this.pitch;
            this.previousRoll = this.roll;

            // This code was derived from MechJeb2's implementation for getting the vessel's surface relative rotation.
            this.heading = this.surfaceRotation.eulerAngles.y;
            this.pitch = this.surfaceRotation.eulerAngles.x > 180.0f
                ? 360.0f - this.surfaceRotation.eulerAngles.x
                : -this.surfaceRotation.eulerAngles.x;
            this.roll = this.surfaceRotation.eulerAngles.z > 180.0f
                ? 360.0f - this.surfaceRotation.eulerAngles.z
                : -this.surfaceRotation.eulerAngles.z;

            this.headingRate = this.heading - this.previousHeading;
            this.pitchRate = this.pitch - this.previousPitch;
            this.rollRate = this.roll - this.previousRoll;
        }

        private Quaternion GetSurfaceRotation()
        {
            // This code was derived from MechJeb2's implementation for getting the vessel's surface relative rotation.
            this.centreOfMass = FlightGlobals.ActiveVessel.findWorldCenterOfMass();
            this.up = (this.centreOfMass - FlightGlobals.ActiveVessel.mainBody.position).normalized;
            this.north = Vector3.ProjectOnPlane((FlightGlobals.ActiveVessel.mainBody.position + FlightGlobals.ActiveVessel.mainBody.transform.up * (float)FlightGlobals.ActiveVessel.mainBody.Radius) - this.centreOfMass, this.up).normalized;

            return Quaternion.Inverse(Quaternion.Euler(90.0f, 0.0f, 0.0f) * Quaternion.Inverse(FlightGlobals.ActiveVessel.transform.rotation) * Quaternion.LookRotation(this.north, this.up));
        }

        #endregion
    }
}