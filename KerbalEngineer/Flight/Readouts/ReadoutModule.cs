// Project:	KerbalEngineer
// Author:	CYBUTEK
// License:	Attribution-NonCommercial-ShareAlike 3.0 Unported

#region Using Directives

using UnityEngine;

#endregion

namespace KerbalEngineer.Flight.Readouts
{
    public abstract class ReadoutModule
    {
        #region Constructors

        /// <summary>
        ///     Creates a new readout module.
        /// </summary>
        protected ReadoutModule()
        {
            this.InitialiseStyles();
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets and sets the readout name.
        /// </summary>
        public string Name { get; set; }

        public string HelpMessage { get; set; }

        public bool ShowHelp { get; set; }

        /// <summary>
        ///     Gets ans sets the readout category.
        /// </summary>
        public ReadoutCategory Category { get; set; }

        /// <summary>
        ///     Gets the width of the content. (Sum of NameStyle + ValueStyle widths.)
        /// </summary>
        public float ContentWidth
        {
            get { return this.NameStyle.fixedWidth + this.ValueStyle.fixedWidth; }
        }

        #endregion

        #region GUIStyles

        /// <summary>
        ///     Gets and sets the name style.
        /// </summary>
        public GUIStyle NameStyle { get; set; }

        /// <summary>
        ///     Gets and sets the value style.
        /// </summary>
        public GUIStyle ValueStyle { get; set; }

        /// <summary>
        ///     Gets and sets the button style.
        /// </summary>
        public GUIStyle ButtonStyle { get; set; }

        /// <summary>
        ///     Initialises all the styles required for this object.
        /// </summary>
        private void InitialiseStyles()
        {
            this.NameStyle = new GUIStyle(HighLogic.Skin.label)
            {
                normal =
                {
                    textColor = Color.white
                },
                margin = new RectOffset(),
                padding = new RectOffset(),
                alignment = TextAnchor.MiddleLeft,
                fontSize = 11,
                fontStyle = FontStyle.Bold,
                fixedWidth = 110.0f,
                fixedHeight = 20.0f
            };

            this.ValueStyle = new GUIStyle(HighLogic.Skin.label)
            {
                margin = new RectOffset(),
                padding = new RectOffset(),
                alignment = TextAnchor.MiddleRight,
                fontSize = 11,
                fontStyle = FontStyle.Normal,
                fixedWidth = 110.0f,
                fixedHeight = 20.0f
            };

            this.ButtonStyle = new GUIStyle(HighLogic.Skin.button)
            {
                normal =
                {
                    textColor = Color.white
                },
                margin = new RectOffset(),
                padding = new RectOffset(),
                fontSize = 11,
                fixedHeight = 20.0f
            };
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Draws a single data line.
        /// </summary>
        protected void DrawLine(string name, string value)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(name, this.NameStyle);
            GUILayout.Label(value, this.ValueStyle);
            GUILayout.EndHorizontal();
        }

        /// <summary>
        ///     Draws a single data line.
        /// </summary>
        protected void DrawLine(string value)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(this.Name, this.NameStyle);
            GUILayout.Label(value, this.ValueStyle);
            GUILayout.EndHorizontal();
        }

        #endregion

        #region Virtual Methods

        /// <summary>
        ///     Called on each update frame where the readout is visible.
        /// </summary>
        public virtual void Update() { }

        /// <summary>
        ///     Called when a readout is asked to draw its self.
        /// </summary>
        public virtual void Draw() { }

        /// <summary>
        ///     Called when the active vessel changes.
        /// </summary>
        public virtual void Reset() { }

        #endregion
    }
}