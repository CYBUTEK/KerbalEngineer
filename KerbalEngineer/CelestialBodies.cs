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

using System.Collections.Generic;
using UnityEngine;

#endregion

namespace KerbalEngineer
{
    public class CelestialBodies
    {
        #region Instance

        private static CelestialBodies _instance;

        /// <summary>
        ///     Gets or creates a global instance to be used.
        /// </summary>
        public static CelestialBodies Instance
        {
            get { return _instance ?? (_instance = new CelestialBodies()); }
        }

        #endregion

        #region Fields

        private string selectedBodyName;

        #endregion

        #region Properties

        /// <summary>
        ///     Gets a list of BodyInfo objects.
        /// </summary>
        public Dictionary<string, BodyInfo> BodyList { get; private set; }

        /// <summary>
        ///     Gets and sets the selected body name.
        /// </summary>
        public string SelectedBodyName
        {
            get { return this.selectedBodyName; }
            set
            {
                this.selectedBodyName = value;
                if (this.BodyList.ContainsKey(this.selectedBodyName))
                {
                    this.SelectedBodyInfo = this.BodyList[this.selectedBodyName];
                }
            }
        }

        /// <summary>
        ///     Gets the selected BodyInfo object.
        /// </summary>
        public BodyInfo SelectedBodyInfo { get; private set; }

        #endregion

        #region Initialisation

        private CelestialBodies()
        {
            this.BodyList = new Dictionary<string, BodyInfo>();

			//
			// Change by Nathaniel R. Lewis (aka. Teknoman117) (linux.robotdude@gmail.com)
			// 
			// Generate the bodies list by crawling the core's local body list.  This allows
			// Kerbal Engineer to automatically support any future worlds Squad may add 
			// and provide compatibility with world adding mods such as Kopernicus and Planet
			// Factory.
			//
			foreach (CelestialBody cb in PSystemManager.Instance.localBodies) 
			{
				// Generate a list of the names of the bodies orbiting this one (if it has orbiting bodies)
				List<string> orbitingBodies = null;
				if (cb.orbitingBodies != null && cb.orbitingBodies.Count > 0) 
				{
					orbitingBodies = new List<string> ();
					foreach (CelestialBody ob in cb.orbitingBodies)
					{
						orbitingBodies.Add (ob.bodyName);
					}
				}
				
				// Find the parent body (cb.referenceBody != cb prevents the circular reference of Kerbol)
				string parentBody = null;
				if (cb.referenceBody != null && cb.referenceBody != cb) 
				{
					parentBody = cb.referenceBody.bodyName;
				}

				// Compute atmospheric (in kPa) and gravitational properties (m/s^2)
				double gravitationalAccelerationASL = 9.81d * cb.GeeASL; 
				double atmosphericPressureASL = cb.atmosphere ? (101.325d * cb.atmosphereMultiplier) : 0d;

				// Add this body info
				this.AddBody (new BodyInfo (cb.bodyName, gravitationalAccelerationASL, atmosphericPressureASL, parentBody, (orbitingBodies != null) ? orbitingBodies.ToArray () : null));
			}

            this.SelectedBodyName = "Kerbin";
        }

        #endregion

        #region Methods

        private void AddBody(BodyInfo bodyInfo)
        {
            this.BodyList.Add(bodyInfo.Name, bodyInfo);
        }

        #endregion

        #region Embedded Classes

        public class BodyInfo
        {
            public BodyInfo(string name, double gravity, double atmosphere, string parent, string[] children)
            {
                this.Name = name;
                this.Gravity = gravity;
                this.Atmosphere = atmosphere;
                this.Parent = parent;
                this.Children = children;
            }

            public string Name { get; protected set; }
            public double Gravity { get; protected set; }
            public double Atmosphere { get; protected set; }
            public string Parent { get; protected set; }
            public string[] Children { get; protected set; }
        }

        #endregion
    }
}
