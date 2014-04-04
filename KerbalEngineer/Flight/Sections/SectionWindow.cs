// Project:	KerbalEngineer
// Author:	CYBUTEK
// License:	Attribution-NonCommercial-ShareAlike 3.0 Unported

#region Using Directives

using KerbalEngineer.Extensions;

using UnityEngine;

#endregion

namespace KerbalEngineer.Flight.Sections
{
    public class SectionWindow : MonoBehaviour
    {
        #region Fields

        private bool resizeRequested;
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
        ///     Gets and sets the parent section for the floating section window.
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

        private GUIStyle windowStyle;

        /// <summary>
        ///     Initialises all the styles required for this object.
        /// </summary>
        private void InitialiseStyles()
        {
            this.windowStyle = new GUIStyle(HighLogic.Skin.window)
            {
                margin = new RectOffset(),
                padding = new RectOffset(5, 5, 0, 5)
            };
        }

        #endregion

        #region Drawing

        /// <summary>
        ///     Called to draw the floating section window when the UI is enabled.
        /// </summary>
        private void Draw()
        {
            if (this.ParentSection != null && this.ParentSection.IsVisible)
            {
                if (this.resizeRequested)
                {
                    this.windowPosition.height = 0;
                    this.resizeRequested = false;
                }
                this.windowPosition = GUILayout.Window(this.windowId, this.windowPosition, this.Window, string.Empty, this.windowStyle).ClampToScreen();
                this.ParentSection.FloatingPositionX = this.windowPosition.x;
                this.ParentSection.FloatingPositionY = this.windowPosition.y;
            }
        }

        /// <summary>
        ///     Draws the floating section window.
        /// </summary>
        private void Window(int windowId)
        {
            this.ParentSection.Draw();

            GUI.DragWindow();
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

        #region Methods

        /// <summary>
        ///     Request that the floating section window's size is reset in the next draw call.
        /// </summary>
        public void RequestResize()
        {
            this.resizeRequested = true;
        }

        #endregion
    }
}