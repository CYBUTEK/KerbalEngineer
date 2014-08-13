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

using KerbalEngineer.Extensions;
using KerbalEngineer.Flight.Readouts;
using KerbalEngineer.UIControls;

using UnityEngine;

#endregion

namespace KerbalEngineer.Flight.Sections
{
    public class SectionEditor : MonoBehaviour
    {
        #region Constants

        public const float Width = 500.0f;
        public const float Height = 500.0f;

        #endregion

        #region Fields

        private Vector2 scrollPositionAvailable;
        private Vector2 scrollPositionInstalled;
        private Rect position;
        private DropDown categoryList;

        #endregion

        #region Constructors

        private void Awake()
        {
            try
            {
                this.categoryList = this.gameObject.AddComponent<DropDown>();
                this.categoryList.DrawCallback = this.DrawCategories;
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }

        /// <summary>
        ///     Initialises the object's state on creation.
        /// </summary>
        private void Start()
        {
            this.InitialiseStyles();
            ReadoutCategory.Selected = ReadoutCategory.GetCategory("Orbital");
            RenderingManager.AddToPostDrawQueue(0, this.Draw);
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets and sets the parent section for the section editor.
        /// </summary>
        public SectionModule ParentSection { get; set; }

        /// <summary>
        ///     Gets and sets the window position.
        /// </summary>
        public Rect Position
        {
            get { return this.position; }
            set { this.position = value; }
        }

        #endregion

        #region GUIStyles

        private GUIStyle categoryTitleButtonStyle;
        private GUIStyle categoryButtonStyle;
        private GUIStyle categoryButtonActiveStyle;
        private GUIStyle helpBoxStyle;
        private GUIStyle helpTextStyle;
        private GUIStyle panelTitleStyle;
        private GUIStyle readoutButtonStyle;
        private GUIStyle readoutNameStyle;
        private GUIStyle textStyle;
        private GUIStyle windowStyle;

        /// <summary>
        ///     Initialises all the styles required for this object.
        /// </summary>
        private void InitialiseStyles()
        {
            this.windowStyle = new GUIStyle(HighLogic.Skin.window);

            this.categoryButtonStyle = new GUIStyle(HighLogic.Skin.button)
            {
                normal =
                {
                    textColor = Color.white
                },
                margin = new RectOffset(0,0,2,0),
                padding = new RectOffset(5,5,5,5),
                alignment = TextAnchor.MiddleCenter,
                fontSize = 12,
                fontStyle = FontStyle.Normal,
                richText = true
            };

            this.categoryButtonActiveStyle = new GUIStyle(this.categoryButtonStyle)
            {
                normal = this.categoryButtonStyle.onNormal,
                hover = this.categoryButtonStyle.onHover
            };

            this.panelTitleStyle = new GUIStyle(HighLogic.Skin.label)
            {
                normal =
                {
                    textColor = Color.white
                },
                margin = new RectOffset(),
                padding = new RectOffset(),
                alignment = TextAnchor.MiddleLeft,
                fontSize = 12,
                fontStyle = FontStyle.Bold,
                fixedHeight = 30.0f,
                stretchWidth = true
            };

            this.textStyle = new GUIStyle(HighLogic.Skin.textField)
            {
                margin = new RectOffset(3, 3, 3, 3),
                alignment = TextAnchor.MiddleLeft,
                stretchWidth = true,
                stretchHeight = true
            };

            this.readoutNameStyle = new GUIStyle(HighLogic.Skin.label)
            {
                normal =
                {
                    textColor = Color.white
                },
                margin = new RectOffset(),
                padding = new RectOffset(10, 0, 0, 0),
                alignment = TextAnchor.MiddleLeft,
                fontSize = 12,
                fontStyle = FontStyle.Bold,
                stretchWidth = true,
                stretchHeight = true
            };

            this.readoutButtonStyle = new GUIStyle(HighLogic.Skin.button)
            {
                normal =
                {
                    textColor = Color.white
                },
                margin = new RectOffset(2, 2, 2, 2),
                padding = new RectOffset(),
                alignment = TextAnchor.MiddleCenter,
                fontSize = 12,
                fontStyle = FontStyle.Bold,
                stretchHeight = true
            };

            this.helpBoxStyle = new GUIStyle(HighLogic.Skin.box)
            {
                margin = new RectOffset(2, 2, 2, 10),
                padding = new RectOffset(10, 10, 10, 10)
            };

            this.helpTextStyle = new GUIStyle(HighLogic.Skin.label)
            {
                normal =
                {
                    textColor = Color.yellow
                },
                margin = new RectOffset(),
                padding = new RectOffset(),
                alignment = TextAnchor.MiddleLeft,
                fontSize = 13,
                fontStyle = FontStyle.Normal,
                stretchWidth = true,
                richText = true
            };

            this.categoryTitleButtonStyle = new GUIStyle(this.readoutButtonStyle)
            {
                fixedHeight = 30.0f,
                stretchHeight = false
            };
        }

        #endregion

        #region Drawing

        /// <summary>
        ///     Called to draw the editor when the UI is enabled.
        /// </summary>
        private void Draw()
        {
            this.position = GUILayout.Window(this.GetInstanceID(), this.position, this.Window, "EDIT SECTION - " + this.ParentSection.Name.ToUpper(), this.windowStyle).ClampToScreen();
            this.ParentSection.EditorPositionX = this.position.x;
            this.ParentSection.EditorPositionY = this.position.y;
        }

        /// <summary>
        ///     Draws the editor window.
        /// </summary>
        private void Window(int windowId)
        {
            this.DrawCustomOptions();
            this.DrawCategorySelector();
            this.DrawAvailableReadouts();
            GUILayout.Space(5.0f);
            this.DrawInstalledReadouts();

            if (GUILayout.Button("CLOSE EDITOR", this.categoryTitleButtonStyle))
            {
                this.ParentSection.IsEditorVisible = false;
            }

            GUI.DragWindow();
        }

        /// <summary>
        ///     Draws the options for editing custom sections.
        /// </summary>
        private void DrawCustomOptions()
        {
            if (!this.ParentSection.IsCustom)
            {
                return;
            }

            GUILayout.BeginHorizontal(GUILayout.Height(25.0f));
            this.ParentSection.Name = GUILayout.TextField(this.ParentSection.Name, this.textStyle);
            this.ParentSection.Abbreviation = GUILayout.TextField(this.ParentSection.Abbreviation, this.textStyle, GUILayout.Width(75.0f));
            if (GUILayout.Button("DELETE SECTION", this.readoutButtonStyle, GUILayout.Width(125.0f)))
            {
                this.ParentSection.IsFloating = false;
                this.ParentSection.IsEditorVisible = false;
                SectionLibrary.CustomSections.Remove(this.ParentSection);
                DisplayStack.Instance.RequestResize();
            }
            GUILayout.EndHorizontal();
        }

        /// <summary>
        ///     Draws the readoutCategories selection list.
        /// </summary>
        private void DrawCategorySelector()
        {
            this.categoryList.enabled = GUILayout.Toggle(this.categoryList.enabled, "▼ SELECTED CATEGORY: " + ReadoutCategory.Selected.ToString().ToUpper() + " ▼", this.categoryTitleButtonStyle);
            if (Event.current.type == EventType.repaint)
            {
                this.categoryList.SetPosition(GUILayoutUtility.GetLastRect().Translate(this.position));
            }
        }

        private void DrawCategories()
        {
            foreach (var category in ReadoutCategory.Categories)
            {
                if (GUILayout.Button("<b>" + category.Name.ToUpper() + "</b>" + (string.IsNullOrEmpty(category.Description) ? string.Empty : "\n<i>" + category.Description + "</i>"), category == ReadoutCategory.Selected ? this.categoryButtonActiveStyle : this.categoryButtonStyle))
                {
                    ReadoutCategory.Selected = category;
                    this.categoryList.enabled = false;
                }
            }
        }

        /// <summary>
        ///     Draws the available readouts panel.
        /// </summary>
        private void DrawAvailableReadouts()
        {
            GUI.skin = HighLogic.Skin;
            this.scrollPositionAvailable = GUILayout.BeginScrollView(this.scrollPositionAvailable, false, true, GUILayout.Height(200.0f));
            GUI.skin = null;

            GUILayout.Label("AVAILABLE", this.panelTitleStyle);

            foreach (var readout in ReadoutLibrary.GetCategory(ReadoutCategory.Selected))
            {
                if (!this.ParentSection.ReadoutModules.Contains(readout) || readout.Cloneable)
                {
                    GUILayout.BeginHorizontal(GUILayout.Height(30.0f));
                    GUILayout.Label(readout.Name, this.readoutNameStyle);
                    readout.ShowHelp = GUILayout.Toggle(readout.ShowHelp, "?", this.readoutButtonStyle, GUILayout.Width(30.0f));
                    if (GUILayout.Button("INSTALL", this.readoutButtonStyle, GUILayout.Width(125.0f)))
                    {
                        this.ParentSection.ReadoutModules.Add(readout);
                    }
                    GUILayout.EndHorizontal();

                    this.ShowHelpMessage(readout);
                }
            }

            GUILayout.EndScrollView();
        }

        /// <summary>
        ///     Draws the installed readouts panel.
        /// </summary>
        private void DrawInstalledReadouts()
        {
            GUI.skin = HighLogic.Skin;
            this.scrollPositionInstalled = GUILayout.BeginScrollView(this.scrollPositionInstalled, false, true);
            GUI.skin = null;

            GUILayout.Label("INSTALLED", this.panelTitleStyle);
            var removeReadout = false;
            var removeReadoutIndex = 0;

            for (var i = 0; i < this.ParentSection.ReadoutModules.Count; i++)
            {
                var readout = this.ParentSection.ReadoutModules[i];

                GUILayout.BeginHorizontal(GUILayout.Height(30.0f));
                GUILayout.Label(readout.Name, this.readoutNameStyle);
                if (GUILayout.Button("▲", this.readoutButtonStyle, GUILayout.Width(30.0f)))
                {
                    if (i > 0)
                    {
                        this.ParentSection.ReadoutModules[i] = this.ParentSection.ReadoutModules[i - 1];
                        this.ParentSection.ReadoutModules[i - 1] = readout;
                    }
                }
                if (GUILayout.Button("▼", this.readoutButtonStyle, GUILayout.Width(30.0f)))
                {
                    if (i < this.ParentSection.ReadoutModules.Count - 1)
                    {
                        this.ParentSection.ReadoutModules[i] = this.ParentSection.ReadoutModules[i + 1];
                        this.ParentSection.ReadoutModules[i + 1] = readout;
                    }
                }
                readout.ShowHelp = GUILayout.Toggle(readout.ShowHelp, "?", this.readoutButtonStyle, GUILayout.Width(30.0f));
                if (GUILayout.Button("REMOVE", this.readoutButtonStyle, GUILayout.Width(125.0f)))
                {
                    removeReadout = true;
                    removeReadoutIndex = i;
                }
                GUILayout.EndHorizontal();

                this.ShowHelpMessage(readout);
            }

            GUILayout.EndScrollView();

            if (removeReadout)
            {
                this.ParentSection.ReadoutModules.RemoveAt(removeReadoutIndex);
            }
        }

        private void ShowHelpMessage(ReadoutModule readout)
        {
            if (readout.ShowHelp)
            {
                GUILayout.BeginVertical(this.helpBoxStyle);
                if (readout.HelpString != null && readout.HelpString.Length > 0)
                {
                    GUILayout.Label(readout.HelpString, this.helpTextStyle);
                }
                else
                {
                    GUILayout.Label("Sorry, no help information has been provided for this readout module.", this.helpTextStyle);
                }

                GUILayout.EndVertical();
            }
        }

        #endregion

        #region Destruction

        /// <summary>
        ///     Runs when the object is destroyed.
        /// </summary>
        private void OnDestroy()
        {
            RenderingManager.RemoveFromPostDrawQueue(0, this.Draw);
        }

        #endregion
    }
}