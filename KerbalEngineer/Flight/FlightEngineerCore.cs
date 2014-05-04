// Project:	KerbalEngineer
// Author:	CYBUTEK
// License:	Attribution-NonCommercial-ShareAlike 3.0 Unported

#region Using Directives

using System.Collections.Generic;

using KerbalEngineer.Flight.Readouts;
using KerbalEngineer.Flight.Sections;

using UnityEngine;

#endregion

namespace KerbalEngineer.Flight
{
    /// <summary>
    ///     Core management system for the Flight Engineer.
    /// </summary>
    public sealed class FlightEngineerCore : MonoBehaviour
    {
        #region Instance

        /// <summary>
        ///     Gets the current instance of FlightEngineerCore.
        /// </summary>
        public static FlightEngineerCore Instance { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        ///     Create base Flight Engineer child objects.
        /// </summary>
        private void Awake()
        {
            Instance = this;

            this.ActionMenu = this.gameObject.AddComponent<ActionMenu>();
            this.DisplayStack = this.gameObject.AddComponent<DisplayStack>();
            this.SectionWindows = new List<SectionWindow>();
            this.SectionEditors = new List<SectionEditor>();
            this.UpdatableModules = new List<IUpdatable>();
        }

        /// <summary>
        ///     Initialises the object's state on creation.
        /// </summary>
        private void Start()
        {
            SectionLibrary.Instance.Load();
            ReadoutLibrary.Instance.Reset();
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets and sets the ActionMenu object.
        /// </summary>
        public ActionMenu ActionMenu { get; set; }

        /// <summary>
        ///     Gets and sets the DisplayStack object.
        /// </summary>
        public DisplayStack DisplayStack { get; set; }

        /// <summary>
        ///     Gets the section windows for floating sections.
        /// </summary>
        public List<SectionWindow> SectionWindows { get; private set; }

        /// <summary>
        ///     Gets the editor windows for sections with open editors.
        /// </summary>
        public List<SectionEditor> SectionEditors { get; private set; }

        /// <summary>
        ///     Gets the list of currently running updatable modules.
        /// </summary>
        public List<IUpdatable> UpdatableModules { get; private set; }

        #endregion

        #region Updating

        /// <summary>
        ///     Update all required Flight Engineer objects.
        /// </summary>
        private void Update()
        {
            SectionLibrary.Instance.Update();
            this.UpdateModules();
        }

        /// <summary>
        ///     Update all updatable modules.
        /// </summary>
        private void UpdateModules()
        {
            foreach (var updatable in this.UpdatableModules)
            {
                if (updatable is IUpdateRequest)
                {
                    var request = updatable as IUpdateRequest;
                    if (request.UpdateRequested)
                    {
                        updatable.Update();
                        request.UpdateRequested = false;
                    }
                }
                else
                {
                    updatable.Update();
                }
            }
        }

        #endregion

        #region Destruction

        /// <summary>
        ///     Force the destruction of child objects on core destruction.
        /// </summary>
        private void OnDestroy()
        {
            SectionLibrary.Instance.Save();

            foreach (var window in this.SectionWindows)
            {
                print("[FlightEngineer]: Destroying Floating Window for " + window.ParentSection.Name);
                Destroy(window);
            }

            foreach (var editor in this.SectionEditors)
            {
                print("[FlightEngineer]: Destroying Editor Window for " + editor.ParentSection.Name);
                Destroy(editor);
            }

            if (this.ActionMenu != null)
            {
                print("[FlightEngineer]: Destroying ActionMenu");
                DestroyImmediate(this.ActionMenu);
            }

            if (this.DisplayStack != null)
            {
                print("[FlightEngineer]: Destroying DisplayStack");
                DestroyImmediate(this.DisplayStack);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Creates a section window, adds it to the FlightEngineerCore and returns a reference to it.
        /// </summary>
        public SectionWindow AddSectionWindow(SectionModule section)
        {
            var window = this.gameObject.AddComponent<SectionWindow>();
            window.ParentSection = section;
            window.WindowPosition = new Rect(section.FloatingPositionX, section.FloatingPositionY, 0, 0);
            this.SectionWindows.Add(window);
            return window;
        }

        /// <summary>
        ///     Creates a section editor, adds it to the FlightEngineerCore and returns a reference to it.
        /// </summary>
        public SectionEditor AddSectionEditor(SectionModule section)
        {
            var editor = this.gameObject.AddComponent<SectionEditor>();
            editor.ParentSection = section;
            editor.WindowPosition = new Rect(section.EditorPositionX, section.EditorPositionY, SectionEditor.Width, SectionEditor.Height);
            this.SectionEditors.Add(editor);
            return editor;
        }

        /// <summary>
        ///     Adds an updatable object to be automatically updated every frame and will ignore duplicate objects.
        /// </summary>
        public void AddUpdatable(IUpdatable updatable)
        {
            if (!this.UpdatableModules.Contains(updatable))
            {
                this.UpdatableModules.Add(updatable);
            }
        }

        #endregion
    }
}