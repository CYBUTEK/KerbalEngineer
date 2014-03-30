// Project:	KerbalEngineer
// Author:	CYBUTEK
// License:	Attribution-NonCommercial-ShareAlike 3.0 Unported

#region Using Directives

using System.Collections.Generic;
using System.IO;
using System.Linq;

using KerbalEngineer.Settings;

using UnityEngine;

#endregion

namespace KerbalEngineer.FlightEngineer
{
    public class Section
    {
        #region Properties

        private readonly EditDisplay editDisplay;
        private readonly SectionWindow window;

        private List<ReadoutCategory> categories = new List<ReadoutCategory>();
        private string fileName = string.Empty;
        private List<Readout> readouts = new List<Readout>();
        private string shortTitle = string.Empty;
        private string title = string.Empty;
        private bool visible;

        /// <summary>
        ///     Gets the GUIStyle for the section title.
        /// </summary>
        protected GUIStyle TitleStyle { get; private set; }

        /// <summary>
        ///     Gets the GUIStyle for the section area.
        /// </summary>
        protected GUIStyle AreaStyle { get; private set; }

        /// <summary>
        ///     Gets the GUIStyle for section message labels.
        /// </summary>
        protected GUIStyle LabelStyle { get; private set; }

        /// <summary>
        ///     Gets and sets the readouts to be displayed.
        /// </summary>
        public List<Readout> Readouts
        {
            get { return this.readouts; }
            set { this.readouts = value; }
        }

        /// <summary>
        ///     Gets and sets whether the section is visible.
        /// </summary>
        public bool Visible
        {
            get { return this.visible; }
            set
            {
                if (this.visible != value)
                {
                    FlightDisplay.Instance.RequireResize = true;
                }

                this.visible = value;
            }
        }

        /// <summary>
        ///     Gets and sets the section title.
        /// </summary>
        public string Title
        {
            get { return this.title; }
            set { this.title = value; }
        }

        /// <summary>
        ///     Gets and sets the section short title.
        /// </summary>
        public string ShortTitle
        {
            get { return this.shortTitle; }
            set { this.shortTitle = value; }
        }

        /// <summary>
        ///     Gets and sets the filename of the section.
        /// </summary>
        public string FileName
        {
            get { return this.fileName; }
            set { this.fileName = value; }
        }

        /// <summary>
        ///     Gets and sets whether the section was user created.
        /// </summary>
        public bool IsUser { get; set; }

        /// <summary>
        ///     Gets the edit display associated with the section.
        /// </summary>
        public EditDisplay EditDisplay
        {
            get { return this.editDisplay; }
        }

        /// <summary>
        ///     Gets the section window object.
        /// </summary>
        public SectionWindow Window
        {
            get { return this.window; }
        }

        /// <summary>
        ///     Gets and sets the categories associated with the section.
        /// </summary>
        public List<ReadoutCategory> Categories
        {
            get { return this.categories; }
            set { this.categories = value; }
        }

        #endregion

        #region Initialisation

        public Section(bool isUserSection = false, bool isNewSection = true)
        {
            this.IsUser = false;
            this.editDisplay = HighLogic.fetch.gameObject.AddComponent<EditDisplay>();
            this.editDisplay.Section = this;
            RenderingManager.AddToPostDrawQueue(0, this.editDisplay.Draw);

            this.window = HighLogic.fetch.gameObject.AddComponent<SectionWindow>();
            this.window.Section = this;
            RenderingManager.AddToPostDrawQueue(0, this.window.Draw);

            if (isUserSection)
            {
                this.IsUser = true;

                this.Title = "Custom " + (SectionList.Instance.UserSections.Count + 1);
                this.Categories.Add(ReadoutCategory.Orbital);
                this.Categories.Add(ReadoutCategory.Surface);
                this.Categories.Add(ReadoutCategory.Vessel);
                this.Categories.Add(ReadoutCategory.Rendezvous);
                this.Categories.Add(ReadoutCategory.Misc);
                this.Visible = true;

                if (isNewSection)
                {
                    this.editDisplay.Visible = true;
                }
            }

            this.InitialiseStyles();
        }

