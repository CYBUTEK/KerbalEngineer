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
using KerbalEngineer.VesselSimulator;

using UnityEngine;

#endregion

namespace KerbalEngineer.Flight
{
    /// <summary>
    ///     Graphical controller for section interaction in the form of a menu system.
    /// </summary>
    public class ActionMenu : MonoBehaviour
    {
        #region Constants

        private const float ScrollSpeed = 2.0f;

        #endregion

        #region Fields

        private readonly int windowId = new Guid().GetHashCode();

        private bool isOpen = true;
        private int numberOfSections;
        private float scrollPercent;
        private Rect windowPosition = new Rect((Screen.width * 0.25f) - 100.0f, 0, 200.0f, 0);

        #endregion

        #region Constructors

        /// <summary>
        ///     Initialises object's state on creation.
        /// </summary>
        private void Start()
        {
            this.InitialiseStyles();
            this.Load();
            RenderingManager.AddToPostDrawQueue(0, this.Draw);
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

        #endregion

        #region Drawing

        /// <summary>
        ///     Called to draw the menu when the UI is enabled.
        /// </summary>
        private void Draw()
        {
            if (this.numberOfSections != SectionLibrary.Instance.NumberOfSections)
            {
                this.numberOfSections = SectionLibrary.Instance.NumberOfSections;
                this.windowPosition.height = 0;
            }
            this.windowPosition = GUILayout.Window(this.windowId, this.windowPosition, this.Window, string.Empty, this.windowStyle);
            this.ScrollMechanism();
        }

        /// <summary>
        ///     Draws the menu window.
        /// </summary>
        private void Window(int windowId)
        {
            GUILayout.BeginVertical(this.boxStyle);

            this.DrawControlBarButton();
            this.DrawSections(SectionLibrary.Instance.StockSections);
            this.DrawSections(SectionLibrary.Instance.CustomSections);
            this.DrawNewButton();

            GUILayout.EndVertical();

            if (GUILayout.Button("FLIGHT ENGINEER", this.buttonStyle))
            {
                this.isOpen = !this.isOpen;
            }
        }

        /// <summary>
        ///     Draws and performs the control bar button action.
        /// </summary>
        private void DrawControlBarButton()
        {
            if (GUILayout.Toggle(DisplayStack.Instance.ShowControlBar, "CONTROL BAR", this.buttonStyle) != DisplayStack.Instance.ShowControlBar)
            {
                DisplayStack.Instance.ShowControlBar = !DisplayStack.Instance.ShowControlBar;
                DisplayStack.Instance.RequestResize();
            }
        }

        /// <summary>
        ///     Draws an action list for the supplied sections.
        /// </summary>
        private void DrawSections(IEnumerable<SectionModule> sections)
        {
            foreach (var section in sections)
            {
                GUILayout.BeginHorizontal();
                section.IsVisible = GUILayout.Toggle(section.IsVisible, section.Name.ToUpper(), this.buttonStyle);
                section.IsEditorVisible = GUILayout.Toggle(section.IsEditorVisible, "EDIT", this.buttonStyle, GUILayout.Width(50.0f));
                GUILayout.EndHorizontal();
            }
        }

        /// <summary>
        ///     Draws and performs the new section button action.
        /// </summary>
        private void DrawNewButton()
        {
            GUILayout.BeginHorizontal();
            GUI.skin = HighLogic.Skin;
            SimManager.minSimTime = (long)GUILayout.HorizontalSlider(SimManager.minSimTime, 0, 1000.0f);
            GUI.skin = null;

            if (GUILayout.Button("NEW", this.buttonStyle, GUILayout.Width(50.0f)))
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

        /// <summary>
        ///     Controls the dynamics of the scrolling mechanism.
        /// </summary>
        private void ScrollMechanism()
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
        ///     Saves the menu's state.
        /// </summary>
        private void Save()
        {
            var handler = new SettingHandler();
            handler.Set("isOpen", this.isOpen);
            handler.Set("windowPositionY", this.windowPosition.y);
            handler.Set("windowPositionHeight", this.windowPosition.height);
            handler.Save("ActionMenu.xml");
        }

        /// <summary>
        ///     Loads the menu's state.
        /// </summary>
        private void Load()
        {
            var handler = SettingHandler.Load("ActionMenu.xml");
            handler.Get("isOpen", ref this.isOpen);
            this.windowPosition.y = handler.Get("windowPositionY", this.windowPosition.y);
            this.windowPosition.height = handler.Get("windowPositionHeight", this.windowPosition.height);
        }

        #endregion
    }
}