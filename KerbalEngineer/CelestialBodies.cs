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
using System.Collections.Generic;
using System.Linq;

#endregion

namespace KerbalEngineer
{
    /*
     * 
     * With thanks to Nathaniel R. Lewis (aka. Teknoman117) (linux.robotdude@gmail.com) for working out
     * the best way of getting the celestial body information dynamically using PSystemManager.
     *
     */

    public static class CelestialBodies
    {
        #region Constructors

        static CelestialBodies()
        {
            try
            {
                SystemBody = new BodyInfo(PSystemManager.Instance.localBodies.Find(b => b.referenceBody == null || b.referenceBody == b));
                if (!SetSelectedBody("Kerbin"))
                {
                    SelectedBody = SystemBody;
                    SelectedBody.SetSelected(true);
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }

        #endregion

        #region Properties

        public static BodyInfo SelectedBody { get; private set; }
        public static BodyInfo SystemBody { get; private set; }

        #endregion

        #region Public Methods

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
                Logger.Exception(ex);
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
                var body = GetBodyInfo(bodyName);
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
                Logger.Exception(ex);
            }
            return false;
        }

        #endregion

        #region Nested type: BodyInfo

        public class BodyInfo
        {
            #region Constructors

            public BodyInfo(CelestialBody body, BodyInfo parent = null)
            {
                try
                {
                    // Set the body information.
                    this.CelestialBody = body;
                    this.Name = body.bodyName;
                    this.Gravity = 9.81 * body.GeeASL;
                    this.Atmosphere = body.atmosphere ? body.atmospherePressureSeaLevel : 0;
                    this.Parent = parent;

                    // Set orbiting bodies information.
                    this.Children = new List<BodyInfo>();
                    foreach (var orbitingBody in body.orbitingBodies)
                    {
                        this.Children.Add(new BodyInfo(orbitingBody, this));
                    }

                    this.SelectedDepth = 0;
                }
                catch (Exception ex)
                {
                    Logger.Exception(ex);
                }
            }

            #endregion

            #region Properties

            public string Name { get; private set; }
            public double Gravity { get; private set; }
            public double Atmosphere { get; private set; }
            public BodyInfo Parent { get; private set; }
            public List<BodyInfo> Children { get; private set; }
            public CelestialBody CelestialBody { get; private set; }
            public bool Selected { get; private set; }
            public int SelectedDepth { get; private set; }

            #endregion

            #region Public Methods

            public BodyInfo GetBodyInfo(string bodyName)
            {
                try
                {
                    // This is the searched body.
                    if (String.Equals(this.Name, bodyName, StringComparison.CurrentCultureIgnoreCase))
                    {
                        return this;
                    }

                    // Check to see if any of this bodies children are the searched body.
                    foreach (var child in this.Children)
                    {
                        var body = child.GetBodyInfo(bodyName);
                        if (body != null)
                        {
                            return body;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Exception(ex);
                }

                // A body with the specified name was not found.
                return null;
            }

            public void SetSelected(bool state, int depth = 0)
            {
                this.Selected = state;
                this.SelectedDepth = depth;
                if (this.Parent != null)
                {
                    this.Parent.SetSelected(state, depth + 1);
                }
            }

            #endregion

            #region Debugging

            public override string ToString()
            {
                var log = "\n" + this.Name +
                          "\n\tGravity: " + this.Gravity +
                          "\n\tAtmosphere: " + this.Atmosphere +
                          "\n\tSelected: " + this.Selected;

                return this.Children.Aggregate(log, (current, child) => current + "\n" + child);
            }

            #endregion
        }

        #endregion
    }
}