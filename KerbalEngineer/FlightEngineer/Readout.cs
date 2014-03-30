// Project:	KerbalEngineer
// Author:	CYBUTEK
// License:	Attribution-NonCommercial-ShareAlike 3.0 Unported

#region Using Directives

using UnityEngine;

#endregion

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
        #region Static Properties

        private static float _nameWidth = 125.0f;
        private static float _dataWidth = 125.0f;

        /// <summary>
        ///     Width of the name column.
        /// </summary>
        public static float NameWidth
        {
            get { return _nameWidth; }
            set { _nameWidth = value; }
        }

        /// <summary>
        ///     Width of the data column.
        /// </summary>
        public static float DataWidth
        {
            get { return _dataWidth; }
            set { _dataWidth = value; }
        }

        #endregion

        #region Properties

        private ReadoutCategory category = ReadoutCategory.Misc;
        private string description = string.Empty;
        private string name = string.Empty;

        /// <summary>
        ///     Gets the GUIStyle for the readout name.
        /// </summary>
        protected GUIStyle NameStyle { get; private set; }

        /// <summary>
        ///     Gets the GUIStyle for the readout data.
        /// </summary>
        protected GUIStyle DataStyle { get; private set; }

        /// <summary>
        ///     Gets the GUIStyle for readout messages.
        /// </summary>
        protected GUIStyle MsgStyle { get; private set; }

        /// <summary>
        ///     Gets the readout name.
        /// </summary>
        public string Name
        {
            get { return this.name; }
            protected set { this.name = value; }
        }

        /// <summary>
        ///     Gets the readout description.
        /// </summary>
        public string Description
        {
            get { return this.description; }
            protected set { this.description = value; }
        }

        /// <summary>
        ///     Gets the category in which the readout is contained.
        /// </summary>
        public ReadoutCategory Category
        {
            get { return this.category; }
            protected set { this.category = value; }
        }

        #endregion

        #region Initialisation

        protected Readout()
        {
            this.InitialiseStyles();
            this.Initialise();
        }

        private void InitialiseStyles()
        {
            this.NameStyle = new GUIStyle(HighLogic.Skin.label)
            {
                normal =
                {
                    textColor = Color.white
                },
                margin = new RectOffset(),
                padding = new RectOffset(3, 3, 3, 3),
                fontSize = 12,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleLeft,
                stretchWidth = true
            };

            this.DataStyle = new GUIStyle(HighLogic.Skin.label)
            {
                margin = new RectOffset(),
                padding = new RectOffset(3, 3, 3, 3),
                fontSize = 12,
                fontStyle = FontStyle.Normal,
                alignment = TextAnchor.MiddleRight,
                stretchWidth = true
            };

            this.MsgStyle = new GUIStyle(this.NameStyle)
            {
                alignment = TextAnchor.MiddleCenter
            };
        }

        protected virtual void Initialise() { }

        #endregion

        #region Update and Draw

        public virtual void Update() { }

        public virtual void Draw() { }

        /// <summary>
        ///     Draws a single line readout using the provided data.
        /// </summary>
        protected void DrawLine(string data)
        {
            GUILayout.BeginHorizontal();

            GUILayout.BeginVertical(GUILayout.Width(_nameWidth));
            GUILayout.Label(this.Name, this.NameStyle);
            GUILayout.EndVertical();

            GUILayout.BeginVertical(GUILayout.Width(_dataWidth));
            GUILayout.Label(data, this.DataStyle);
            GUILayout.EndVertical();

            GUILayout.EndHorizontal();
        }

        /// <summary>
        ///     Draws a single line readout using the provided name and data.
        /// </summary>
        protected void DrawLine(string name, string data)
        {
            GUILayout.BeginHorizontal();

            GUILayout.BeginVertical(GUILayout.Width(_nameWidth));
            GUILayout.Label(name, this.NameStyle);
            GUILayout.EndVertical();

            GUILayout.BeginVertical(GUILayout.Width(_dataWidth));
            GUILayout.Label(data, this.DataStyle);
            GUILayout.EndVertical();

            GUILayout.EndHorizontal();
        }

        /// <summary>
        ///     Draws a single line message that is centred.
        /// </summary>
        protected void DrawMessageLine(string message)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(message, this.MsgStyle);
            GUILayout.EndHorizontal();
        }

        #endregion
    }
}