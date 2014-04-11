// Project:	KerbalEngineer
// Author:	CYBUTEK
// License:	Attribution-NonCommercial-ShareAlike 3.0 Unported

#region Using Directives

using UnityEngine;

#endregion

namespace KerbalEngineer.Flight.Readouts.Rendezvous
{
    public class TargetSelector : ReadoutModule
    {
        #region Fields

        private readonly float typeButtonWidth;
        private string searchQuery = string.Empty;
        private string searchText = string.Empty;
        private int targetCount;
        private bool typeIsBody;
        private bool usingSearch;
        private VesselType vesselType = VesselType.Unknown;
        private ITargetable targetObject;

        #endregion

        #region Initialisation

        public TargetSelector()
        {
            this.Name = "Target Selector";
            this.Category = ReadoutCategory.Rendezvous;
            this.HelpString = "A tool to allow easy browsing, searching and selection of targets.";
            this.typeButtonWidth = Mathf.Round(this.ContentWidth * 0.5f);
        }

        #endregion

        #region Drawing

        /// <summary>
        ///     Draws the target selector structure.
        /// </summary>
        public override void Draw()
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
                this.DrawTarget();
            }

            if (this.targetObject != FlightGlobals.fetch.VesselTarget)
            {
                this.targetObject = FlightGlobals.fetch.VesselTarget;
                this.ResizeRequested = true;
            }  
        }

        /// <summary>
        ///     Draws the search bar.
        /// </summary>
        private void DrawSearch()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("SEARCH:", this.FlexiLabelStyle, GUILayout.Width(60.0f));

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
            }

            GUILayout.EndHorizontal();
        }

        /// <summary>
        ///     Draws the button list of target types.
        /// </summary>
        private void DrawTypes()
        {
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
        ///     Draws the target information when selected.
        /// </summary>
        private void DrawTarget()
        {
            if (GUILayout.Button("Go Back to Target Selection", this.ButtonStyle, GUILayout.Width(this.ContentWidth)))
            {
                FlightGlobals.fetch.SetVesselTarget(null);
                this.ResizeRequested = true;
            }

            GUILayout.Space(3f);

            this.DrawLine("Selected Target", FlightGlobals.fetch.VesselTarget.GetName());
        }

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
        ///     Draws targetable vessels.
        /// </summary>
        private int DrawVessels()
        {
            var count = 0;
            foreach (var vessel in FlightGlobals.Vessels)
            {
                if (vessel == FlightGlobals.ActiveVessel || vessel.vesselType != this.vesselType)
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

        private void SetTargetAs(ITargetable target)
        {
            FlightGlobals.fetch.SetVesselTarget(target);
            this.targetObject = target;
            this.ResizeRequested = true;
        }

        #endregion
    }
}