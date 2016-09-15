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

namespace KerbalEngineer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /*
     * 
     * With thanks to Nathaniel R. Lewis (aka. Teknoman117) (linux.robotdude@gmail.com) for working out
     * the best way of getting the celestial body information dynamically using PSystemManager.
     *
     */

    public static class CelestialBodies
    {
        static CelestialBodies()
        {
            try
            {
                SystemBody = new BodyInfo(PSystemManager.Instance.localBodies.Find(b => b.referenceBody == null || b.referenceBody == b));
                String homeCBName = Planetarium.fetch.Home.bodyName;
                if (!SetSelectedBody(homeCBName))
                {
                    SelectedBody = SystemBody;
                    SelectedBody.SetSelected(true);
                }
            }
            catch (Exception ex)
            {
                MyLogger.Exception(ex);
            }
        }

        public static BodyInfo SelectedBody { get; private set; }
        public static BodyInfo SystemBody { get; private set; }

        /// <summary>
        ///     Gets a body given a supplied body name.
        /// </summary>
        public static BodyInfo GetBodyInfo(string bodyName)
        {
            try
            {
                return SystemBody.GetBodyInfo(bodyName);
            }
            catch (Exception ex)
            {
                MyLogger.Exception(ex);
            }
            return null;
        }

        /// <summary>
        ///     Sets the selected body to one matching the supplied body name.  Returns true if successful.
        /// </summary>
        public static bool SetSelectedBody(string bodyName)
        {
            try
            {
                BodyInfo body = GetBodyInfo(bodyName);
                if (body != null)
                {
                    if (SelectedBody != null)
                    {
                        SelectedBody.SetSelected(false);
                    }
                    SelectedBody = body;
                    SelectedBody.SetSelected(true);
                    return true;
                }
            }
            catch (Exception ex)
            {
                MyLogger.Exception(ex);
            }
            return false;
        }

        public class BodyInfo
        {
            public BodyInfo(CelestialBody body, BodyInfo parent = null)
            {
                try
                {
                    // Set the body information.
                    CelestialBody = body;
                    Name = body.bodyName;
                    Gravity = 9.81 * body.GeeASL;
                    Parent = parent;

                    // Set orbiting bodies information.
                    Children = new List<BodyInfo>();
                    foreach (CelestialBody orbitingBody in body.orbitingBodies)
                    {
                        Children.Add(new BodyInfo(orbitingBody, this));
                    }

                    SelectedDepth = 0;
                }
                catch (Exception ex)
                {
                    MyLogger.Exception(ex);
                }
            }

            public CelestialBody CelestialBody { get; private set; }
            public List<BodyInfo> Children { get; private set; }
            public double Gravity { get; private set; }
            public string Name { get; private set; }
            public BodyInfo Parent { get; private set; }
            public bool Selected { get; private set; }
            public int SelectedDepth { get; private set; }

            public BodyInfo GetBodyInfo(string bodyName)
            {
                try
                {
                    // This is the searched body.
                    if (String.Equals(Name, bodyName, StringComparison.CurrentCultureIgnoreCase))
                    {
                        return this;
                    }

                    // Check to see if any of this bodies children are the searched body.
                    foreach (BodyInfo child in Children)
                    {
                        BodyInfo body = child.GetBodyInfo(bodyName);
                        if (body != null)
                        {
                            return body;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MyLogger.Exception(ex);
                }

                // A body with the specified name was not found.
                return null;
            }

            public double GetDensity(double altitude)
            {
                return CelestialBody.GetDensity(GetPressure(altitude), GetTemperature(altitude));
            }

            public double GetPressure(double altitude)
            {
                return CelestialBody.GetPressure(altitude);
            }

            public double GetTemperature(double altitude)
            {
                return CelestialBody.GetTemperature(altitude);
            }

            public double GetAtmospheres(double altitude)
            {
                return GetPressure(altitude) * PhysicsGlobals.KpaToAtmospheres;
            }

            public void SetSelected(bool state, int depth = 0)
            {
                Selected = state;
                SelectedDepth = depth;
                if (Parent != null)
                {
                    Parent.SetSelected(state, depth + 1);
                }
            }

            public override string ToString()
            {
                string log = "\n" + Name +
                             "\n\tGravity: " + Gravity +
                             "\n\tSelected: " + Selected;

                return Children.Aggregate(log, (current, child) => current + "\n" + child);
            }
        }
    }
}