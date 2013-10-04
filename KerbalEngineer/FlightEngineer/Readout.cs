// Name:    Kerbal Engineer Redux
// Author:  CYBUTEK
// License: Attribution-NonCommercial-ShareAlike 3.0 Unported

using UnityEngine;

namespace KerbalEngineer.FlightEngineer
{
    public enum ReadoutCategory
    {
        Orbital,
        Surface,
        Vessel,
        Rendezvous,
        Misc
    }

    public class Readout
    {
        #region Properties

        protected GUIStyle NameStyle { get; private set; }
        protected GUIStyle DataStyle { get; private set; }
        protected int NameWidth { get; private set; }
        protected int DataWidth { get; private set; }

        /// <summary>
        /// Gets the readout name.
        /// </summary>
        public string Name { get; protected set; }

        /// <summary>
        /// Gets the readout description.
        /// </summary>
        public string Description { get; protected set; }

        /// <summary>
        /// Gets the category in which the readout is contained.
        /// </summary>
        public ReadoutCategory Category { get; protected set; }

        #endregion

        #region Initialisation

        protected Readout()
        {   
            NameWidth = 100;
            DataWidth = 150;

            InitialiseStyles();
            Initialise();
        }

        private void InitialiseStyles()
        {
            NameStyle = new GUIStyle(HighLogic.Skin.label);
            NameStyle.normal.textColor = Color.white;
            NameStyle.fontSize = 11;
            NameStyle.fontStyle = FontStyle.Bold;
            NameStyle.alignment = TextAnchor.MiddleLeft;
            NameStyle.stretchWidth = true;

            DataStyle = new GUIStyle(HighLogic.Skin.label);
            DataStyle.fontSize = 11;
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
