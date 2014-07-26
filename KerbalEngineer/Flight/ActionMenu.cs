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

using KerbalEngineer.Flight.Sections;
using KerbalEngineer.Settings;

using UnityEngine;

#endregion

namespace KerbalEngineer.Flight
{
    /// <summary>
    ///     Graphical controller for section interaction in the form of a menu system.
    /// </summary>
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class ActionMenu : MonoBehaviour
    {
        #region Constants

        private const float ScrollSpeed = 2.0f;

        #endregion

        #region Fields

        private bool isOpen;
        private int numberOfSections;
        private float scrollPercent;
        private int windowId;
        private Rect windowPosition = new Rect(Screen.width - 250.0f, 40.0f, 250.0f, 0);
        private ApplicationLauncherButton button; 

        #endregion

        #region Constructors

        private void Awake()
        {
            GameEvents.onGUIApplicationLauncherReady.Add(this.OnGuiAppLauncherReady);
            Logger.Log("ActionMenu->Awake");
        }

        /// <summary>
        ///     Initialises object's state on creation.
        /// </summary>
        private void Start()
        {
            this.windowId = this.GetHashCode();
            this.InitialiseStyles();
            this.Load();
            RenderingManager.AddToPostDrawQueue(0, this.Draw);
            Logger.Log("ActionMenu->Start");
        }

        private void OnGuiAppLauncherReady()
        {
            try
            {
                this.button = ApplicationLauncher.Instance.AddModApplication(
                    () => this.isOpen = true,
                    () => this.isOpen = false,
                    null,
                    null,
                    null,
                    null,
                    ApplicationLauncher.AppScenes.ALWAYS,
                    GameDatabase.Instance.GetTexture("KerbalEngineer/ToolbarIcon", false)
                    );
                Logger.Log("ActionMenu->OnGuiAppLauncherReady");
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "ActionMenu->OnGuiAppLauncherReady");
            }
        }

        #endregion

        #region GUIStyles

        private GUIStyle boxStyle;
        private GUIStyle buttonStyle;
        private GUIStyle windowStyle;

