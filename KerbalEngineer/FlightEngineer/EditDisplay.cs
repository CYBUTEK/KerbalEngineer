// Project:	KerbalEngineer
// Author:	CYBUTEK
// License:	Attribution-NonCommercial-ShareAlike 3.0 Unported

#region Using Directives

using System.IO;

using KerbalEngineer.Extensions;

using UnityEngine;

#endregion

namespace KerbalEngineer.FlightEngineer
{
    public class EditDisplay : MonoBehaviour
    {
        #region Fields

        private readonly int windowId = EngineerGlobals.GetNextWindowId();

        private Vector2 scrollAvailablePosition = Vector2.zero;
        private Vector2 scrollInstalledPosition = Vector2.zero;
        private ReadoutCategory selectedCategory = ReadoutCategory.None;
        private Rect windowPosition = new Rect(Screen.width * 0.5f - 250.0f, Screen.height * 0.5f - 250.0f, 500.0f, 500.0f);

        #region Styles

        private GUIStyle buttonStyle;
        private GUIStyle customControlBarStyle;
        private GUIStyle labelStyle;
        private GUIStyle rowStyle;
        private GUIStyle textStyle;
        private GUIStyle titleStyle;
        private GUIStyle windowStyle;

        #endregion

        #endregion

        #region Properties

        private Section section;
        private bool visible;

        /// <summary>
        ///     Gets and sets the visibility of the window.
        /// </summary>
        public bool Visible
        {
            get { return this.visible; }
            set { this.visible = value; }
        }

        /// <summary>
        ///     Gets and sets the parent section.
        /// </summary>
        public Section Section
        {
            get { return this.section; }
            set { this.section = value; }
        }

        #endregion

        #region Initialisation

        private void Start()
        {
            this.InitialiseStyles();
        }

        /// <summary>
        ///     Initialises the GUI styles upon request.
        /// </summary>
        private void InitialiseStyles()
        {
            this.windowStyle = new GUIStyle(HighLogic.Skin.window);

            this.customControlBarStyle = new GUIStyle
            {
                fixedHeight = 25.0f
            };

            this.rowStyle = new GUIStyle
            {
                margin = new RectOffset(5, 5, 5, 5),
                fixedHeight = 25.0f
            };

            this.buttonStyle = new GUIStyle(HighLogic.Skin.button)
            {
                normal =
                {
                    textColor = Color.white
                },
                fontSize = 11,
                fontStyle = FontStyle.Bold,
                stretchHeight = true
            };

            this.textStyle = new GUIStyle(HighLogic.Skin.textField)
            {
                alignment = TextAnchor.MiddleLeft,
                fontSize = 12,
                stretchWidth = true,
                stretchHeight = true
            };

            this.titleStyle = new GUIStyle(HighLogic.Skin.label)
            {
                normal =
                {
                    textColor = Color.white
                },
                alignment = TextAnchor.MiddleLeft,
                fontSize = 12,
                fontStyle = FontStyle.Bold,
                stretchWidth = true
            };

            this.labelStyle = new GUIStyle(HighLogic.Skin.label)
            {
                normal =
                {
                    textColor = Color.white
                },
                alignment = TextAnchor.MiddleLeft,
                fontSize = 12,
                fontStyle = FontStyle.Bold,
                stretchHeight = true,
                stretchWidth = true
            };
        }

        #endregion

        #region Drawing

        /// <summary>
        ///     Runs when the object is called to draw.
        /// </summary>
        public void Draw()
        {
            if (this.visible)
            {
                this.windowPosition = GUILayout.Window(this.windowId, this.windowPosition, this.Window, "EDIT SECTION - " + this.section.Title.ToUpper(), this.windowStyle).ClampToScreen();
            }
        }

