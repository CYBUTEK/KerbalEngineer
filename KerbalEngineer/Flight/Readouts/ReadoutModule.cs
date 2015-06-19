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

using System;

using KerbalEngineer.Flight.Sections;

using UnityEngine;

#endregion

namespace KerbalEngineer.Flight.Readouts
{
    using Extensions;

    public abstract class ReadoutModule
    {
        #region Fields

        private int lineCountEnd;
        private int lineCountStart;

        #endregion

        #region Constructors

        protected ReadoutModule()
        {
            this.InitialiseStyles();
            GuiDisplaySize.OnSizeChanged += this.OnSizeChanged;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets and sets the button style.
        /// </summary>
        public GUIStyle ButtonStyle { get; set; }

        /// <summary>
        ///     Gets ans sets the readout category.
        /// </summary>
        public ReadoutCategory Category { get; set; }

        /// <summary>
        ///     Gets and sets whether the readout can be added to a section multiple times.
        /// </summary>
        public bool Cloneable { get; set; }

        /// <summary>
        ///     Gets the width of the content. (Sum of NameStyle + ValueStyle widths.)
        /// </summary>
        public float ContentWidth
        {
            get { return 230.0f * GuiDisplaySize.Offset; }
        }

        /// <summary>
        ///     Gets and sets the flexible label style.
        /// </summary>
        public GUIStyle FlexiLabelStyle { get; set; }

        /// <summary>
        ///     Gets and sets the help string which is shown in the editor.
        /// </summary>
        public string HelpString { get; set; }

        /// <summary>
        ///     Gets and sets whether the readout should be shown on new installs.
        /// </summary>
        public bool IsDefault { get; set; }

        /// <summary>
        ///     Gets the number of drawn lines.
        /// </summary>
        public int LineCount { get; private set; }

        /// <summary>
        ///     Gets and sets the message style.
        /// </summary>
        public GUIStyle MessageStyle { get; set; }

        /// <summary>
        ///     Gets and sets the readout name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Gets and sets the name style.
        /// </summary>
        public GUIStyle NameStyle { get; set; }

        /// <summary>
        ///     Gets and sets whether the readout has requested a section resize.
        /// </summary>
        public bool ResizeRequested { get; set; }

        /// <summary>
        ///     Gets and sets whether the help string is being shown in the editor.
        /// </summary>
        public bool ShowHelp { get; set; }

        /// <summary>
        ///     Gets and sets the text field style.
        /// </summary>
        public GUIStyle TextFieldStyle { get; set; }

        /// <summary>
        ///     Gets and sets the value style.
        /// </summary>
        public GUIStyle ValueStyle { get; set; }

        #endregion

        #region Methods: public

        /// <summary>
        ///     Called when a readout is asked to draw its self.
        /// </summary>
        public virtual void Draw(SectionModule section) { }

        /// <summary>
        ///     Called on each fixed update frame where the readout is visible.
        /// </summary>
        public virtual void FixedUpdate() { }

        public void LineCountEnd()
        {
            this.LineCount = this.lineCountEnd;
            if (this.lineCountEnd.CompareTo(this.lineCountStart) < 0)
            {
                this.ResizeRequested = true;
            }
        }

        public void LineCountStart()
        {
            this.lineCountStart = this.lineCountEnd;
            this.lineCountEnd = 0;
        }

        /// <summary>
        ///     Called when FlightEngineerCore is started.
        /// </summary>
        public virtual void Reset() { }

        /// <summary>
        ///     Called on each update frame when the readout is visible.
        /// </summary>
        public virtual void Update() { }

        #endregion

        #region Methods: protected

        protected void DrawLine(string value, bool compact = false)
        {
            GUILayout.BeginHorizontal(GUILayout.Width(this.ContentWidth));
            if (!compact)
            {
                GUILayout.Label(this.Name, this.NameStyle);
                GUILayout.FlexibleSpace();
                GUILayout.Label(value.ToLength(20), this.ValueStyle);
            }
            else
            {
                GUILayout.Label(this.Name, this.NameStyle, GUILayout.Height(this.NameStyle.fontSize * 1.2f));
                GUILayout.FlexibleSpace();
                GUILayout.Label(value.ToLength(20), this.ValueStyle, GUILayout.Height(this.ValueStyle.fontSize * 1.2f));
            }
            GUILayout.EndHorizontal();

            this.lineCountEnd++;
        }

        protected void DrawLine(string name, string value, bool compact = false)
        {
            GUILayout.BeginHorizontal(GUILayout.Width(this.ContentWidth));
            if (!compact)
            {
                GUILayout.Label(name, this.NameStyle);
                GUILayout.FlexibleSpace();
                GUILayout.Label(value.ToLength(20), this.ValueStyle);
            }
            else
            {
                GUILayout.Label(name, this.NameStyle, GUILayout.Height(this.NameStyle.fontSize * 1.2f));
                GUILayout.FlexibleSpace();
                GUILayout.Label(value.ToLength(20), this.ValueStyle, GUILayout.Height(this.ValueStyle.fontSize * 1.2f));
            }
            GUILayout.EndHorizontal();

            this.lineCountEnd++;
        }

        protected void DrawLine(Action drawAction, bool showName = true, bool compact = false)
        {
            GUILayout.BeginHorizontal(GUILayout.Width(this.ContentWidth));
            if (showName)
            {
                if (!compact)
                {
                    GUILayout.Label(this.Name, this.NameStyle);
                }
                else
                {
                    GUILayout.Label(this.Name, this.NameStyle, GUILayout.Height(this.NameStyle.fontSize * 1.2f));
                }
                GUILayout.FlexibleSpace();
            }
            drawAction();
            GUILayout.EndHorizontal();
            this.lineCountEnd++;
        }

        protected void DrawMessageLine(string value, bool compact = false)
        {
            GUILayout.BeginHorizontal(GUILayout.Width(this.ContentWidth));
            if (!compact)
            {
                GUILayout.Label(value, this.MessageStyle);
            }
            else
            {
                GUILayout.Label(value, this.MessageStyle, GUILayout.Height(this.MessageStyle.fontSize * 1.2f));
            }
            GUILayout.EndHorizontal();
            this.lineCountEnd++;
        }

        #endregion

        #region Methods: private

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
                fontSize = (int)(11 * GuiDisplaySize.Offset),
                fontStyle = FontStyle.Bold,
                fixedHeight = 20.0f * GuiDisplaySize.Offset
            };

