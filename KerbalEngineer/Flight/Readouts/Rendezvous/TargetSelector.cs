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

using KerbalEngineer.Flight.Sections;

using UnityEngine;

#endregion

namespace KerbalEngineer.Flight.Readouts.Rendezvous
{
    public class TargetSelector : ReadoutModule
    {
        #region Fields

        private string searchQuery = string.Empty;
        private string searchText = string.Empty;
        private int targetCount;
        private ITargetable targetObject;
        private float typeButtonWidth;
        private bool typeIsBody;
        private bool usingSearch;
        private VesselType vesselType = VesselType.Unknown;

        #endregion

        #region Initialisation

        public TargetSelector()
        {
            this.Name = "Target Selector";
            this.Category = ReadoutCategory.GetCategory("Rendezvous");
            this.HelpString = "A tool to allow easy browsing, searching and selection of targets.";
            this.IsDefault = true;
        }

        #endregion

        #region Drawing

        #region Methods: public

        /// <summary>
        ///     Draws the target selector structure.
        /// </summary>
        public override void Draw(SectionModule section)
        {
            if (FlightGlobals.fetch.VesselTarget == null)
            {
                if (this.vesselType == VesselType.Unknown && !this.typeIsBody)
                {
                    this.DrawSearch();
                    if (this.searchQuery.Length == 0)
                    {
                        this.DrawTypes();
                    }
                    else
                    {
                        this.DrawTargetList();
                    }
                }
                else
                {
                    this.DrawBackToTypes();
                    this.DrawTargetList();
                }
            }
            else
            {
                this.DrawTarget(section);
            }

            if (this.targetObject != FlightGlobals.fetch.VesselTarget)
            {
                this.targetObject = FlightGlobals.fetch.VesselTarget;
                this.ResizeRequested = true;
            }
        }

        #endregion

        #region Methods: private

        /// <summary>
        ///     Draws the back to types button.
        /// </summary>
        private void DrawBackToTypes()
        {
            if (GUILayout.Button("Go Back to Type Selection", this.ButtonStyle, GUILayout.Width(this.ContentWidth)))
            {
                this.typeIsBody = false;
                this.vesselType = VesselType.Unknown;
                this.ResizeRequested = true;
            }

            GUILayout.Space(3f);
        }

        /// <summary>
        ///     Draws targetable moons.
        /// </summary>
        private int DrawMoons()
        {
            var count = 0;

            foreach (var body in FlightGlobals.Bodies)
            {
                if (FlightGlobals.ActiveVessel.mainBody != body.referenceBody || body == Planetarium.fetch.Sun)
                {
                    continue;
                }

                if (this.searchQuery.Length > 0 && !body.bodyName.ToLower().Contains(this.searchQuery))
                {
                    continue;
                }

                count++;
                if (GUILayout.Button(body.bodyName, this.ButtonStyle, GUILayout.Width(this.ContentWidth)))
                {
                    this.SetTargetAs(body);
                }
            }
            return count;
        }

        /// <summary>
        ///     Draws the targetable planets.
        /// </summary>
        private int DrawPlanets()
        {
            var count = 0;
            foreach (var body in FlightGlobals.Bodies)
            {
                if (FlightGlobals.ActiveVessel.mainBody.referenceBody != body.referenceBody || body == Planetarium.fetch.Sun || body == FlightGlobals.ActiveVessel.mainBody)
                {
                    continue;
                }

                if (this.searchQuery.Length > 0 && !body.bodyName.ToLower().Contains(this.searchQuery))
                {
                    continue;
                }

                count++;
                if (GUILayout.Button(body.GetName(), this.ButtonStyle, GUILayout.Width(this.ContentWidth)))
                {
                    this.SetTargetAs(body);
                }
            }
            return count;
        }

        /// <summary>
        ///     Draws the search bar.
        /// </summary>
        private void DrawSearch()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("SEARCH:", this.FlexiLabelStyle, GUILayout.Width(60.0f * GuiDisplaySize.Offset));

            this.searchText = GUILayout.TextField(this.searchText, this.TextFieldStyle);

            if (this.searchText.Length > 0 || this.searchQuery.Length > 0)
            {
                this.searchQuery = this.searchText.ToLower();

                if (!this.usingSearch)
                {
                    this.usingSearch = true;
                    this.ResizeRequested = true;
                }
            }
            else if (this.usingSearch)
            {
                this.usingSearch = false;
                this.ResizeRequested = true;
            }

            GUILayout.EndHorizontal();
        }