        private void InitialiseStyles()
        {
            this.TitleStyle = new GUIStyle(HighLogic.Skin.label)
            {
                margin = new RectOffset(),
                padding = new RectOffset(3, 3, 3, 3),
                normal =
                {
                    textColor = Color.white
                },
                fontSize = 13,
                fontStyle = FontStyle.Bold,
                stretchWidth = true
            };

            this.AreaStyle = new GUIStyle(HighLogic.Skin.box)
            {
                margin = new RectOffset(),
                padding = new RectOffset(5, 5, 5, 5)
            };

            this.LabelStyle = new GUIStyle(HighLogic.Skin.label)
            {
                normal =
                {
                    textColor = Color.white
                },
                margin = new RectOffset(),
                padding = new RectOffset(3, 3, 3, 3),
                alignment = TextAnchor.MiddleCenter,
                fontSize = 12,
                fontStyle = FontStyle.Bold,
                stretchWidth = true
            };
        }

        #endregion

        #region Update and Draw

        public void Update()
        {
            foreach (var readout in this.readouts)
            {
                readout.Update();
            }
        }

        public void Draw()
        {
            GUILayout.Label(this.title.ToUpper(), this.TitleStyle);
            GUILayout.BeginVertical(this.AreaStyle);
            if (this.readouts.Count > 0)
            {
                foreach (var readout in this.readouts)
                {
                    readout.Draw();
                }
            }
            else
            {
                GUILayout.BeginHorizontal(GUILayout.Width(Readout.NameWidth + Readout.DataWidth));
                GUILayout.Label("No readouts installed!", this.LabelStyle);
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
        }

        #endregion

        #region Save and Load

        /// <summary>
        ///     Saves the settings associated with this section.
        /// </summary>
        public void Save()
        {
            if (this.title != this.fileName)
            {
                if (File.Exists(EngineerGlobals.AssemblyPath + "Settings/Sections/" + this.fileName))
                {
                    File.Delete(EngineerGlobals.AssemblyPath + "Settings/Sections/" + this.fileName);
                }
            }

            this.fileName = this.title;

            var readoutNames = this.readouts.Select(readout => readout.Name).ToList();

            try
            {
                var list = new SettingList();
                list.AddSetting("visible", this.visible);
                list.AddSetting("windowed", this.window.Visible);
                list.AddSetting("x", this.window.PosX);
                list.AddSetting("y", this.window.PosY);
                list.AddSetting("categories", this.categories);
                list.AddSetting("readouts", readoutNames);
                SettingList.SaveToFile(EngineerGlobals.AssemblyPath + "Settings/Sections/" + this.fileName, list);

                MonoBehaviour.print("[KerbalEngineer/FlightSection/" + this.title + "]: Successfully saved settings.");
            }
            catch
            {
                MonoBehaviour.print("[KerbalEngineer/FlightSection/" + this.title + "]: Failed to save settings.");
            }
        }

        /// <summary>
        ///     Loads the settings associated with this section.
        /// </summary>
        public void Load()
        {
            this.fileName = this.title;

            try
            {
                var list = SettingList.CreateFromFile(EngineerGlobals.AssemblyPath + "Settings/Sections/" + this.fileName);
                this.visible = (bool)list.GetSetting("visible", this.visible);
                this.window.Visible = (bool)list.GetSetting("windowed", this.window.Visible);
                this.window.PosX = (float)list.GetSetting("x", this.window.PosX);
                this.window.PosY = (float)list.GetSetting("y", this.window.PosY);
                this.categories = list.GetSetting("categories", this.categories) as List<ReadoutCategory>;

                this.readouts.Clear();
                var readoutNames = list.GetSetting("readouts", new List<string>()) as List<string>;
                foreach (var name in readoutNames)
                {
                    this.Readouts.Add(ReadoutList.Instance.GetReadout(name));
                }

                MonoBehaviour.print("[KerbalEngineer/FlightSection/" + this.title + "]: Successfully loaded settings.");
            }
            catch
            {
                MonoBehaviour.print("[KerbalEngineer/FlightSection/" + this.title + "]: Failed to load settings.");
            }
        }

        #endregion
    }
}