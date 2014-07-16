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

using System.Collections.Generic;

using KerbalEngineer.Extensions;
using KerbalEngineer.Flight.Sections;
using KerbalEngineer.Settings;

using UnityEngine;

#endregion

namespace KerbalEngineer.Flight
{
    /// <summary>
    ///     Graphical controller for displaying stacked sections.
    /// </summary>
    public class DisplayStack : MonoBehaviour
    {
        #region Instance

        /// <summary>
        ///     Gets the current instance of the DisplayStack.
        /// </summary>
        public static DisplayStack Instance { get; private set; }

        #endregion

        #region Fields

        private int numberOfStackSections;
        private bool resizeRequested;
        private int windowId;
        private Rect windowPosition = new Rect(Screen.width - 275.0f, 50.0f, 250.0f, 0);

        #endregion

        #region Constructors

        /// <summary>
        ///     Sets the instance to this object.
        /// </summary>
        private void Awake()
        {
            Instance = this;
        }

        /// <summary>
        ///     Initialises the object's state on creation.
        /// </summary>
        private void Start()
        {
            this.windowId = this.GetHashCode();
            this.InitialiseStyles();
            this.Load();

            RenderingManager.AddToPostDrawQueue(0, this.Draw);
        }

        #endregion

        #region Properties

        private bool showControlBar = true;

        /// <summary>
        ///     Gets and sets the visibility of the control bar.
        /// </summary>
        public bool ShowControlBar
        {
            get { return this.showControlBar; }
            set { this.showControlBar = value; }
        }

        public bool Hidden { get; set; }

        #endregion

        #region GUIStyles

        private GUIStyle buttonStyle;
        private GUIStyle titleStyle;
        private GUIStyle windowStyle;

        /// <summary>
        ///     Initialises all the styles required for this object.
        /// </summary>
        private void InitialiseStyles()
        {
            this.windowStyle = new GUIStyle(HighLogic.Skin.window)
            {
                margin = new RectOffset(),
                padding = new RectOffset(5, 5, 0, 5)
            };

            this.titleStyle = new GUIStyle(HighLogic.Skin.label)
            {
                margin = new RectOffset(0, 0, 5, 3),
                padding = new RectOffset(),
                alignment = TextAnchor.MiddleCenter,
                fontSize = 13,
                fontStyle = FontStyle.Bold,
                stretchWidth = true
            };

            this.buttonStyle = new GUIStyle(HighLogic.Skin.button)
            {
                normal =
                {
                    textColor = Color.white
                },
                margin = new RectOffset(),
                padding = new RectOffset(),
                alignment = TextAnchor.MiddleCenter,
                fontSize = 11,
                fontStyle = FontStyle.Bold,
                fixedWidth = 60.0f,
                fixedHeight = 25.0f,
            };
        }

        #endregion

        #region Updating

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Backslash))
            {
                this.Hidden = !this.Hidden;
            }
        }

        #endregion

        #region Drawing

        /// <summary>
        ///     Called to draw the display stack when the UI is enabled.
        /// </summary>
        private void Draw()
        {
            if (this.resizeRequested || this.numberOfStackSections != SectionLibrary.Instance.NumberOfStackSections)
            {
                this.numberOfStackSections = SectionLibrary.Instance.NumberOfStackSections;
                this.windowPosition.height = 0;
                this.resizeRequested = false;
            }

            if (!this.Hidden && (SectionLibrary.Instance.NumberOfStackSections > 0 || this.ShowControlBar))
            {
                this.windowPosition = GUILayout.Window(this.windowId, this.windowPosition, this.Window, string.Empty, this.windowStyle).ClampToScreen();
            }
        }

        /// <summary>
        ///     Draws the display stack window.
        /// </summary>
        private void Window(int windowId)
        {
            if (this.ShowControlBar)
            {
                this.DrawControlBar();
            }

            if (SectionLibrary.Instance.NumberOfStackSections > 0)
            {
                this.DrawSections(SectionLibrary.Instance.StockSections);
                this.DrawSections(SectionLibrary.Instance.CustomSections);
            }

            GUI.DragWindow();
        }

        /// <summary>
        ///     Draws the control bar.
        /// </summary>
        private void DrawControlBar()
        {
            GUILayout.Label("FLIGHT ENGINEER " + EngineerGlobals.PrettyVersion, this.titleStyle);

            this.DrawControlBarButtons(SectionLibrary.Instance.StockSections);
            this.DrawControlBarButtons(SectionLibrary.Instance.CustomSections);
        }

        /// <summary>
        ///     Draws a button list for a set of sections.
        /// </summary>
        private void DrawControlBarButtons(IEnumerable<SectionModule> sections)
        {
            var index = 0;
            foreach (var section in sections)
            {
                if (index % 4 == 0)
                {
                    if (index > 0)
                    {
                        GUILayout.EndHorizontal();
                    }
                    GUILayout.BeginHorizontal();
                }
                section.IsVisible = GUILayout.Toggle(section.IsVisible, section.Abbreviation.ToUpper(), this.buttonStyle);
                index++;
            }
            if (index > 0)
            {
                GUILayout.EndHorizontal();
            }
        }

        /// <summary>
        ///     Draws a list of sections.
        /// </summary>
        private void DrawSections(IEnumerable<SectionModule> sections)
        {
            foreach (var section in sections)
            {
                if (!section.IsFloating)
                {
                    section.Draw();
                }
            }
        }

        #endregion

        #region Destruction

        /// <summary>
        ///     Runs when the object is destroyed.
        /// </summary>
        private void OnDestroy()
        {
            this.Save();

            RenderingManager.RemoveFromPostDrawQueue(0, this.Draw);
        }

        #endregion

        #region Saving and Loading

        /// <summary>
        ///     Saves the stack's state.
        /// </summary>
        private void Save()
        {
            var handler = new SettingHandler();
            handler.Set("showControlBar", this.ShowControlBar);
            handler.Set("windowPositionX", this.windowPosition.x);
            handler.Set("windowPositionY", this.windowPosition.y);
            handler.Save("DisplayStack.xml");
        }

        /// <summary>
        ///     Load the stack's state.
        /// </summary>
        private void Load()
        {
            var handler = SettingHandler.Load("DisplayStack.xml");
            this.ShowControlBar = handler.Get("showControlBar", this.ShowControlBar);
            this.windowPosition.x = handler.Get("windowPositionX", this.windowPosition.x);
            this.windowPosition.y = handler.Get("windowPositionY", this.windowPosition.y);
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Request that the display stack's size is reset in the next draw call.
        /// </summary>
        public void RequestResize()
        {
            this.resizeRequested = true;
        }

        #endregion
    }
}