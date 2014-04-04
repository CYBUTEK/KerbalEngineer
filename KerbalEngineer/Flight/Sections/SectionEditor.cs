// Project:	KerbalEngineer
// Author:	CYBUTEK
// License:	Attribution-NonCommercial-ShareAlike 3.0 Unported

#region Using Directives

using KerbalEngineer.Extensions;
using KerbalEngineer.Flight.Readouts;

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
        private ReadoutCategory selectedCategory = ReadoutCategory.Orbital;
        private int windowId;
        private Rect windowPosition;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initialises the object's state on creation.
        /// </summary>
        private void Start()
        {
            this.windowId = this.GetHashCode();
            this.InitialiseStyles();
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
        public Rect WindowPosition
        {
            get { return this.windowPosition; }
            set { this.windowPosition = value; }
        }

        #endregion

        #region GUIStyles

        private GUIStyle categoryButtonStyle;
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
                alignment = TextAnchor.MiddleCenter,
                fontSize = 12,
                fontStyle = FontStyle.Bold,
                fixedHeight = 30.0f,
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
        }

        #endregion

        #region Drawing

        /// <summary>
        ///     Called to draw the editor when the UI is enabled.
        /// </summary>
        private void Draw()
        {
            this.windowPosition = GUILayout.Window(this.windowId, this.windowPosition, this.Window, "EDIT SECTION - " + this.ParentSection.Name.ToUpper(), this.windowStyle).ClampToScreen();
            this.ParentSection.EditorPositionX = this.windowPosition.x;
            this.ParentSection.EditorPositionY = this.windowPosition.y;
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

            if (GUILayout.Button("CLOSE EDITOR", this.categoryButtonStyle))
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
                SectionLibrary.Instance.CustomSections.Remove(this.ParentSection);
                DisplayStack.Instance.RequestResize();
            }
            GUILayout.EndHorizontal();
        }

        /// <summary>
        ///     Draws the category selection list.
        /// </summary>
        private void DrawCategorySelector()
        {
            GUILayout.BeginHorizontal();
            var isSelected = this.selectedCategory == ReadoutCategory.Orbital;
            if (GUILayout.Toggle(isSelected, ReadoutCategory.Orbital.ToString().ToUpper(), this.categoryButtonStyle, GUILayout.Width(100.0f)) && !isSelected)
            {
                this.selectedCategory = ReadoutCategory.Orbital;
            }

            isSelected = this.selectedCategory == ReadoutCategory.Surface;
            if (GUILayout.Toggle(isSelected, ReadoutCategory.Surface.ToString().ToUpper(), this.categoryButtonStyle, GUILayout.Width(100.0f)) && !isSelected)
            {
                this.selectedCategory = ReadoutCategory.Surface;
            }

            isSelected = this.selectedCategory == ReadoutCategory.Vessel;
            if (GUILayout.Toggle(isSelected, ReadoutCategory.Vessel.ToString().ToUpper(), this.categoryButtonStyle, GUILayout.Width(100.0f)) && !isSelected)
            {
                this.selectedCategory = ReadoutCategory.Vessel;
            }

            isSelected = this.selectedCategory == ReadoutCategory.Rendezvous;
            if (GUILayout.Toggle(isSelected, ReadoutCategory.Rendezvous.ToString().ToUpper(), this.categoryButtonStyle, GUILayout.Width(100.0f)) && !isSelected)
            {
                this.selectedCategory = ReadoutCategory.Rendezvous;
            }

            isSelected = this.selectedCategory == ReadoutCategory.Misc;
            if (GUILayout.Toggle(isSelected, ReadoutCategory.Misc.ToString().ToUpper(), this.categoryButtonStyle) && !isSelected)
            {
                this.selectedCategory = ReadoutCategory.Misc;
            }
            GUILayout.EndHorizontal();
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

            foreach (var readout in ReadoutLibrary.Instance.GetCategory(this.selectedCategory))
            {
                if (!this.ParentSection.ReadoutModules.Contains(readout))
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
            ReadoutModule removeReadout = null;
            foreach (var readout in this.ParentSection.ReadoutModules)
            {
                GUILayout.BeginHorizontal(GUILayout.Height(30.0f));
                GUILayout.Label(readout.Name, this.readoutNameStyle);
                if (GUILayout.Button("▲", this.readoutButtonStyle, GUILayout.Width(30.0f)))
                {
                    var index = this.ParentSection.ReadoutModules.IndexOf(readout);
                    if (index > 0)
                    {
                        this.ParentSection.ReadoutModules[index] = this.ParentSection.ReadoutModules[index - 1];
                        this.ParentSection.ReadoutModules[index - 1] = readout;
                    }
                }
                if (GUILayout.Button("▼", this.readoutButtonStyle, GUILayout.Width(30.0f)))
                {
                    var index = this.ParentSection.ReadoutModules.IndexOf(readout);
                    if (index < this.ParentSection.ReadoutModules.Count - 1)
                    {
                        this.ParentSection.ReadoutModules[index] = this.ParentSection.ReadoutModules[index + 1];
                        this.ParentSection.ReadoutModules[index + 1] = readout;
                    }
                }
                readout.ShowHelp = GUILayout.Toggle(readout.ShowHelp, "?", this.readoutButtonStyle, GUILayout.Width(30.0f));
                if (GUILayout.Button("REMOVE", this.readoutButtonStyle, GUILayout.Width(125.0f)))
                {
                    removeReadout = readout;
                }
                GUILayout.EndHorizontal();

                this.ShowHelpMessage(readout);
            }

            GUILayout.EndScrollView();

            if (removeReadout != null)
            {
                this.ParentSection.ReadoutModules.Remove(removeReadout);
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