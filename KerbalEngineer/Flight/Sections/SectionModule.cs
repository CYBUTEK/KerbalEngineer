// Project:	KerbalEngineer
// Author:	CYBUTEK
// License:	Attribution-NonCommercial-ShareAlike 3.0 Unported

#region Using Directives

using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

using KerbalEngineer.Flight.Readouts;

using UnityEngine;

#endregion

namespace KerbalEngineer.Flight.Sections
{
    /// <summary>
    ///     Object for management and display of readout modules.
    /// </summary>
    public class SectionModule
    {
        #region Fields

        private SectionEditor editor;
        private int numberOfReadouts;

        #endregion

        #region Constructors

        /// <summary>
        ///     Creates a new section module.
        /// </summary>
        public SectionModule()
        {
            this.FloatingPositionX = Screen.width * 0.5f - 125.0f;
            this.FloatingPositionY = 100.0f;
            this.EditorPositionX = Screen.width * 0.5f - SectionEditor.Width * 0.5f;
            this.EditorPositionY = Screen.height * 0.5f - SectionEditor.Height * 0.5f;
            this.ReadoutModules = new List<ReadoutModule>();
            this.InitialiseStyles();
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets and sets the name of the section.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Gets and sets the abbreviation of the section.
        /// </summary>
        public string Abbreviation { get; set; }

        /// <summary>
        ///     Gets and sets the visibility of the section.
        /// </summary>
        public bool IsVisible { get; set; }

        /// <summary>
        ///     Gets and sets whether the section is custom.
        /// </summary>
        public bool IsCustom { get; set; }

        /// <summary>
        ///     Gets and sets the X position of the floating window. (Only used for serialisation.)
        /// </summary>
        public float FloatingPositionX { get; set; }

        /// <summary>
        ///     Gets and sets the Y position of the floating window. (Only used for serialisation.)
        /// </summary>
        public float FloatingPositionY { get; set; }

        /// <summary>
        ///     Gets and sets whether the section is in a floating state.
        /// </summary>
        public bool IsFloating
        {
            get { return this.Window != null; }
            set
            {
                if (value && this.Window == null)
                {
                    this.Window = FlightEngineerCore.Instance.AddSectionWindow(this);
                }
                else if (!value && this.Window != null)
                {
                    Object.Destroy(this.Window);
                }
            }
        }

        /// <summary>
        ///     Gets and sets the X position of the editor window. (Only used for serialisation.)
        /// </summary>
        public float EditorPositionX { get; set; }

        /// <summary>
        ///     Gets and sets the Y position of the editor window. (Only used for serialisation.)
        /// </summary>
        public float EditorPositionY { get; set; }

        /// <summary>
        ///     Gets and sets whether the section editor is visible.
        /// </summary>
        public bool IsEditorVisible
        {
            get { return this.editor != null; }
            set
            {
                if (value && this.editor == null)
                {
                    this.editor = FlightEngineerCore.Instance.AddSectionEditor(this);
                }
                else if (!value && this.editor != null)
                {
                    Object.Destroy(this.editor);
                }
            }
        }

        /// <summary>
        ///     Gets and sets the names of the installed readout modules. (Only used with serialisation.)
        /// </summary>
        public string[] ReadoutModuleNames
        {
            get { return this.ReadoutModules.Select(r => r.GetType().Name).ToArray(); }
            set { this.ReadoutModules = value.Select(n => ReadoutLibrary.Instance.GetReadoutModule(n)).ToList(); }
        }

        /// <summary>
        ///     Gets and sets the list of readout modules.
        /// </summary>
        [XmlIgnore] public List<ReadoutModule> ReadoutModules { get; set; }

        /// <summary>
        ///     Gets and sets the floating window.
        /// </summary>
        [XmlIgnore] public SectionWindow Window { get; set; }

        #endregion

        #region GUIStyles

        private GUIStyle boxStyle;
        private GUIStyle buttonStyle;
        private GUIStyle messageStyle;
        private GUIStyle titleStyle;

        /// <summary>
        ///     Initialises all the styles required for this object.
        /// </summary>
        private void InitialiseStyles()
        {
            this.boxStyle = new GUIStyle(HighLogic.Skin.box)
            {
                margin = new RectOffset(),
                padding = new RectOffset(5, 5, 5, 5)
            };

            this.titleStyle = new GUIStyle(HighLogic.Skin.label)
            {
                normal =
                {
                    textColor = Color.white
                },
                margin = new RectOffset(),
                padding = new RectOffset(2, 0, 5, 2),
                fontStyle = FontStyle.Bold,
                stretchWidth = true
            };

            this.buttonStyle = new GUIStyle(HighLogic.Skin.button)
            {
                normal =
                {
                    textColor = Color.white
                },
                margin = new RectOffset(0, 0, 5, 3),
                padding = new RectOffset(),
                fontSize = 10,
                stretchHeight = true
            };

            this.messageStyle = new GUIStyle(HighLogic.Skin.label)
            {
                normal =
                {
                    textColor = Color.white
                },
                margin = new RectOffset(),
                padding = new RectOffset(),
                alignment = TextAnchor.MiddleCenter,
                fontSize = 12,
                fontStyle = FontStyle.Bold,
                fixedWidth = 220.0f,
                fixedHeight = 20.0f
            };
        }

        #endregion

        #region Updating

        /// <summary>
        ///     Updates all of the internal readout modules.
        /// </summary>
        public void Update()
        {
            if (!this.IsVisible)
            {
                return;
            }

            foreach (var readout in this.ReadoutModules)
            {
                readout.Update();
            }

            if (this.numberOfReadouts != this.ReadoutModules.Count)
            {
                this.numberOfReadouts = this.ReadoutModules.Count;
                if (!this.IsFloating)
                {
                    DisplayStack.Instance.RequestResize();
                }
                else
                {
                    this.Window.RequestResize();
                }
            }
        }

        #endregion

        #region Drawing

        /// <summary>
        ///     Draws the section and all of the internal readout modules.
        /// </summary>
        public void Draw()
        {
            if (!this.IsVisible)
            {
                return;
            }

            this.DrawSectionTitleBar();
            this.DrawReadoutModules();
        }

        /// <summary>
        ///     Draws the section title and action buttons.
        /// </summary>
        private void DrawSectionTitleBar()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(this.Name.ToUpper(), this.titleStyle);
            this.IsEditorVisible = GUILayout.Toggle(this.IsEditorVisible, "EDIT", this.buttonStyle, GUILayout.Width(60.0f));
            this.IsFloating = GUILayout.Toggle(this.IsFloating, "FLOAT", this.buttonStyle, GUILayout.Width(60.0f));
            GUILayout.EndHorizontal();
        }

        /// <summary>
        ///     Draws all the readout modules.
        /// </summary>
        private void DrawReadoutModules()
        {
            GUILayout.BeginVertical(this.boxStyle);
            if (this.ReadoutModules.Count > 0)
            {
                foreach (var readout in this.ReadoutModules)
                {
                    readout.Draw();
                }
            }
            else
            {
                GUILayout.Label("No readouts are installed.", this.messageStyle);
            }
            GUILayout.EndVertical();
        }

        #endregion
    }
}