// Name:    Kerbal Engineer Redux
// Author:  CYBUTEK
// License: Attribution-NonCommercial-ShareAlike 3.0 Unported

using UnityEngine;

namespace KerbalEngineer.FlightEngineer.Rendezvous
{
    public class TargetSelector : Readout
    {
        #region Fields

        private GUIStyle _buttonStyle, _searchStyle;
        private float _typeButtonWidth = 0f;
        private string _searchQuery = string.Empty;
        private string _searchText = string.Empty;
        private bool _usingSearch = false;
        private VesselType _vesselType = VesselType.Unknown;
        private bool _typeIsBody = false;
        private int _targetCount = 0;

        #endregion

        #region Initialisation

        protected override void Initialise()
        {
            Name = "Target Selector";
            Description = "A tool to allow easy browsing, searching and selection of targets.";
            Category = ReadoutCategory.Rendezvous;
            InitialiseStyles();
        }

        private void InitialiseStyles()
        {
            _buttonStyle = new GUIStyle(HighLogic.Skin.button);
            _buttonStyle.normal.textColor = Color.white;
            _buttonStyle.margin = new RectOffset();
            _buttonStyle.padding = new RectOffset(5, 5, 5, 5);
            _buttonStyle.fontSize = 11;
            _buttonStyle.fontStyle = FontStyle.Bold;

            _searchStyle = new GUIStyle(HighLogic.Skin.textField);
            _searchStyle.stretchHeight = true;

            _typeButtonWidth = Mathf.Round((Readout.NameWidth + Readout.DataWidth) / 2f);
        }

        #endregion

        #region Drawing

        // Draws the target selector structure.
        public override void Draw()
        {
            if (FlightGlobals.fetch.VesselTarget == null)
            {
                if (_vesselType == VesselType.Unknown && !_typeIsBody)
                {
                    DrawSearch();
                    if (_searchQuery.Length == 0)
                        DrawTypes();
                    else
                        DrawTargetList();
                }
                else
                {
                    DrawTargetList();
                    DrawBackToTypes();
                }
            }
            else
            {
                DrawTarget();
            }
        }

        // Draws the search bar.
        private void DrawSearch()
        {
            GUILayout.BeginHorizontal(GUILayout.Height(30f));
            GUILayout.BeginVertical(GUILayout.Width(75f));
            GUILayout.Label("SEARCH:", NameStyle, GUILayout.ExpandHeight(true));
            GUILayout.EndVertical();
            GUILayout.BeginVertical();

            _searchText = GUILayout.TextField(_searchText, _searchStyle);

            if (_searchText.Length > 0 || _searchQuery.Length > 0)
            {
                _searchQuery = _searchText.ToLower();

                if (!_usingSearch)
                {
                    _usingSearch = true;
                    FlightDisplay.Instance.RequireResize = true;
                }
            }
            else
                if (_usingSearch)
                    _usingSearch = false;

            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }

        // Draws the button list of target types.
        private void DrawTypes()
        {
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical(GUILayout.Width(_typeButtonWidth));
            if (GUILayout.Button("Celestial Bodies", _buttonStyle)) SetTypeAsBody();
            GUILayout.EndVertical();
            GUILayout.BeginVertical(GUILayout.Width(_typeButtonWidth));
            if (GUILayout.Button("Debris", _buttonStyle)) SetTypeAs(VesselType.Debris);
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical(GUILayout.Width(_typeButtonWidth));
            if (GUILayout.Button("Probes", _buttonStyle)) SetTypeAs(VesselType.Probe);
            GUILayout.EndVertical();
            GUILayout.BeginVertical(GUILayout.Width(_typeButtonWidth));
            if (GUILayout.Button("Rovers", _buttonStyle)) SetTypeAs(VesselType.Rover);
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical(GUILayout.Width(_typeButtonWidth));
            if (GUILayout.Button("Landers", _buttonStyle)) SetTypeAs(VesselType.Lander);
            GUILayout.EndVertical();
            GUILayout.BeginVertical(GUILayout.Width(_typeButtonWidth));
            if (GUILayout.Button("Ships", _buttonStyle)) SetTypeAs(VesselType.Ship);
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical(GUILayout.Width(_typeButtonWidth));
            if (GUILayout.Button("Stations", _buttonStyle)) SetTypeAs(VesselType.Station);
            GUILayout.EndVertical();
            GUILayout.BeginVertical(GUILayout.Width(_typeButtonWidth));
            if (GUILayout.Button("Bases", _buttonStyle)) SetTypeAs(VesselType.Base);
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical(GUILayout.Width(_typeButtonWidth));
            if (GUILayout.Button("EVAs", _buttonStyle)) SetTypeAs(VesselType.EVA);
            GUILayout.EndVertical();
            GUILayout.BeginVertical(GUILayout.Width(_typeButtonWidth));
            if (GUILayout.Button("Flags", _buttonStyle)) SetTypeAs(VesselType.Flag);
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }

