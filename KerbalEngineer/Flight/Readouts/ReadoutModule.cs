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

        public string HelpString { get; set; }

        public bool ShowHelp { get; set; }

        public bool ResizeRequested { get; set; }

        public bool IsDefault { get; set; }

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
        ///     Gets and sets the message style.
        /// </summary>
        public GUIStyle MessageStyle { get; set; }

        /// <summary>
        ///     Gets and sets the flexible label style.
        /// </summary>
        public GUIStyle FlexiLabelStyle { get; set; }

        /// <summary>
        ///     Gets and sets the button style.
        /// </summary>
        public GUIStyle ButtonStyle { get; set; }

        /// <summary>
        ///     Gets and sets the text field style.
        /// </summary>
        public GUIStyle TextFieldStyle { get; set; }

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
                padding = new RectOffset(5, 0, 0, 0),
                alignment = TextAnchor.MiddleLeft,
                fontSize = 11,
                fontStyle = FontStyle.Bold,
                fixedWidth = 115.0f,
                fixedHeight = 20.0f
            };

            this.ValueStyle = new GUIStyle(HighLogic.Skin.label)
            {
                margin = new RectOffset(),
                padding = new RectOffset(0, 5, 0, 0),
                alignment = TextAnchor.MiddleRight,
                fontSize = 11,
                fontStyle = FontStyle.Normal,
                fixedWidth = 115.0f,
                fixedHeight = 20.0f
            };

            this.MessageStyle = new GUIStyle(HighLogic.Skin.label)
            {
                normal =
                {
                    textColor = Color.white
                },
                margin = new RectOffset(),
                padding = new RectOffset(),
                alignment = TextAnchor.MiddleCenter,
                fontSize = 11,
                fontStyle = FontStyle.Normal,
                fixedWidth = this.ContentWidth,
                fixedHeight = 20.0f
            };

            this.FlexiLabelStyle = new GUIStyle(this.NameStyle)
            {
                fixedWidth = 0,
                stretchWidth = true
            };

            this.ButtonStyle = new GUIStyle(HighLogic.Skin.button)
            {
                normal =
                {
                    textColor = Color.white
                },
                margin = new RectOffset(0, 0, 1, 1),
                padding = new RectOffset(),
                alignment = TextAnchor.MiddleCenter,
                fontSize = 11,
                fixedHeight = 18.0f
            };

            this.TextFieldStyle = new GUIStyle(HighLogic.Skin.textField)
            {
                margin = new RectOffset(0, 0, 1, 1),
                padding = new RectOffset(5, 5, 0, 0),
                alignment = TextAnchor.MiddleLeft,
                fontSize = 11,
                fixedHeight = 18.0f
            };
        }

        #endregion

        #region Protected Methods

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

        protected void DrawMessageLine(string value)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(value, this.MessageStyle);
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