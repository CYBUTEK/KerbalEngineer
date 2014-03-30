// Project:	KerbalEngineer
// Author:	CYBUTEK
// License:	Attribution-NonCommercial-ShareAlike 3.0 Unported

#region Using Directives

using KerbalEngineer.Extensions;

using UnityEngine;

#endregion

namespace KerbalEngineer.FlightEngineer
{
    public class SectionWindow : MonoBehaviour
    {
        #region Fields

        private readonly int windowId = EngineerGlobals.GetNextWindowId();
        private Rect position = new Rect(Screen.width * 0.5f - 125.0f, 100.0f, 250.0f, 0);
        private GUIStyle windowStyle;

        #endregion

        #region Properties

        private bool requireResize;
        private Section section;
        private bool visible;

        /// <summary>
        ///     Gets and sets the X position of the window.
        /// </summary>
        public float PosX
        {
            get { return this.position.x; }
            set { this.position.x = value; }
        }

        /// <summary>
        ///     Gets and sets the Y position of the window.
        /// </summary>
        public float PosY
        {
            get { return this.position.y; }
            set { this.position.y = value; }
        }

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

        /// <summary>
        ///     Gets and sets whether the window requires a resize.
        /// </summary>
        public bool RequireResize
        {
            get { return this.requireResize; }
            set { this.requireResize = value; }
        }

        #endregion

        #region Initialisation

        private void Start()
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
        }

        #endregion

        #region Update and Drawing

        private void Update() { }

        public void Draw()
        {
            if (!this.section.Visible || !this.visible)
            {
                return;
            }

            // Handle window resizing if something has changed within the GUI.
            if (this.requireResize)
            {
                this.requireResize = false;
                this.position.width = 0;
                this.position.height = 0;
            }

            this.position = GUILayout.Window(this.windowId, this.position, this.Window, string.Empty, this.windowStyle).ClampToScreen();
        }

        private void Window(int windowId)
        {
            this.section.Draw();
            GUI.DragWindow();
        }

        #endregion
    }
}