        /// <summary>
        ///     Initialises all the styles required for this object.
        /// </summary>
        private void InitialiseStyles()
        {
            try
            {
                this.windowStyle = new GUIStyle();

                this.boxStyle = new GUIStyle(HighLogic.Skin.window)
                {
                    margin = new RectOffset(),
                    padding = new RectOffset(3, 3, 3, 3)
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
                    fixedHeight = 20.0f,
                };
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "ActionMenu->InitialiseStyles");
            }
        }

        #endregion

        #region Updating

        private void Update()
        {
            try
            {
                if (this.button == null)
                {
                    return;
                }

                if (FlightEngineerCore.Instance != null)
                {
                    if (this.isOpen && this.button.State != RUIToggleButton.ButtonState.TRUE)
                    {
                        this.button.SetTrue();
                    }
                    else if (!this.isOpen && this.button.State != RUIToggleButton.ButtonState.FALSE)
                    {
                        this.button.SetFalse();
                    }
                }
                else if (this.button.State != RUIToggleButton.ButtonState.DISABLED)
                {
                    this.button.Disable();
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "ActionMenu->Update");
            }
        }

        #endregion

        #region Drawing

        /// <summary>
        ///     Called to draw the menu when the UI is enabled.
        /// </summary>
        private void Draw()
        {
            try
            {
                if (!this.isOpen || (this.button != null && this.button.State == RUIToggleButton.ButtonState.DISABLED))
                {
                    return;
                }

                if (this.numberOfSections != SectionLibrary.Instance.NumberOfSections)
                {
                    this.numberOfSections = SectionLibrary.Instance.NumberOfSections;
                    this.windowPosition.height = 0;
                }

                this.windowPosition = GUILayout.Window(this.windowId, this.windowPosition, this.Window, string.Empty, this.windowStyle);
                //this.ScrollMechanism();
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "ActionMenu->Draw");
            }
        }

        /// <summary>
        ///     Draws the menu window.
        /// </summary>
        private void Window(int windowId)
        {
            try
            {
                GUILayout.BeginVertical(this.boxStyle);

                this.DrawControlBarButton();
                GUILayout.Space(5.0f);
                this.DrawSections(SectionLibrary.Instance.StockSections);
                this.DrawSections(SectionLibrary.Instance.CustomSections);
                GUILayout.Space(5.0f);
                this.DrawNewButton();

                GUILayout.EndVertical();
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "ActionMenu->Window");
            }
        }

        /// <summary>
        ///     Draws and performs the control bar button action.
        /// </summary>
        private void DrawControlBarButton()
        {
            try
            {
                if (GUILayout.Toggle(DisplayStack.Instance.ShowControlBar, "CONTROL BAR", this.buttonStyle) != DisplayStack.Instance.ShowControlBar)
                {
                    DisplayStack.Instance.ShowControlBar = !DisplayStack.Instance.ShowControlBar;
                    DisplayStack.Instance.RequestResize();
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "ActionMenu->DrawControlBarButton");
            }
        }

        /// <summary>
        ///     Draws an action list for the supplied sections.
        /// </summary>
        private void DrawSections(IEnumerable<SectionModule> sections)
        {
            try
            {
                foreach (var section in sections)
                {
                    GUILayout.BeginHorizontal();
                    section.IsVisible = GUILayout.Toggle(section.IsVisible, section.Name.ToUpper(), this.buttonStyle);
                    section.IsEditorVisible = GUILayout.Toggle(section.IsEditorVisible, "EDIT", this.buttonStyle, GUILayout.Width(50.0f));
                    GUILayout.EndHorizontal();
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "ActionMenu->DrawSections");
            }
        }

        /// <summary>
        ///     Draws and performs the new section button action.
        /// </summary>
        private void DrawNewButton()
        {
            try
            {
                GUILayout.BeginHorizontal();

                if (GUILayout.Button("NEW CUSTOM SECTION", this.buttonStyle))
                {
                    SectionLibrary.Instance.CustomSections.Add(new SectionModule
                    {
                        Name = "Custom " + (SectionLibrary.Instance.CustomSections.Count + 1),
                        Abbreviation = "CUST " + (SectionLibrary.Instance.CustomSections.Count + 1),
                        IsVisible = true,
                        IsCustom = true,
                        IsEditorVisible = true
                    });
                }
                GUILayout.EndHorizontal();
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "ActionMenu->DrawNewButton");
            }
        }

        /// <summary>
        ///     Controls the dynamics of the scrolling mechanism.
        /// </summary>
        private void ScrollMechanism()
        {
            try
            {
                if (this.isOpen && this.windowPosition.y != 0)
                {
                    this.scrollPercent += Time.deltaTime * ScrollSpeed;
                    this.windowPosition.y = Mathf.Lerp(this.windowPosition.y, 0, this.scrollPercent);
                }
                else if (!this.isOpen && this.windowPosition.y != 20.0f - this.windowPosition.height)
                {
                    this.scrollPercent += Time.deltaTime * ScrollSpeed;
                    this.windowPosition.y = Mathf.Lerp(this.windowPosition.y, 20.0f - this.windowPosition.height, this.scrollPercent);
                }
                else
                {
                    this.scrollPercent = 0;
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "ActionMenu->ScrollMechanism");
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
                GameEvents.onGUIApplicationLauncherReady.Remove(this.OnGuiAppLauncherReady);
                if (this.button != null)
                {
                    ApplicationLauncher.Instance.RemoveModApplication(this.button);
                }
                Logger.Log("ActionMenu->OnDestroy");
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "ActionMenu->OnDestroy");
            }
        }

        #endregion

        #region Saving and Loading

        /// <summary>
        ///     Saves the menu's state.
        /// </summary>
        private void Save()
        {
            try
            {
                var handler = new SettingHandler();
                handler.Set("isOpen", this.isOpen);
                handler.Set("windowPositionY", this.windowPosition.y);
                handler.Set("windowPositionHeight", this.windowPosition.height);
                handler.Save("ActionMenu.xml");
                Logger.Log("ActionMenu->Save");
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "ActionMenu->Save");
            }
        }

        /// <summary>
        ///     Loads the menu's state.
        /// </summary>
        private void Load()
        {
            try
            {
                var handler = SettingHandler.Load("ActionMenu.xml");
                handler.Get("isOpen", ref this.isOpen);
                this.windowPosition.y = handler.Get("windowPositionY", this.windowPosition.y);
                this.windowPosition.height = handler.Get("windowPositionHeight", this.windowPosition.height);
                Logger.Log("ActionMenu->Load");
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "ActionMenu->Load");
            }
        }

        #endregion
    }
}