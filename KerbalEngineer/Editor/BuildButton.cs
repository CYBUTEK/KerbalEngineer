// Project:	KerbalEngineer
// Author:	CYBUTEK
// License:	Attribution-NonCommercial-ShareAlike 3.0 Unported

#region Using Directives

using System.IO;

using UnityEngine;

#endregion

namespace KerbalEngineer.Editor
{
    [KSPAddon(KSPAddon.Startup.EditorAny, false)]
    public class BuildButton : MonoBehaviour
    {
        #region Fields

        private readonly Texture2D down = new Texture2D(50, 45, TextureFormat.RGBA32, false);
        private readonly Texture2D hover = new Texture2D(50, 45, TextureFormat.RGBA32, false);
        private readonly Texture2D locked = new Texture2D(50, 45, TextureFormat.RGBA32, false);
        private readonly Texture2D normal = new Texture2D(50, 45, TextureFormat.RGBA32, false);

        private bool clicked;
        private Rect position = new Rect(Screen.width * 0.5f - 300.0f, 0, 50.0f, 45.0f);

        #region Styles

        private GUIStyle tooltipInfoStyle;
        private GUIStyle tooltipTitleStyle;

        #endregion

        #endregion

        #region Initialisation

        private void Start()
        {
            // Load the button textures directly from the PNG files. (Would of used GameDatabase but it compresses them so it looks shit!)
            this.normal.LoadImage(File.ReadAllBytes(EngineerGlobals.AssemblyPath + "PluginData/BuildButton/Normal.png"));
            this.hover.LoadImage(File.ReadAllBytes(EngineerGlobals.AssemblyPath + "PluginData/BuildButton/Hover.png"));
            this.down.LoadImage(File.ReadAllBytes(EngineerGlobals.AssemblyPath + "PluginData/BuildButton/Down.png"));
            this.locked.LoadImage(File.ReadAllBytes(EngineerGlobals.AssemblyPath + "PluginData/BuildButton/Locked.png"));

            this.InitialiseStyles();
            RenderingManager.AddToPostDrawQueue(0, this.OnDraw);
        }

        /// <summary>
        ///     Initialises all of the GUI styles that are required.
        /// </summary>
        private void InitialiseStyles()
        {
            this.tooltipTitleStyle = new GUIStyle(HighLogic.Skin.label)
            {
                normal =
                {
                    textColor = Color.white
                },
                fontSize = 13,
                fontStyle = FontStyle.Bold
            };

            this.tooltipInfoStyle = new GUIStyle(HighLogic.Skin.label)
            {
                fontSize = 11,
                fontStyle = FontStyle.Bold
            };
        }

        #endregion

        #region Update and Drawing

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Backslash))
            {
                LeftClick();
            }
        }

        private void OnDraw()
        {
            if (EditorLogic.fetch.ship.Count > 0)
            {
                if (this.clicked) // Button has been clicked whilst being hovered.
                {
                    GUI.DrawTexture(this.position, this.down);

                    if (this.position.Contains(Event.current.mousePosition)) // Mouse is hovering over the button.
                    {
                        this.DrawToolTip();

                        if (Mouse.Left.GetButtonUp()) // The mouse up event has been triggered whilst over the button.
                        {
                            this.clicked = false;
                            LeftClick();
                        }
                        else if (Mouse.Right.GetButtonUp())
                        {
                            this.clicked = false;
                            RightClick();
                        }
                    }
                }
                else // The button is not registering as being clicked.
                {
                    if (this.position.Contains(Event.current.mousePosition)) // Mouse is hovering over the button.
                    {
                        // If the left mouse button has just been pressed, see the button as being clicked.
                        if (!this.clicked && (Mouse.Left.GetButtonDown() || Mouse.Right.GetButtonDown()))
                        {
                            this.clicked = true;
                        }

                        if (this.clicked) // The button has just been clicked.
                        {
                            GUI.DrawTexture(this.position, this.down);
                        }
                        else if (!Mouse.Left.GetButton() && !Mouse.Right.GetButton()) // Mouse button is not down and is just hovering.
                        {
                            GUI.DrawTexture(this.position, this.hover);
                            this.DrawToolTip();
                        }
                        else // Mouse button is down but no click was registered over the button.
                        {
                            GUI.DrawTexture(this.position, this.normal);
                        }
                    }
                    else // The mouse is not being hovered.
                    {
                        GUI.DrawTexture(this.position, this.normal);
                    }
                }

                // Check for an unclick event whilst the mouse is not hovering.
                if (this.clicked && (Mouse.Left.GetButtonUp() || Mouse.Right.GetButtonUp()))
                {
                    this.clicked = false;
                }
            }
            else // The editor is set as being locked.
            {
                GUI.DrawTexture(this.position, this.locked);
            }
        }

        /// <summary>
        ///     Draws the tooltop next to the mouse cursor.
        /// </summary>
        private void DrawToolTip()
        {
            GUI.Label(new Rect(Event.current.mousePosition.x + 16.0f, Event.current.mousePosition.y, 500.0f, 25.0f), "Kerbal Engineer Redux", this.tooltipTitleStyle);
            GUI.Label(new Rect(Event.current.mousePosition.x + 16.0f, Event.current.mousePosition.y + 16.0f, 500.0f, 25.0f), "[Left Click / Backslash] Advanced - [Right Click] Overlay", this.tooltipInfoStyle);
        }

        #endregion

        #region Static Methods

        /// <summary>
        ///     Runs when the button is clicked with the left mouse button.
        /// </summary>
        public static void LeftClick()
        {
            if (BuildAdvanced.Instance != null)
            {
                BuildAdvanced.Instance.Visible = !BuildAdvanced.Instance.Visible;
            }
        }

        /// <summary>
        ///     Runs when the button is clicked with the right mouse button.
        /// </summary>
        private static void RightClick()
        {
            if (BuildOverlay.Instance != null)
            {
                BuildOverlay.Instance.Visible = !BuildOverlay.Instance.Visible;
            }
        }

        #endregion
    }
}