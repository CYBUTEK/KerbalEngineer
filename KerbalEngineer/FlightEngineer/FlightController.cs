// Project:	KerbalEngineer
// Author:	CYBUTEK
// License:	Attribution-NonCommercial-ShareAlike 3.0 Unported

#region Using Directives

using System.IO;

using KerbalEngineer.Settings;

using UnityEngine;

#endregion

namespace KerbalEngineer.FlightEngineer
{
    public class FlightController
    {
        #region Instance

        private static FlightController _instance;

        /// <summary>
        ///     Gets the current instance of the flight controller.
        /// </summary>
        public static FlightController Instance
        {
            get { return _instance ?? (_instance = new FlightController()); }
        }

        #endregion

        #region Fields

        private readonly Texture2D closedDown = new Texture2D(100, 17, TextureFormat.RGBA32, false);
        private readonly Texture2D closedHover = new Texture2D(100, 17, TextureFormat.RGBA32, false);
        private readonly Texture2D closedNormal = new Texture2D(100, 17, TextureFormat.RGBA32, false);
        private readonly Texture2D openDown = new Texture2D(100, 17, TextureFormat.RGBA32, false);
        private readonly Texture2D openHover = new Texture2D(100, 17, TextureFormat.RGBA32, false);
        private readonly Texture2D openNormal = new Texture2D(100, 17, TextureFormat.RGBA32, false);
        private readonly int windowId = EngineerGlobals.GetNextWindowId();

        private bool clicked;
        private Rect handlePosition = new Rect(Screen.width * 0.5f + 200.0f, 0, 100.0f, 17.0f);
        private bool open;
        private float openAmount;
        private Rect windowPosition = new Rect(Screen.width * 0.5f + 150.0f, 0, 200.0f, 0);

        #region Styles

        private GUIStyle buttonStyle;
        private GUIStyle windowStyle;

        #endregion

        #endregion

        #region Properties

        private bool requireResize;

        /// <summary>
        ///     Gets and sets whether the display requires a resize.
        /// </summary>
        public bool RequireResize
        {
            get { return this.requireResize; }
            set { this.requireResize = value; }
        }

        #endregion

        #region Initialisation

        private FlightController()
        {
            // Load textures directly from the PNG files. (Would of used GameDatabase but it compresses them so it looks shit!)
            this.closedNormal.LoadImage(File.ReadAllBytes(EngineerGlobals.AssemblyPath + "GUI/FlightButton/ClosedNormal.png"));
            this.closedHover.LoadImage(File.ReadAllBytes(EngineerGlobals.AssemblyPath + "GUI/FlightButton/ClosedHover.png"));
            this.closedDown.LoadImage(File.ReadAllBytes(EngineerGlobals.AssemblyPath + "GUI/FlightButton/ClosedDown.png"));
            this.openNormal.LoadImage(File.ReadAllBytes(EngineerGlobals.AssemblyPath + "GUI/FlightButton/OpenNormal.png"));
            this.openHover.LoadImage(File.ReadAllBytes(EngineerGlobals.AssemblyPath + "GUI/FlightButton/OpenHover.png"));
            this.openDown.LoadImage(File.ReadAllBytes(EngineerGlobals.AssemblyPath + "GUI/FlightButton/OpenDown.png"));

            this.InitialiseStyles();
        }

        private void InitialiseStyles()
        {
            this.windowStyle = new GUIStyle(HighLogic.Skin.window)
            {
                margin = new RectOffset(),
                padding = new RectOffset(3, 3, 3, 3),
                fixedWidth = this.windowPosition.width
            };

            this.buttonStyle = new GUIStyle(HighLogic.Skin.button)
            {
                normal =
                {
                    textColor = Color.white
                },
                margin = new RectOffset(),
                fixedHeight = 20.0f,
                fontSize = 11,
                fontStyle = FontStyle.Bold
            };
        }

        #endregion

        #region Update and Drawing

        public void Update()
        {
            // Controls the sliding animation.
            if (this.open && this.openAmount < 1f) // Opening
            {
                this.openAmount += ((10.0f * (1.0f - this.openAmount)) + 0.5f) * Time.deltaTime;

                if (this.openAmount > 1.0f)
                {
                    this.openAmount = 1.0f;
                }
            }
            else if (!this.open && this.openAmount > 0f) // Closing
            {
                this.openAmount -= ((10.0f * this.openAmount) + 0.5f) * Time.deltaTime;

                if (this.openAmount < 0)
                {
                    this.openAmount = 0;
                }
            }

            // Set the sliding positions.
            this.windowPosition.y = -this.windowPosition.height * (1.0f - this.openAmount);
            this.handlePosition.y = this.windowPosition.y + this.windowPosition.height;
        }

        public void Draw()
        {
            // Handle window resizing if something has changed within the GUI.
            if (this.requireResize)
            {
                this.requireResize = false;
                this.windowPosition.height = 0;
            }

            this.DrawButton();

            if (this.windowPosition.y + this.windowPosition.height > 0 || this.windowPosition.height == 0)
            {
                this.windowPosition = GUILayout.Window(this.windowId, this.windowPosition, this.Window, string.Empty, this.windowStyle);
            }
        }