            this.ValueStyle = new GUIStyle(HighLogic.Skin.label)
            {
                margin = new RectOffset(),
                padding = new RectOffset(0, 5, 0, 0),
                alignment = TextAnchor.MiddleRight,
                fontSize = (int)(11 * GuiDisplaySize.Offset),
                fontStyle = FontStyle.Normal,
                fixedHeight = 20.0f * GuiDisplaySize.Offset
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
                fontSize = (int)(11 * GuiDisplaySize.Offset),
                fontStyle = FontStyle.Normal,
                fixedHeight = 20.0f * GuiDisplaySize.Offset,
                stretchWidth = true
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
                fontSize = (int)(11 * GuiDisplaySize.Offset),
                fixedHeight = 18.0f * GuiDisplaySize.Offset
            };

            this.TextFieldStyle = new GUIStyle(HighLogic.Skin.textField)
            {
                margin = new RectOffset(0, 0, 1, 1),
                padding = new RectOffset(5, 5, 0, 0),
                alignment = TextAnchor.MiddleLeft,
                fontSize = (int)(11 * GuiDisplaySize.Offset),
                fixedHeight = 18.0f * GuiDisplaySize.Offset
            };
        }

        private void OnSizeChanged()
        {
            this.InitialiseStyles();
            this.ResizeRequested = true;
        }

        #endregion
    }
}