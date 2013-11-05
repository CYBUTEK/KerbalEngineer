// Name:    Kerbal Engineer Redux
// Author:  CYBUTEK
// License: Attribution-NonCommercial-ShareAlike 3.0 Unported

using UnityEngine;

namespace KerbalEngineer.FlightEngineer
{
    public enum ReadoutCategory
    {
        None,
        Orbital,
        Surface,
        Vessel,
        Rendezvous,
        Misc
    }

    public abstract class Readout
    {
        #region Properties

        protected GUIStyle NameStyle { get; private set; }
        protected GUIStyle DataStyle { get; private set; }

        // Width of the name column.
        private float _nameWidth = 125f;
        protected float NameWidth
        {
            get { return _nameWidth; }
        }

        // Width of the data column.
        private float _dataWidth = 125f;
        protected float DataWidth
        {
            get { return _dataWidth; }
        }

        private string _name = string.Empty;
        /// <summary>
        /// Gets the readout name.
        /// </summary>
        public string Name
        {
            get { return _name; }
            protected set { _name = value; }
        }

        private string _description = string.Empty;
        /// <summary>
        /// Gets the readout description.
        /// </summary>
        public string Description
        {
            get { return _description; }
            protected set { _description = value; }
        }

        private ReadoutCategory _category = ReadoutCategory.Misc;
        /// <summary>
        /// Gets the category in which the readout is contained.
        /// </summary>
        public ReadoutCategory Category
        {
            get { return _category; }
            protected set { _category = value; }
        }

        #endregion

        #region Initialisation

        public Readout()
        {   
            InitialiseStyles();
            Initialise();
        }

        private void InitialiseStyles()
        {
            NameStyle = new GUIStyle(HighLogic.Skin.label);
            NameStyle.normal.textColor = Color.white;
            NameStyle.margin = new RectOffset();
            NameStyle.padding = new RectOffset(3, 3, 3, 3);
            NameStyle.fontSize = 12;
            NameStyle.fontStyle = FontStyle.Bold;
            NameStyle.alignment = TextAnchor.MiddleLeft;
            NameStyle.stretchWidth = true;

            DataStyle = new GUIStyle(HighLogic.Skin.label);
            DataStyle.margin = new RectOffset();
            DataStyle.padding = new RectOffset(3, 3, 3, 3);
            DataStyle.fontSize = 12;
            DataStyle.fontStyle = FontStyle.Normal;
            DataStyle.alignment = TextAnchor.MiddleRight;
            DataStyle.stretchWidth = true;
        }

        protected virtual void Initialise() { }

        #endregion

        #region Update and Draw

        public virtual void Update() { }

        public virtual void Draw() { }

        /// <summary>
        /// Draws a single line readout using the provided data.
        /// </summary>
        protected void DrawLine(string data)
        {
            GUILayout.BeginHorizontal();

            GUILayout.BeginVertical(GUILayout.Width(NameWidth));
            GUILayout.Label(Name, NameStyle);
            GUILayout.EndVertical();

            GUILayout.BeginVertical(GUILayout.Width(DataWidth));
            GUILayout.Label(data, DataStyle);
            GUILayout.EndVertical();

            GUILayout.EndHorizontal();
        }

        /// <summary>
        /// Draws a single line readout using the provided name and data.
        /// </summary>
        protected void DrawLine(string name, string data)
        {
            GUILayout.BeginHorizontal();

            GUILayout.BeginVertical(GUILayout.Width(NameWidth));
            GUILayout.Label(name, NameStyle);
            GUILayout.EndVertical();

            GUILayout.BeginVertical(GUILayout.Width(DataWidth));
            GUILayout.Label(data, DataStyle);
            GUILayout.EndVertical();

            GUILayout.EndHorizontal();
        }

        #endregion
    }
}
