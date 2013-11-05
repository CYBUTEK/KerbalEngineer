// Name:    Kerbal Engineer Redux
// Author:  CYBUTEK
// License: Attribution-NonCommercial-ShareAlike 3.0 Unported

using KerbalEngineer.Extensions;
using UnityEngine;

namespace KerbalEngineer.FlightEngineer
{
    public class EditDisplay : MonoBehaviour
    {
        #region Fields

        private Rect _windowPosition = new Rect(Screen.width / 2 - 250f, Screen.height / 2 - 250f, 500f, 500f);
        private int _windowID = EngineerGlobals.GetNextWindowID();
        private GUIStyle _windowStyle, _scrollStyle, _rowStyle, _buttonStyle, _titleStyle, _labelStyle;
        private Vector2 _scrollAvailablePosition = Vector2.zero;
        private Vector2 _scrollInstalledPosition = Vector2.zero;
        private ReadoutCategory _selectedCategory = ReadoutCategory.None;

        private bool _hasInitStyles = false;

        #endregion

        #region Properties

        private bool _visible = false;
        /// <summary>
        /// Gets and sets the visibility of the window.
        /// </summary>
        public bool Visible
        {
            get { return _visible; }
            set { _visible = value; }
        }

        private Section _section;
        /// <summary>
        /// Gets and sets the parent section.
        /// </summary>
        public Section Section
        {
            get { return _section; }
            set { _section = value; }
        }

        #endregion

        #region Initialisation

        public EditDisplay()
        {
            RenderingManager.AddToPostDrawQueue(0, Draw);
        }

        private void InitialiseStyles()
        {
            _hasInitStyles = true;

            _windowStyle = new GUIStyle(HighLogic.Skin.window);
            _windowStyle.alignment = TextAnchor.UpperCenter;

            _scrollStyle = new GUIStyle(HighLogic.Skin.scrollView);

            _rowStyle = new GUIStyle();
            _rowStyle.margin = new RectOffset(5, 5, 5, 5);
            _rowStyle.fixedHeight = 25f;

            _buttonStyle = new GUIStyle(HighLogic.Skin.button);
            _buttonStyle.normal.textColor = Color.white;
            _buttonStyle.fontSize = 11;
            _buttonStyle.fontStyle = FontStyle.Bold;
            _buttonStyle.stretchHeight = true;

            _titleStyle = new GUIStyle(HighLogic.Skin.label);
            _titleStyle.normal.textColor = Color.white;
            _titleStyle.alignment = TextAnchor.MiddleLeft;
            _titleStyle.fontSize = 12;
            _titleStyle.fontStyle = FontStyle.Bold;
            _titleStyle.stretchWidth = true;

            _labelStyle = new GUIStyle(HighLogic.Skin.label);
            _labelStyle.normal.textColor = Color.white;
            _labelStyle.alignment = TextAnchor.MiddleLeft;
            _labelStyle.fontSize = 12;
            _labelStyle.fontStyle = FontStyle.Bold;
            _labelStyle.stretchHeight = true;
            _labelStyle.stretchWidth = true;
        }

        #endregion

        #region Drawing

        private void Draw()
        {
            if (_visible)
            {
                if (!_hasInitStyles) InitialiseStyles();

                _windowPosition = GUILayout.Window(_windowID, _windowPosition, Window, "EDIT SECTION - " + _section.Title.ToUpper(), _windowStyle).ClampToScreen();
            }
        }

        private void Window(int windowID)
        {
            if (_selectedCategory == ReadoutCategory.None)
            {
                if (_section.Categories.Count > 0)
                    _selectedCategory = _section.Categories[0];
            }

            if (_section.Categories.Count > 1)
                Categories();

            Available(_selectedCategory);
            Installed();

            GUI.DragWindow();
        }

        private void Categories()
        {
            GUILayout.BeginHorizontal(GUILayout.Height(30f));
            foreach (ReadoutCategory category in _section.Categories)
            {
                bool isSelected = _selectedCategory == category;
                if (GUILayout.Toggle(isSelected, category.ToString().ToUpper(), _buttonStyle) && !isSelected)
                    _selectedCategory = category;
            }
            GUILayout.EndHorizontal();
        }

        private void Available(ReadoutCategory category)
        {
            GUI.skin = HighLogic.Skin;
            _scrollAvailablePosition = GUILayout.BeginScrollView(_scrollAvailablePosition, false, true, GUILayout.Height(150f));
            GUI.skin = null;

            GUILayout.Label("AVAILABLE", _titleStyle);

            GUILayout.BeginVertical();
            int count = 0;
            foreach (Readout readout in ReadoutList.Instance.GetCategory(category))
            {
                if (_section.Readouts.Contains(readout)) continue;

                count++;

                GUILayout.BeginHorizontal(_rowStyle);

                GUILayout.BeginVertical();
                GUILayout.Label(readout.Name, _labelStyle);
                GUILayout.EndVertical();

                GUILayout.BeginVertical(GUILayout.Width(100f));
                if (GUILayout.Button("INSTALL", _buttonStyle))
                    _section.Readouts.Add(readout);
                GUILayout.EndVertical();

                GUILayout.EndHorizontal();
            }

            if (count == 0)
            {
                GUILayout.BeginHorizontal(_rowStyle);
                GUILayout.Label("All readouts are installed!", _labelStyle);
                GUILayout.EndHorizontal();
            }

            GUILayout.EndVertical();
            GUILayout.EndScrollView();

            GUILayout.Space(5f);
        }

        private void Installed()
        {
            GUI.skin = HighLogic.Skin;
            _scrollInstalledPosition = GUILayout.BeginScrollView(_scrollInstalledPosition, false, true);
            GUI.skin = null;

            GUILayout.Label("INSTALLED", _titleStyle);

            GUILayout.BeginVertical();
            foreach (Readout readout in _section.Readouts)
            {
                GUILayout.BeginHorizontal(_rowStyle);

                GUILayout.BeginVertical();
                GUILayout.Label(readout.Name, _labelStyle);
                GUILayout.EndVertical();

                GUILayout.BeginVertical(GUILayout.Width(100f));
                if (GUILayout.Button("REMOVE", _buttonStyle))
                {
                    _section.Readouts.Remove(readout);
                    FlightDisplay.Instance.RequireResize = true;
                }
                GUILayout.EndVertical();

                GUILayout.EndHorizontal();
            }

            if (_section.Readouts.Count == 0)
            {
                GUILayout.BeginHorizontal(_rowStyle);
                GUILayout.Label("No readouts are installed!", _labelStyle);
                GUILayout.EndHorizontal();
            }

            GUILayout.EndVertical();
            GUILayout.EndScrollView();
        }

        #endregion
    }
}