        private void Window(int windowId)
        {
            // Selected category has not been selected.
            if (this.selectedCategory == ReadoutCategory.None)
            {
                // Set selected category to first category.
                if (this.section.Categories.Count > 0)
                {
                    this.selectedCategory = this.section.Categories[0];
                }
            }

            // Show user controls if the section was user created.
            if (this.section.IsUser)
            {
                this.UserControls();
            }

            // Show categories selection if there is more than one.
            if (this.section.Categories.Count > 1)
            {
                this.Categories();
            }

            this.Available(this.selectedCategory);
            this.Installed();

            // Detach and close buttons.
            GUILayout.BeginHorizontal(GUILayout.Height(30.0f));
            if (GUILayout.Toggle(this.section.Window.Visible, "DETACH INTO WINDOW", this.buttonStyle, GUILayout.Width(150.0f)) != this.section.Window.Visible)
            {
                this.section.Window.Visible = !this.section.Window.Visible;
                FlightDisplay.Instance.RequireResize = true;
            }
            if (GUILayout.Button("CLOSE EDITOR", this.buttonStyle))
            {
                this.visible = false;
            }
            GUILayout.EndHorizontal();

            GUI.DragWindow();
        }

        /// <summary>
        ///     Draws the user section controls.
        /// </summary>
        private void UserControls()
        {
            GUILayout.BeginHorizontal(this.customControlBarStyle);

            GUILayout.BeginVertical(GUILayout.Width(50.0f));
            GUILayout.Label("TITLE - ", this.labelStyle);
            GUILayout.EndVertical();

            // Title text box.
            GUILayout.BeginVertical();
            this.section.Title = GUILayout.TextField(this.section.Title, this.textStyle);
            GUILayout.EndVertical();

            // Delete button and handling.
            GUILayout.BeginVertical(GUILayout.Width(100.0f));
            if (GUILayout.Button("DELETE", this.buttonStyle))
            {
                // Remove objects from lists and render queues.
                SectionList.Instance.UserSections.Remove(this.section);
                FlightController.Instance.RequireResize = true;
                if (this.section.Visible)
                {
                    FlightDisplay.Instance.RequireResize = true;
                }
                RenderingManager.RemoveFromPostDrawQueue(0, this.Draw);
                RenderingManager.RemoveFromPostDrawQueue(0, this.section.Window.Draw);

                // Delete the settings file.
                if (File.Exists(EngineerGlobals.AssemblyPath + "Settings/Sections/" + this.section.FileName))
                {
                    File.Delete(EngineerGlobals.AssemblyPath + "Settings/Sections/" + this.section.FileName);
                }

                // Set MonoBehaviour objects to be destroyed.
                Destroy(this.section.Window);
                Destroy(this);

                print("[KerbalEngineer]: Deleted " + this.section.Title + " section.");
            }
            GUILayout.EndVertical();

            GUILayout.EndHorizontal();
        }

        /// <summary>
        ///     Draws the available categories selection.
        /// </summary>
        private void Categories()
        {
            GUILayout.BeginHorizontal(GUILayout.Height(30.0f));
            foreach (var category in this.section.Categories)
            {
                var isSelected = this.selectedCategory == category;
                if (GUILayout.Toggle(isSelected, category.ToString().ToUpper(), this.buttonStyle) && !isSelected)
                {
                    this.selectedCategory = category;
                }
            }
            GUILayout.EndHorizontal();
        }

