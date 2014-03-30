// Project:	KerbalEngineer
// Author:	CYBUTEK
// License:	Attribution-NonCommercial-ShareAlike 3.0 Unported

#region Using Directives

using KerbalEngineer.Extensions;

using UnityEngine;

#endregion

namespace KerbalEngineer.FlightEngineer
{
    public class InfoDisplay : MonoBehaviour
    {
        #region Instance

        private static InfoDisplay _instance;

        /// <summary>
        ///     Gets the current instance of the InfoDisplay object.
        /// </summary>
        public static InfoDisplay Instance
        {
            get { return _instance ?? (_instance = HighLogic.fetch.gameObject.AddComponent<InfoDisplay>()); }
        }

        #endregion

        #region Fields

        private readonly int windowId = EngineerGlobals.GetNextWindowId();

        private Vector2 scrollPosition = Vector2.zero;
        private Rect windowPosition = new Rect(Screen.width * 0.5f - 150.0f, Screen.height * 0.5f - 100.0f, 300.0f, 200.0f);

        #region Styles

        private GUIStyle buttonStyle;
        private GUIStyle infoStyle;
        private GUIStyle labelStyle;
        private GUIStyle windowStyle;

        #endregion

        #endregion

        #region Properties

        private Readout readout;
        private bool visible;

        /// <summary>
        ///     Gets and sets whether the display is visible.
        /// </summary>
        public bool Visible
        {
            get { return this.visible; }
            set { this.visible = value; }
        }

        /// <summary>
        ///     Gets and sets the information to be displayed.
        /// </summary>
        public Readout Readout
        {
            get { return this.readout; }
            set { this.readout = value; }
        }

        #endregion

        #region Initialisation

        // Runs when the object is created.
        private void Start()
        {
            this.InitialiseStyles();
            RenderingManager.AddToPostDrawQueue(0, this.Draw);
        }

        // Initialises the gui styles upon request.
        private void InitialiseStyles()
        {
            this.windowStyle = new GUIStyle(HighLogic.Skin.window);

            this.infoStyle = new GUIStyle
            {
                margin = new RectOffset(5, 5, 5, 5)
            };

            this.buttonStyle = new GUIStyle(HighLogic.Skin.button)
            {
                normal =
                {
                    textColor = Color.white
                },
                fontSize = 11,
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
                stretchWidth = true
            };
        }

        #endregion

        #region Drawing

        /// <summary>
        ///     Runs when the object is called to draw.
        /// </summary>
        private void Draw()
        {
            if (this.visible)
            {
                this.windowPosition = GUILayout.Window(this.windowId, this.windowPosition, this.Window, "READOUT INFORMATION", this.windowStyle).ClampToScreen();
            }
        }

        private void Window(int windowId)
        {
            GUI.skin = HighLogic.Skin;
            this.scrollPosition = GUILayout.BeginScrollView(this.scrollPosition, false, true);
            GUI.skin = null;

            if (this.readout != null)
            {
                GUILayout.Label(this.readout.Name, this.labelStyle);

                GUILayout.BeginHorizontal(this.infoStyle);
                GUILayout.Label(this.readout.Description, this.labelStyle);
                GUILayout.EndHorizontal();
            }

            GUILayout.EndScrollView();

            if (GUILayout.Button("CLOSE", this.buttonStyle))
            {
                this.visible = false;
            }

            GUI.DragWindow();
        }

        #endregion
    }
}