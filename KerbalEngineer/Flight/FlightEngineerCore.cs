// 
//     Kerbal Engineer Redux
// 
//     Copyright (C) 2014 CYBUTEK
// 
//     This program is free software: you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     This program is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General Public License for more details.
// 
//     You should have received a copy of the GNU General Public License
//     along with this program.  If not, see <http://www.gnu.org/licenses/>.
// 

#region Using Directives

#endregion

namespace KerbalEngineer.Flight
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Extensions;
    using Readouts;
    using Sections;
    using Settings;
    using UnityEngine;
    using VesselSimulator;

    #endregion

    /// <summary>
    ///     Core management system for the Flight Engineer.
    /// </summary>
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public sealed class FlightEngineerCore : MonoBehaviour
    {
        #region Instance

        /// <summary>
        ///     Gets the current instance of FlightEngineerCore.
        /// </summary>
        public static FlightEngineerCore Instance { get; private set; }

        #endregion

        #region Fields

        private static bool isCareerMode = true;
        private static bool isKerbalLimited = true;
        private static bool isTrackingStationLimited = true;

        #endregion

        #region Constructors

        static FlightEngineerCore()
        {
            try
            {
                var handler = SettingHandler.Load("FlightEngineerCore.xml");
                handler.Get("isCareerMode", ref isCareerMode);
                handler.Get("isKerbalLimited", ref isKerbalLimited);
                handler.Get("isTrackingStationLimited", ref isTrackingStationLimited);
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets and sets whether to the Flight Engineer should be run using career limitations.
        /// </summary>
        public static bool IsCareerMode
        {
            get { return isCareerMode; }
            set
            {
                try
                {
                    if (isCareerMode != value)
                    {
                        var handler = SettingHandler.Load("FlightEngineerCore.xml");
                        handler.Set("isCareerMode", value);
                        handler.Save("FlightEngineerCore.xml");
                    }
                    isCareerMode = value;
                }
                catch (Exception ex)
                {
                    Logger.Exception(ex);
                }
            }
        }

        /// <summary>
        ///     Gets whether the FlightEngineer should be displayed.
        /// </summary>
        public static bool IsDisplayable
        {
            get
            {
                if (MainCanvasUtil.MainCanvas.enabled == false)
                {
                    return false;
                }

                if (isCareerMode)
                {
                    if (isKerbalLimited && FlightGlobals.ActiveVessel.GetVesselCrew().Exists(c => c.experienceTrait.TypeName == "Engineer"))
                    {
                        return true;
                    }
                    if (isTrackingStationLimited && ScenarioUpgradeableFacilities.GetFacilityLevel(SpaceCenterFacility.TrackingStation) == 1.0f)
                    {
                        return true;
                    }
                    return FlightGlobals.ActiveVessel.parts.Any(p => p.HasModule<FlightEngineerModule>());
                }

                return true;
            }
        }

        /// <summary>
        ///     Gets and sets whether to the Flight Engineer should be kerbal limited.
        /// </summary>
        public static bool IsKerbalLimited
        {
            get { return isKerbalLimited; }
            set
            {
                try
                {
                    if (isKerbalLimited != value)
                    {
                        var handler = SettingHandler.Load("FlightEngineerCore.xml");
                        handler.Set("isKerbalLimited", value);
                        handler.Save("FlightEngineerCore.xml");
                    }
                    isKerbalLimited = value;
                }
                catch (Exception ex)
                {
                    Logger.Exception(ex);
                }
            }
        }

        /// <summary>
        ///     Gets and sets whether to the Flight Engineer should be tracking station limited.
        /// </summary>
        public static bool IsTrackingStationLimited
        {
            get { return isTrackingStationLimited; }
            set
            {
                try
                {
                    if (isTrackingStationLimited != value)
                    {
                        var handler = SettingHandler.Load("FlightEngineerCore.xml");
                        handler.Set("isTrackingStationLimited", value);
                        handler.Save("FlightEngineerCore.xml");
                    }
                    isTrackingStationLimited = value;
                }
                catch (Exception ex)
                {
                    Logger.Exception(ex);
                }
            }
        }

        /// <summary>
        ///     Gets the editor windows for sections with open editors.
        /// </summary>
        public List<SectionEditor> SectionEditors { get; private set; }

        /// <summary>
        ///     Gets the section windows for floating sections.
        /// </summary>
        public List<SectionWindow> SectionWindows { get; private set; }

        /// <summary>
        ///     Gets the list of currently running updatable modules.
        /// </summary>
        public List<IUpdatable> UpdatableModules { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        ///     Creates a section editor, adds it to the FlightEngineerCore and returns a reference to it.
        /// </summary>
        public SectionEditor AddSectionEditor(SectionModule section)
        {
            try
            {
                var editor = this.gameObject.AddComponent<SectionEditor>();
                editor.ParentSection = section;
                editor.Position = new Rect(section.EditorPositionX, section.EditorPositionY, SectionEditor.Width, SectionEditor.Height);
                this.SectionEditors.Add(editor);
                return editor;
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
                return null;
            }
        }

        /// <summary>
        ///     Creates a section window, adds it to the FlightEngineerCore and returns a reference to it.
        /// </summary>
        public SectionWindow AddSectionWindow(SectionModule section)
        {
            try
            {
                var window = this.gameObject.AddComponent<SectionWindow>();
                window.ParentSection = section;
                window.WindowPosition = new Rect(section.FloatingPositionX, section.FloatingPositionY, 0, 0);
                this.SectionWindows.Add(window);
                return window;
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
                return null;
            }
        }

        /// <summary>
        ///     Adds an updatable object to be automatically updated every frame and will ignore duplicate objects.
        /// </summary>
        public void AddUpdatable(IUpdatable updatable)
        {
            try
            {
                if (!this.UpdatableModules.Contains(updatable))
                {
                    this.UpdatableModules.Add(updatable);
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }

        /// <summary>
        ///     Create base Flight Engineer child objects.
        /// </summary>
        private void Awake()
        {
            try
            {
                Instance = this;

                this.SectionWindows = new List<SectionWindow>();
                this.SectionEditors = new List<SectionEditor>();
                this.UpdatableModules = new List<IUpdatable>();

                SimManager.UpdateModSettings();

                Logger.Log("FlightEngineerCore->Awake");
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }

        /// <summary>
        ///     Fixed update all required Flight Engineer objects.
        /// </summary>
        private void FixedUpdate()
        {
            if (FlightGlobals.ActiveVessel == null)
            {
                return;
            }

            try
            {
                SectionLibrary.FixedUpdate();
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }

        /// <summary>
        ///     Force the destruction of child objects on core destruction.
        /// </summary>
        private void OnDestroy()
        {
            try
            {
                SectionLibrary.Save();

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

                Logger.Log("FlightEngineerCore->OnDestroy");
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }

        /// <summary>
        ///     Initialises the object's state on creation.
        /// </summary>
        private void Start()
        {
            try
            {
                SectionLibrary.Load();
                ReadoutLibrary.Reset();
                Logger.Log("FlightEngineerCore->Start");
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }

        /// <summary>
        ///     Update all required Flight Engineer objects.
        /// </summary>
        private void Update()
        {
            if (FlightGlobals.ActiveVessel == null)
            {
                return;
            }

            try
            {
                SectionLibrary.Update();
                this.UpdateModules();
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }

        /// <summary>
        ///     Update all updatable modules.
        /// </summary>
        private void UpdateModules()
        {
            try
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
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }

        #endregion
    }
}