// Project:	KerbalEngineer
// Author:	CYBUTEK
// License:	Attribution-NonCommercial-ShareAlike 3.0 Unported

#region Using Directives

using UnityEngine;

#endregion

namespace KerbalEngineer.FlightEngineer.Rendezvous
{
    public class TargetSelector : Readout
    {
        #region Fields

        private string searchQuery = string.Empty;
        private string searchText = string.Empty;
        private int targetCount;
        private float typeButtonWidth;
        private bool typeIsBody;
        private bool usingSearch;
        private VesselType vesselType = VesselType.Unknown;

        #region Styles

        private GUIStyle buttonStyle;
        private GUIStyle searchStyle;

        #endregion

        #endregion

        #region Initialisation

        protected override void Initialise()
        {
            this.Name = "Target Selector";
            this.Description = "A tool to allow easy browsing, searching and selection of targets.";
            this.Category = ReadoutCategory.Rendezvous;
            this.InitialiseStyles();
        }

        private void InitialiseStyles()
        {
            this.buttonStyle = new GUIStyle(HighLogic.Skin.button)
            {
                normal =
                {
                    textColor = Color.white
                },
                margin = new RectOffset(),
                padding = new RectOffset(5, 5, 5, 5),
                fontSize = 11,
                fontStyle = FontStyle.Bold
            };

            this.searchStyle = new GUIStyle(HighLogic.Skin.textField)
            {
                stretchHeight = true
            };

            this.typeButtonWidth = Mathf.Round((NameWidth + DataWidth) * 0.5f);
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
        }

        /// <summary>
        ///     Draws the search bar.
        /// </summary>
        private void DrawSearch()
        {
            GUILayout.BeginHorizontal(GUILayout.Height(30.0f));
            GUILayout.BeginVertical(GUILayout.Width(75.0f));
            GUILayout.Label("SEARCH:", this.NameStyle, GUILayout.ExpandHeight(true));
            GUILayout.EndVertical();
            GUILayout.BeginVertical();

            this.searchText = GUILayout.TextField(this.searchText, this.searchStyle);

            if (this.searchText.Length > 0 || this.searchQuery.Length > 0)
            {
                this.searchQuery = this.searchText.ToLower();

                if (!this.usingSearch)
                {
                    this.usingSearch = true;
                    FlightDisplay.Instance.RequireResize = true;
                }
            }
            else if (this.usingSearch)
            {
                this.usingSearch = false;
            }

            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }

        /// <summary>
        ///     Draws the button list of target types.
        /// </summary>
        private void DrawTypes()
        {
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical(GUILayout.Width(this.typeButtonWidth));
            if (GUILayout.Button("Celestial Bodies", this.buttonStyle))
            {
                this.SetTypeAsBody();
            }
            GUILayout.EndVertical();
            GUILayout.BeginVertical(GUILayout.Width(this.typeButtonWidth));
            if (GUILayout.Button("Debris", this.buttonStyle))
            {
                this.SetTypeAs(VesselType.Debris);
            }
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical(GUILayout.Width(this.typeButtonWidth));
            if (GUILayout.Button("Probes", this.buttonStyle))
            {
                this.SetTypeAs(VesselType.Probe);
            }
            GUILayout.EndVertical();
            GUILayout.BeginVertical(GUILayout.Width(this.typeButtonWidth));
            if (GUILayout.Button("Rovers", this.buttonStyle))
            {
                this.SetTypeAs(VesselType.Rover);
            }
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical(GUILayout.Width(this.typeButtonWidth));
            if (GUILayout.Button("Landers", this.buttonStyle))
            {
                this.SetTypeAs(VesselType.Lander);
            }
            GUILayout.EndVertical();
            GUILayout.BeginVertical(GUILayout.Width(this.typeButtonWidth));
            if (GUILayout.Button("Ships", this.buttonStyle))
            {
                this.SetTypeAs(VesselType.Ship);
            }
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical(GUILayout.Width(this.typeButtonWidth));
            if (GUILayout.Button("Stations", this.buttonStyle))
            {
                this.SetTypeAs(VesselType.Station);
            }
            GUILayout.EndVertical();
            GUILayout.BeginVertical(GUILayout.Width(this.typeButtonWidth));
            if (GUILayout.Button("Bases", this.buttonStyle))
            {
                this.SetTypeAs(VesselType.Base);
            }
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical(GUILayout.Width(this.typeButtonWidth));
            if (GUILayout.Button("EVAs", this.buttonStyle))
            {
                this.SetTypeAs(VesselType.EVA);
            }
            GUILayout.EndVertical();
            GUILayout.BeginVertical(GUILayout.Width(this.typeButtonWidth));
            if (GUILayout.Button("Flags", this.buttonStyle))
            {
                this.SetTypeAs(VesselType.Flag);
            }
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }

        /// <summary>
        ///     Draws the target information when selected.
        /// </summary>
        private void DrawTarget()
        {
            if (GUILayout.Button("Go Back to Target Selection", this.buttonStyle))
            {
                FlightGlobals.fetch.SetVesselTarget(null);
                FlightDisplay.Instance.RequireResize = true;
            }

            GUILayout.Space(3f);

            this.DrawLine("Selected Target", FlightGlobals.fetch.VesselTarget.GetName());
        }

        /// <summary>
        ///     Draws the back to types button.
        /// </summary>
        private void DrawBackToTypes()
        {
            if (GUILayout.Button("Go Back to Type Selection", this.buttonStyle))
            {
                this.typeIsBody = false;
                this.vesselType = VesselType.Unknown;
                FlightDisplay.Instance.RequireResize = true;
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
                FlightDisplay.Instance.RequireResize = true;
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
                if (GUILayout.Button(body.bodyName, this.buttonStyle))
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
                if (GUILayout.Button(body.GetName(), this.buttonStyle))
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

                    if (GUILayout.Button(vessel.GetName(), this.buttonStyle))
                    {
                        this.SetTargetAs(vessel);
                    }
                }
                else if (vessel.vesselName.ToLower().Contains(this.searchQuery))
                {
                    count++;
                    if (GUILayout.Button(vessel.GetName(), this.buttonStyle))
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
            SectionList.Instance.RequestResize();
        }

        private void SetTypeAsBody()
        {
            this.typeIsBody = true;
            SectionList.Instance.RequestResize();
        }

        private void SetTargetAs(ITargetable target)
        {
            FlightGlobals.fetch.SetVesselTarget(target);
            SectionList.Instance.RequestResize();
        }

        #endregion
    }
}