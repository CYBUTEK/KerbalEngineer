// Name:    Kerbal Engineer Redux
// Author:  CYBUTEK
// License: Attribution-NonCommercial-ShareAlike 3.0 Unported

using System.Collections.Generic;
using UnityEngine;

namespace KerbalEngineer.FlightEngineer
{
    public class Section
    {
        #region Properties

        protected GUIStyle TitleStyle { get; private set; }
        protected GUIStyle AreaStyle { get; private set; }

        private List<Readout> _readouts = new List<Readout>();
        /// <summary>
        /// Gets and sets the readouts to be displayed.
        /// </summary>
        public List<Readout> Readouts
        {
            get { return _readouts; }
            set { _readouts = value; }
        }

        private bool _visible = false;
        /// <summary>
        /// Gets and sets whether the section is visible.
        /// </summary>
        public bool Visible
        {
            get { return _visible; }
            set
            {
                if (_visible != value)
                    FlightDisplay.Instance.RequireResize = true;

                _visible = value;
            }
        }

        private string _title = string.Empty;
        /// <summary>
        /// Gets and sets the section title.
        /// </summary>
        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }

        #endregion

        #region Initialisation

        protected Section()
        {
            InitialiseStyles();
        }

        private void InitialiseStyles()
        {
            TitleStyle = new GUIStyle(HighLogic.Skin.label);
            TitleStyle.margin = new RectOffset();
            TitleStyle.padding = new RectOffset(3, 3, 3, 3);
            TitleStyle.normal.textColor = Color.white;
            TitleStyle.fontSize = 13;
            TitleStyle.fontStyle = FontStyle.Bold;
            TitleStyle.stretchWidth = true;

            AreaStyle = new GUIStyle(HighLogic.Skin.box);
            AreaStyle.margin = new RectOffset();
            AreaStyle.padding = new RectOffset(5, 5, 5, 5);
        }

        #endregion

        #region Update and Draw

        public void Update()
        {
            foreach (Readout readout in _readouts)
                readout.Update();
        }

        public void Draw()
        {
            GUILayout.Label(_title.ToUpper(), TitleStyle);
            GUILayout.BeginVertical(AreaStyle);
            foreach (Readout readout in _readouts)
                readout.Draw();
            GUILayout.EndVertical();
        }

        #endregion
    }
}
