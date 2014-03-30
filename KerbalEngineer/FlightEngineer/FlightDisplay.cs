// Project:	KerbalEngineer
// Author:	CYBUTEK
// License:	Attribution-NonCommercial-ShareAlike 3.0 Unported

#region Using Directives

using KerbalEngineer.Extensions;
using KerbalEngineer.Settings;

using UnityEngine;

#endregion

namespace KerbalEngineer.FlightEngineer
{
    public class FlightDisplay
    {
        #region Instance

        private static FlightDisplay _instance;

        /// <summary>
        ///     Gets the current instance of the flight display.
        /// </summary>
        public static FlightDisplay Instance
        {
            get { return _instance ?? (_instance = new FlightDisplay()); }
        }

        #endregion

        #region Fields

        private readonly int windowId = EngineerGlobals.GetNextWindowId();

        #region Styles

        private GUIStyle buttonStyle;
        private GUIStyle titleStyle;
        private GUIStyle windowStyle;

        #endregion

        #endregion

        #region Properties

        private bool controlBar = true;
        private bool requireResize;
        private Rect windowPosition = new Rect(Screen.width * 0.5f - 125.0f, 100.0f, 250.0f, 0);

        /// <summary>
        ///     Gets and sets the window position.
        /// </summary>
        public Rect WindowPosition
        {
            get { return this.windowPosition; }
            set { this.windowPosition = value; }
        }

        /// <summary>
        ///     Gets and sets whether the window requires a resize.
        /// </summary>
        public bool RequireResize
        {
            get { return this.requireResize; }
            set { this.requireResize = value; }
        }

        /// <summary>
        ///     Gets and sets the visibility of the control bar.
        /// </summary>
        public bool ControlBar
        {
            get { return this.controlBar; }
            set
            {
                if (this.controlBar && !value)
                {
                    this.requireResize = true;
                }

                this.controlBar = value;
            }
        }

        #endregion

        #region Initialisation

        public FlightDisplay()
        {
            this.InitialiseStyles();
        }

        private void InitialiseStyles()
        {
            this.windowStyle = new GUIStyle(HighLogic.Skin.window)
            {
                margin = new RectOffset(),
                padding = new RectOffset(5, 5, 3, 5),
                fixedWidth = 270.0f
            };

            this.buttonStyle = new GUIStyle(HighLogic.Skin.button)
            {
                normal =
                {
                    textColor = Color.white
                },
                fontSize = 11,
                fontStyle = FontStyle.Bold
            };

            this.titleStyle = new GUIStyle(HighLogic.Skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 13,
                fontStyle = FontStyle.Bold,
                stretchWidth = true
            };
        }

        #endregion

        #region Update and Drawing

        public void Update() { }

        public void Draw()
        {
            if ((SectionList.Instance.HasVisibleSections && SectionList.Instance.HasAttachedSections) || this.controlBar)
            {
                // Handle window resizing if something has changed within the GUI.
                if (this.requireResize)
                {
                    this.requireResize = false;
                    this.windowPosition.width = 0;
                    this.windowPosition.height = 0;
                }

                this.windowPosition = GUILayout.Window(this.windowId, this.windowPosition, this.Window, string.Empty, this.windowStyle).ClampToScreen();
            }
        }

        private void Window(int windowId)
        {
            // Draw control bar.
            if (this.controlBar)
            {
                GUILayout.Label("FLIGHT ENGINEER " + EngineerGlobals.AssemblyVersion, this.titleStyle);
                GUILayout.BeginHorizontal();
                foreach (var section in SectionList.Instance.FixedSections)
                {
                    section.Visible = GUILayout.Toggle(section.Visible, section.ShortTitle, this.buttonStyle, GUILayout.Height(25.0f));
                }
                GUILayout.EndHorizontal();
            }

            // Draw all visible fixed sections.
            foreach (var section in SectionList.Instance.FixedSections)
            {
                if (section.Visible && !section.Window.Visible)
                {
                    section.Draw();
                }
            }

            // Draw all visible user sections.
            foreach (var section in SectionList.Instance.UserSections)
            {
                if (section.Visible && !section.Window.Visible)
                {
                    section.Draw();
                }
            }

            GUI.DragWindow();
        }

        #endregion

        #region Save and Load

        /// <summary>
        ///     Saves the settings associated with the flight display.
        /// </summary>
        public void Save()
        {
            try
            {
                var list = new SettingList();
                list.AddSetting("x", this.windowPosition.x);
                list.AddSetting("y", this.windowPosition.y);
                list.AddSetting("controlBar", this.controlBar);
                SettingList.SaveToFile(EngineerGlobals.AssemblyPath + "Settings/FlightDisplay", list);

                MonoBehaviour.print("[KerbalEngineer/FlightDisplay]: Successfully saved settings.");
            }
            catch
            {
                MonoBehaviour.print("[KerbalEngineer/FlightDisplay]: Failed to save settings.");
            }
        }

        /// <summary>
        ///     Loads the settings associated with the flight display.
        /// </summary>
        public void Load()
        {
            try
            {
                var list = SettingList.CreateFromFile(EngineerGlobals.AssemblyPath + "Settings/FlightDisplay");
                this.windowPosition.x = (float)list.GetSetting("x", this.windowPosition.x);
                this.windowPosition.y = (float)list.GetSetting("y", this.windowPosition.y);
                this.controlBar = (bool)list.GetSetting("controlBar", this.controlBar);

                MonoBehaviour.print("[KerbalEngineer/FlightDisplay]: Successfully loaded settings.");
            }
            catch
            {
                MonoBehaviour.print("[KerbalEngineer/FlightDisplay]: Failed to load settings.");
            }
        }

        #endregion
    }
}