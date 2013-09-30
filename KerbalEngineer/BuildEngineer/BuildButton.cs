// Name:    Kerbal Engineer Redux
// Author:  CYBUTEK
// License: Attribution-NonCommercial-ShareAlike 3.0 Unported

using System.IO;
using UnityEngine;

namespace KerbalEngineer.BuildEngineer
{
    [KSPAddon(KSPAddon.Startup.EditorAny, false)]
    public class BuildButton : MonoBehaviour
    {
        #region Fields

        private Rect _position = new Rect(265f, 0f, 50f, 45f);
        private GUIStyle _tooltipTitleStyle, _tooltipInfoStyle;
        private Texture2D _normal = new Texture2D(50, 45, TextureFormat.RGBA32, false);
        private Texture2D _hover = new Texture2D(50, 45, TextureFormat.RGBA32, false);
        private Texture2D _down = new Texture2D(50, 45, TextureFormat.RGBA32, false);
        private Texture2D _locked = new Texture2D(50, 45, TextureFormat.RGBA32, false);

        private bool _clicked = false;
        private bool _hasInitStyles = false;

        #endregion

        #region Initialisation

        public void Start()
        {
            // Load the button textures directly from the PNG files. (Would of used GameDatabase but it compresses them so it looks shit!)
            _normal.LoadImage(File.ReadAllBytes(EngineerGlobals.AssemblyPath + "GUI/BuildButton/Normal.png"));
            _hover.LoadImage(File.ReadAllBytes(EngineerGlobals.AssemblyPath + "GUI/BuildButton/Hover.png"));
            _down.LoadImage(File.ReadAllBytes(EngineerGlobals.AssemblyPath + "GUI/BuildButton/Down.png"));
            _locked.LoadImage(File.ReadAllBytes(EngineerGlobals.AssemblyPath + "GUI/BuildButton/Locked.png"));

            RenderingManager.AddToPostDrawQueue(0, OnDraw);
        }

        // Initialises all of the GUI styles that are required.
        private void InitialiseStyles()
        {
            _tooltipTitleStyle = new GUIStyle(GUI.skin.label);
            _tooltipTitleStyle.fontSize = 13;
            _tooltipTitleStyle.fontStyle = FontStyle.Bold;

            _tooltipInfoStyle = new GUIStyle(GUI.skin.label);
            _tooltipInfoStyle.fontSize = 11;
            _tooltipInfoStyle.fontStyle = FontStyle.Bold;
        }

        #endregion

        #region Update and Drawing

        private void OnDraw()
        {
            if (!_hasInitStyles) InitialiseStyles();

            if (EditorLogic.fetch.editorScreen != EditorLogic.EditorScreen.Parts) return;

            if (!EditorLogic.editorLocked)
            {
                if (_clicked) // Button has been clicked whilst being hovered.
                {
                    GUI.DrawTexture(_position, _down);

                    if (_position.Contains(Event.current.mousePosition)) // Mouse is hovering over the button.
                    {
                        DrawToolTip();

                        if (Mouse.Left.GetButtonUp()) // The mouse up event has been triggered whilst over the button.
                        {
                            _clicked = false;
                            ButtonClickedLeft();
                        }
                        else if (Mouse.Right.GetButtonUp())
                        {
                            _clicked = false;
                            ButtonClickedRight();
                        }
                    }
                }
                else // The button is not registering as being clicked.
                {
                    if (_position.Contains(Event.current.mousePosition)) // Mouse is hovering over the button.
                    {
                        // If the left mouse button has just been pressed, see the button as being clicked.
                        if (!_clicked && (Mouse.Left.GetButtonDown() || Mouse.Right.GetButtonDown())) _clicked = true;

                        if (_clicked) // The button has just been clicked.
                        {
                            GUI.DrawTexture(_position, _down);
                        }
                        else if (!Mouse.Left.GetButton() && !Mouse.Right.GetButton()) // Mouse button is not down and is just hovering.
                        {
                            GUI.DrawTexture(_position, _hover);
                            DrawToolTip();
                        }
                        else // Mouse button is down but no click was registered over the button.
                        {
                            GUI.DrawTexture(_position, _normal);
                        }
                    }
                    else // The mouse is not being hovered.
                    {
                        GUI.DrawTexture(_position, _normal);
                    }
                }

                // Check for an unclick event whilst the mouse is not hovering.
                if (_clicked && (Mouse.Left.GetButtonUp() || Mouse.Right.GetButtonUp())) _clicked = false;
            }
            else // The editor is set as being locked.
            {
                GUI.DrawTexture(_position, _locked);
            }
        }

        // Draws the tooltip next to the mouse cursor.
        private void DrawToolTip()
        {
            GUI.Label(new Rect(Event.current.mousePosition.x + 16f, Event.current.mousePosition.y, 256f, 25f), "Kerbal Engineer Redux", _tooltipTitleStyle);
            GUI.Label(new Rect(Event.current.mousePosition.x + 16f, Event.current.mousePosition.y + 16f, 256f, 25f), "[Left Click] Advanced - [Right Click] Overlay", _tooltipInfoStyle);
            //GUI.Label(new Rect(Event.current.mousePosition.x + 16f, Event.current.mousePosition.y + 30f, 256f, 25f), "", _tooltipInfoStyle);
        }

        // Runs the stuff to do when the button is clicked with the left mouse button.
        private void ButtonClickedLeft()
        {
            if (BuildAdvanced.Instance != null)
                BuildAdvanced.Instance.Visible = !BuildAdvanced.Instance.Visible;
        }

        // Runs the stuff to do when the button is clicked with the right mouse button.
        private void ButtonClickedRight()
        {
            if (BuildOverlay.Instance != null)
                BuildOverlay.Instance.Visible = !BuildOverlay.Instance.Visible;
        }

        #endregion
    }
}