        /// <summary>
        ///     Draws the target information when selected.
        /// </summary>
        private void DrawTarget(SectionModule section)
        {
            if (GUILayout.Button("Go Back to Target Selection", this.ButtonStyle, GUILayout.Width(this.ContentWidth)))
            {
                FlightGlobals.fetch.SetVesselTarget(null);
                this.ResizeRequested = true;
            }

            if (!(FlightGlobals.fetch.VesselTarget is CelestialBody) && GUILayout.Button("Switch to Target", this.ButtonStyle, GUILayout.Width(this.ContentWidth)))
            {
                FlightGlobals.SetActiveVessel(FlightGlobals.fetch.VesselTarget.GetVessel());
                this.ResizeRequested = true;
            }

            GUILayout.Space(3f);

            this.DrawLine("Selected Target", FlightGlobals.fetch.VesselTarget.GetName(), section.IsHud);
        }

        /// <summary>
        ///     Draws the target list.
        /// </summary>
        private void DrawTargetList()
        {
            var count = 0;

            if (this.searchQuery.Length == 0)
            {
                if (this.typeIsBody)
                {
                    count += this.DrawMoons();
                    count += this.DrawPlanets();
                }
                else
                {
                    count += this.DrawVessels();
                }
            }
            else
            {
                count += this.DrawVessels();
                count += this.DrawMoons();
                count += this.DrawPlanets();
            }

            if (count == 0)
            {
                this.DrawMessageLine("No targets found!");
            }

            if (count != this.targetCount)
            {
                this.targetCount = count;
                this.ResizeRequested = true;
            }
        }

        /// <summary>
        ///     Draws the button list of target types.
        /// </summary>
        private void DrawTypes()
        {
            this.typeButtonWidth = Mathf.Round(this.ContentWidth * 0.5f);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Celestial Bodies", this.ButtonStyle, GUILayout.Width(this.typeButtonWidth)))
            {
                this.SetTypeAsBody();
            }
            if (GUILayout.Button("Debris", this.ButtonStyle, GUILayout.Width(this.typeButtonWidth)))
            {
                this.SetTypeAs(VesselType.Debris);
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Probes", this.ButtonStyle, GUILayout.Width(this.typeButtonWidth)))
            {
                this.SetTypeAs(VesselType.Probe);
            }

            if (GUILayout.Button("Rovers", this.ButtonStyle, GUILayout.Width(this.typeButtonWidth)))
            {
                this.SetTypeAs(VesselType.Rover);
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Landers", this.ButtonStyle, GUILayout.Width(this.typeButtonWidth)))
            {
                this.SetTypeAs(VesselType.Lander);
            }
            if (GUILayout.Button("Ships", this.ButtonStyle, GUILayout.Width(this.typeButtonWidth)))
            {
                this.SetTypeAs(VesselType.Ship);
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Stations", this.ButtonStyle, GUILayout.Width(this.typeButtonWidth)))
            {
                this.SetTypeAs(VesselType.Station);
            }
            if (GUILayout.Button("Bases", this.ButtonStyle, GUILayout.Width(this.typeButtonWidth)))
            {
                this.SetTypeAs(VesselType.Base);
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("EVAs", this.ButtonStyle, GUILayout.Width(this.typeButtonWidth)))
            {
                this.SetTypeAs(VesselType.EVA);
            }
            if (GUILayout.Button("Flags", this.ButtonStyle, GUILayout.Width(this.typeButtonWidth)))
            {
                this.SetTypeAs(VesselType.Flag);
            }
            GUILayout.EndHorizontal();
        }

        /// <summary>
        ///     Draws targetable vessels.
        /// </summary>
        private int DrawVessels()
        {
            var count = 0;
            foreach (var vessel in FlightGlobals.Vessels)
            {
                if (vessel == FlightGlobals.ActiveVessel || (this.searchQuery.Length == 0 && vessel.vesselType != this.vesselType))
                {
                    continue;
                }

                if (this.searchQuery.Length == 0)
                {
                    count++;

                    if (GUILayout.Button(vessel.GetName(), this.ButtonStyle, GUILayout.Width(this.ContentWidth)))
                    {
                        this.SetTargetAs(vessel);
                    }
                }
                else if (vessel.vesselName.ToLower().Contains(this.searchQuery))
                {
                    count++;
                    if (GUILayout.Button(vessel.GetName(), this.ButtonStyle, GUILayout.Width(this.ContentWidth)))
                    {
                        this.SetTargetAs(vessel);
                    }
                }
            }
            return count;
        }

        private void SetTargetAs(ITargetable target)
        {
            FlightGlobals.fetch.SetVesselTarget(target);
            this.targetObject = target;
            this.ResizeRequested = true;
        }

        private void SetTypeAs(VesselType vesselType)
        {
            this.vesselType = vesselType;
            this.ResizeRequested = true;
        }

        private void SetTypeAsBody()
        {
            this.typeIsBody = true;
            this.ResizeRequested = true;
        }

        #endregion

        #endregion
    }
}