        private void Window(int windowId)
        {
            // Control bar toggle.
            FlightDisplay.Instance.ControlBar = GUILayout.Toggle(FlightDisplay.Instance.ControlBar, "CONTROL BAR", this.buttonStyle);

            GUILayout.BeginHorizontal(); // Begin fixed sections.

            // Draw fixed section display toggles.
            GUILayout.BeginVertical();
            foreach (var section in SectionList.Instance.FixedSections)
            {
                section.Visible = GUILayout.Toggle(section.Visible, section.Title.ToUpper(), this.buttonStyle);
            }
            GUILayout.EndVertical();

            // Draw fixed section edit toggles.
            GUILayout.BeginVertical(GUILayout.Width(50.0f));
            foreach (var section in SectionList.Instance.FixedSections)
            {
                section.EditDisplay.Visible = GUILayout.Toggle(section.EditDisplay.Visible, "EDIT", this.buttonStyle);
            }
            GUILayout.EndVertical();

            GUILayout.EndHorizontal(); // End fixed sections.
            GUILayout.BeginHorizontal(); // Begin user sections.

            // Draw user section display toggles.
            GUILayout.BeginVertical();
            foreach (var section in SectionList.Instance.UserSections)
            {
                section.Visible = GUILayout.Toggle(section.Visible, section.Title.ToUpper(), this.buttonStyle);
            }
            GUILayout.EndVertical();

            // Draw user section edit toggles.
            GUILayout.BeginVertical(GUILayout.Width(50f));
            foreach (var section in SectionList.Instance.UserSections)
            {
                section.EditDisplay.Visible = GUILayout.Toggle(section.EditDisplay.Visible, "EDIT", this.buttonStyle);
            }
            GUILayout.EndVertical();

            GUILayout.EndHorizontal(); // End user sections.

            // New custom user section button.
            if (GUILayout.Button("NEW USER SECTION", this.buttonStyle))
            {
                SectionList.Instance.UserSections.Add(new Section(true));
            }
        }

        private void DrawButton()
        {
            if (this.clicked) // Button has been clicked whilst being hovered.
            {
                GUI.DrawTexture(this.handlePosition, this.open ? this.openDown : this.closedDown);

                if (this.handlePosition.Contains(Event.current.mousePosition)) // Mouse is hovering over the button.
                {
                    if (Mouse.Left.GetButtonUp()) // The mouse up event has been triggered whilst over the button.
                    {
                        this.clicked = false;
                        this.open = !this.open;
                    }
                }
            }
            else // The button is not registering as being clicked.
            {
                if (this.handlePosition.Contains(Event.current.mousePosition)) // Mouse is hovering over the button.
                {
                    // If the left mouse button has just been pressed, see the button as being clicked.
                    if (!this.clicked && (Mouse.Left.GetButtonDown()))
                    {
                        this.clicked = true;
                    }

                    if (this.clicked) // The button has just been clicked.
                    {
                        GUI.DrawTexture(this.handlePosition, this.open ? this.openDown : this.closedDown);
                    }
                    else if (!Mouse.Left.GetButton()) // Mouse button is not down and is just hovering.
                    {
                        GUI.DrawTexture(this.handlePosition, this.open ? this.openHover : this.closedHover);
                    }
                    else // Mouse button is down but no click was registered over the button.
                    {
                        GUI.DrawTexture(this.handlePosition, this.open ? this.openNormal : this.closedNormal);
                    }
                }
                else // The mouse is not being hovered.
                {
                    GUI.DrawTexture(this.handlePosition, this.open ? this.openNormal : this.closedNormal);
                }
            }

            // Check for an unclick event whilst the mouse is not hovering.
            if (this.clicked && (Mouse.Left.GetButtonUp()))
            {
                this.clicked = false;
            }
        }

        #endregion

        #region Save and Load

        /// <summary>
        ///     Saves the settings associated with the flight controller.
        /// </summary>
        public void Save()
        {
            try
            {
                var list = new SettingList();
                list.AddSetting("open", this.open);
                SettingList.SaveToFile(EngineerGlobals.AssemblyPath + "Settings/FlightController", list);

                MonoBehaviour.print("[KerbalEngineer/FlightController]: Successfully saved settings.");
            }
            catch
            {
                MonoBehaviour.print("[KerbalEngineer/FlightController]: Failed to save settings.");
            }
        }

        /// <summary>
        ///     Loads the settings associated with the flight controller.
        /// </summary>
        public void Load()
        {
            try
            {
                var list = SettingList.CreateFromFile(EngineerGlobals.AssemblyPath + "Settings/FlightController");
                this.open = (bool)list.GetSetting("open", this.open);

                MonoBehaviour.print("[KerbalEngineer/FlightController]: Successfully loaded settings.");
            }
            catch
            {
                MonoBehaviour.print("[KerbalEngineer/FlightController]: Failed to load settings.");
            }
        }

        #endregion
    }
}