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
    [KSPAddon(KSPAddon.Startup.Flight, false)]
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
        private Rect windowPosition;

        #endregion

        #region Constructors

        /// <summary>
        ///     Sets the instance to this object.
        /// </summary>
        private void Awake()
        {
            try
            {
                if (Instance == null)
                {
                    Instance = this;
                    GuiDisplaySize.OnSizeChanged += this.OnSizeChanged;
                    Logger.Log("ActionMenu->Awake");
                }
                else
                {
                    Destroy(this);
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "DisplayStack->Awake");
            }
        }

        /// <summary>
        ///     Initialises the object's state on creation.
        /// </summary>
        private void Start()
        {
            try
            {
                this.windowId = this.GetHashCode();
                this.InitialiseStyles();
                this.Load();
                RenderingManager.AddToPostDrawQueue(0, this.Draw);
                Logger.Log("ActionMenu->Start");
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "DisplayStack->Start");
            }
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
            try
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
                    fontSize = (int)(13 * GuiDisplaySize.Offset),
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
                    fontSize = (int)(11 * GuiDisplaySize.Offset),
                    fontStyle = FontStyle.Bold,
                    fixedWidth = 60.0f * GuiDisplaySize.Offset,
                    fixedHeight = 25.0f * GuiDisplaySize.Offset,
                };
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "DisplayStack->InitialiseStyles");
            }
        }

        private void OnSizeChanged()
        {
            this.InitialiseStyles();
            this.RequestResize();
        }

        #endregion

        #region Updating

        private void Update()
        {
            try
            {
                if (FlightEngineerCore.Instance == null)
                {
                    return;
                }

                if (Input.GetKeyDown(KeyCode.Backslash))
                {
                    this.Hidden = !this.Hidden;
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "DisplayStack->Update");
            }
        }

        #endregion

        #region Drawing

        /// <summary>
        ///     Called to draw the display stack when the UI is enabled.
        /// </summary>
        private void Draw()
        {
            try
            {
                if (FlightEngineerCore.Instance == null)
                {
                    return;
                }

                if (this.resizeRequested || this.numberOfStackSections != SectionLibrary.Instance.NumberOfStackSections)
                {
                    this.numberOfStackSections = SectionLibrary.Instance.NumberOfStackSections;
                    this.windowPosition.width = 0;
                    this.windowPosition.height = 0;
                    this.resizeRequested = false;
                }

                if (!this.Hidden && (SectionLibrary.Instance.NumberOfStackSections > 0 || this.ShowControlBar))
                {
                    var shouldCentre = this.windowPosition.min == Vector2.zero;
                    GUI.skin = null;
                    this.windowPosition = GUILayout.Window(this.windowId, this.windowPosition, this.Window, string.Empty, this.windowStyle).ClampToScreen();
                    if (shouldCentre)
                    {
                        this.windowPosition.center = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "DisplayStack->Draw");
            }
        }

        /// <summary>
        ///     Draws the display stack window.
        /// </summary>
        private void Window(int windowId)
        {
            try
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
            catch (Exception ex)
            {
                Logger.Exception(ex, "DisplayStack->Widnow");
            }
        }

        /// <summary>
        ///     Draws the control bar.
        /// </summary>
        private void DrawControlBar()
        {
            try
            {
                GUILayout.Label("FLIGHT ENGINEER " + EngineerGlobals.AssemblyVersion, this.titleStyle);

                this.DrawControlBarButtons(SectionLibrary.Instance.StockSections);
                this.DrawControlBarButtons(SectionLibrary.Instance.CustomSections);
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "DisplayStack->DrawControlBar");
            }
        }

        /// <summary>
        ///     Draws a button list for a set of sections.
        /// </summary>
        private void DrawControlBarButtons(IEnumerable<SectionModule> sections)
        {
            try
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
            catch (Exception ex)
            {
                Logger.Exception(ex, "DisplayStack->DrawControlBarButtons");
            }
        }

        /// <summary>
        ///     Draws a list of sections.
        /// </summary>
        private void DrawSections(IEnumerable<SectionModule> sections)
        {
            try
            {
                foreach (var section in sections)
                {
                    if (!section.IsFloating)
                    {
                        section.Draw();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "DisplayStack->DrawSections");
            }
        }

        #endregion

        #region Destruction

        /// <summary>
        ///     Runs when the object is destroyed.
        /// </summary>
        private void OnDestroy()
        {
            try
            {
                this.Save();
                RenderingManager.RemoveFromPostDrawQueue(0, this.Draw);
                Logger.Log("ActionMenu->OnDestroy");
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "DisplayStack->OnDestroy");
            }
        }

        #endregion

        #region Saving and Loading

        /// <summary>
        ///     Saves the stack's state.
        /// </summary>
        private void Save()
        {
            try
            {
                var handler = new SettingHandler();
                handler.Set("showControlBar", this.ShowControlBar);
                handler.Set("windowPositionX", this.windowPosition.x);
                handler.Set("windowPositionY", this.windowPosition.y);
                handler.Save("DisplayStack.xml");
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "DisplayStack->Save");
            }
        }

        /// <summary>
        ///     Load the stack's state.
        /// </summary>
        private void Load()
        {
            try
            {
                var handler = SettingHandler.Load("DisplayStack.xml");
                this.ShowControlBar = handler.Get("showControlBar", this.ShowControlBar);
                this.windowPosition.x = handler.Get("windowPositionX", this.windowPosition.x);
                this.windowPosition.y = handler.Get("windowPositionY", this.windowPosition.y);
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "DisplayStack->Load");
            }
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