        /// <summary>
        ///     Draws the available readouts panel.
        /// </summary>
        private void Available(ReadoutCategory category)
        {
            GUI.skin = HighLogic.Skin;
            this.scrollAvailablePosition = GUILayout.BeginScrollView(this.scrollAvailablePosition, false, true, GUILayout.Height(150f));
            GUI.skin = null;

            // Panel title.
            GUILayout.Label("AVAILABLE", this.titleStyle);

            GUILayout.BeginVertical();
            var count = 0;
            foreach (var readout in ReadoutList.Instance.GetCategory(category))
            {
                // Readout is already installed.
                if (this.section.Readouts.Contains(readout))
                {
                    continue;
                }

                count++;

                GUILayout.BeginHorizontal(this.rowStyle);

                // Readout name.
                GUILayout.BeginVertical();
                GUILayout.Label(readout.Name, this.labelStyle);
                GUILayout.EndVertical();

                // Info button.
                GUILayout.BeginVertical(GUILayout.Width(30.0f));
                if (GUILayout.Button("?", this.buttonStyle))
                {
                    InfoDisplay.Instance.Readout = readout;
                    InfoDisplay.Instance.Visible = true;
                }
                GUILayout.EndVertical();

                // Install button
                GUILayout.BeginVertical(GUILayout.Width(100.0f));
                if (GUILayout.Button("INSTALL", this.buttonStyle))
                {
                    this.section.Readouts.Add(readout);
                }
                GUILayout.EndVertical();

                GUILayout.EndHorizontal();
            }

            // Panel is void of readouts.
            if (count == 0)
            {
                GUILayout.BeginHorizontal(this.rowStyle);
                GUILayout.Label("All readouts are installed!", this.labelStyle);
                GUILayout.EndHorizontal();
            }

            GUILayout.EndVertical();
            GUILayout.EndScrollView();

            // Insert space between available and installed panels.
            GUILayout.Space(5f);
        }

        /// <summary>
        ///     Draws the installed readouts panel.
        /// </summary>
        private void Installed()
        {
            GUI.skin = HighLogic.Skin;
            this.scrollInstalledPosition = GUILayout.BeginScrollView(this.scrollInstalledPosition, false, true);
            GUI.skin = null;

            // Panel title
            GUILayout.Label("INSTALLED", this.titleStyle);

            GUILayout.BeginVertical();
            foreach (var readout in this.section.Readouts)
            {
                GUILayout.BeginHorizontal(this.rowStyle);

                // Readout name.
                GUILayout.BeginVertical();
                GUILayout.Label(readout.Name, this.labelStyle);
                GUILayout.EndVertical();

                // Move position up button.
                GUILayout.BeginVertical(GUILayout.Width(30.0f));
                if (GUILayout.Button("▲", this.buttonStyle))
                {
                    var index = this.section.Readouts.IndexOf(readout);
                    if (index > 0)
                    {
                        this.section.Readouts[index] = this.section.Readouts[index - 1];
                        this.section.Readouts[index - 1] = readout;
                    }
                }
                GUILayout.EndVertical();

                // Move position down button.
                GUILayout.BeginVertical(GUILayout.Width(30.0f));
                if (GUILayout.Button("▼", this.buttonStyle))
                {
                    var index = this.section.Readouts.IndexOf(readout);
                    if (index < this.section.Readouts.Count - 1)
                    {
                        this.section.Readouts[index] = this.section.Readouts[index + 1];
                        this.section.Readouts[index + 1] = readout;
                    }
                }
                GUILayout.EndVertical();

                // Info button.
                GUILayout.BeginVertical(GUILayout.Width(30.0f));
                if (GUILayout.Button("?", this.buttonStyle))
                {
                    InfoDisplay.Instance.Readout = readout;
                    InfoDisplay.Instance.Visible = true;
                }
                GUILayout.EndVertical();

                // Remove button.
                GUILayout.BeginVertical(GUILayout.Width(100.0f));
                if (GUILayout.Button("REMOVE", this.buttonStyle))
                {
                    this.section.Readouts.Remove(readout);
                    FlightDisplay.Instance.RequireResize = true;
                }
                GUILayout.EndVertical();

                GUILayout.EndHorizontal();
            }

            // Panel is void of readouts.
            if (this.section.Readouts.Count == 0)
            {
                GUILayout.BeginHorizontal(this.rowStyle);
                GUILayout.Label("No readouts are installed!", this.labelStyle);
                GUILayout.EndHorizontal();
            }

            GUILayout.EndVertical();
            GUILayout.EndScrollView();
        }

        #endregion
    }
}