        // Draws the target information when selected.
        private void DrawTarget()
        {
            DrawLine("Selected Target", FlightGlobals.fetch.VesselTarget.GetName());

            GUILayout.Space(3f);

            if (GUILayout.Button("Go Back to Target Selection", _buttonStyle))
            {
                FlightGlobals.fetch.SetVesselTarget(null);
                FlightDisplay.Instance.RequireResize = true;
            }
        }

        // Draws back to types button.
        private void DrawBackToTypes()
        {
            GUILayout.Space(3f);

            if (GUILayout.Button("Go Back to Type Selection", _buttonStyle))
            {
                _typeIsBody = false;
                _vesselType = VesselType.Unknown;
                FlightDisplay.Instance.RequireResize = true;
            }
        }

        // Draws the target list.
        private void DrawTargetList()
        {
            int count = 0;

            if (_searchQuery.Length == 0)
            {
                if (_typeIsBody)
                {
                    count += DrawMoons();
                    count += DrawPlanets();
                }
                else
                {
                    count += DrawVessels();
                }
            }
            else
            {
                count += DrawVessels();
                count += DrawMoons();
                count += DrawPlanets();
            }

            if (count == 0) DrawMessageLine("No targets found!");
            if (count != _targetCount)
            {
                _targetCount = count;
                FlightDisplay.Instance.RequireResize = true;
            }
        }

        // Draws targettable moons.
        private int DrawMoons()
        {
            int count = 0;
            foreach (CelestialBody body in FlightGlobals.Bodies)
            {
                if (body == Planetarium.fetch.Sun) continue;

                if (FlightGlobals.ActiveVessel.mainBody == body.referenceBody)
                {
                    if (_searchQuery.Length > 0 && !body.bodyName.ToLower().Contains(_searchQuery)) continue;

                    count++;
                    if (GUILayout.Button(body.bodyName, _buttonStyle))
                        SetTargetAs(body);
                }

            }
            return count;
        }

        // Draws targettable planets.
        private int DrawPlanets()
        {
            int count = 0;
            foreach (CelestialBody body in FlightGlobals.Bodies)
            {
                if (body == Planetarium.fetch.Sun || body == FlightGlobals.ActiveVessel.mainBody) continue;

                if (FlightGlobals.ActiveVessel.mainBody.referenceBody == body.referenceBody)
                {
                    if (_searchQuery.Length > 0 && !body.bodyName.ToLower().Contains(_searchQuery)) continue;

                    count++;
                    if (GUILayout.Button(body.GetName(), _buttonStyle))
                        SetTargetAs(body);
                }
            }
            return count;
        }

        // Draws targettable vessels.
        private int DrawVessels()
        {
            int count = 0;
            foreach (global::Vessel vessel in FlightGlobals.Vessels)
            {
                if (vessel == FlightGlobals.ActiveVessel) continue;

                if (_searchQuery.Length == 0)
                {
                    if (vessel.vesselType == _vesselType)
                    {
                        count++;
                        if (GUILayout.Button(vessel.GetName(), _buttonStyle))
                            SetTargetAs(vessel);
                    }
                }
                else if (vessel.vesselName.ToLower().Contains(_searchQuery))
                {
                    count++;
                    if (GUILayout.Button(vessel.GetName(), _buttonStyle))
                        SetTargetAs(vessel);
                }
            }
            return count;
        }
       

        #endregion

        #region Private Methods

        private void SetTypeAs(VesselType vesselType)
        {
            _vesselType = vesselType;
            FlightDisplay.Instance.RequireResize = true;
        }

        private void SetTypeAsBody()
        {
            _typeIsBody = true;
            FlightDisplay.Instance.RequireResize = true;
        }

        private void SetTargetAs(ITargetable target)
        {
            FlightGlobals.fetch.SetVesselTarget(target);
            FlightDisplay.Instance.RequireResize = true;
        }

        #endregion
    }
}
