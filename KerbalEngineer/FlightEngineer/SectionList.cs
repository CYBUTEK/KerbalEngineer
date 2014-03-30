// Project:	KerbalEngineer
// Author:	CYBUTEK
// License:	Attribution-NonCommercial-ShareAlike 3.0 Unported

#region Using Directives

using System.Collections.Generic;
using System.Linq;

using KerbalEngineer.FlightEngineer.Surface;
using KerbalEngineer.Settings;
using KerbalEngineer.Simulation;

using UnityEngine;

#endregion

namespace KerbalEngineer.FlightEngineer
{
    public class SectionList
    {
        #region Static Fields

        private static bool _hasLoadedSections;

        #endregion

        #region Instance

        private static SectionList _instance;

        /// <summary>
        ///     Gets the current instance of the section list.
        /// </summary>
        public static SectionList Instance
        {
            get { return _instance ?? (_instance = new SectionList()); }
        }

        #endregion

        #region Properties

        private List<Section> fixedSections = new List<Section>();
        private bool hasAttachedSections;
        private bool hasVisibleSections;
        private bool requireResize;

        private List<Section> userSections = new List<Section>();

        /// <summary>
        ///     Gets and sets the available fixed sections.
        /// </summary>
        public List<Section> FixedSections
        {
            get { return this.fixedSections; }
            set { this.fixedSections = value; }
        }

        /// <summary>
        ///     Gets and sets the available user sections.
        /// </summary>
        public List<Section> UserSections
        {
            get { return this.userSections; }
            set { this.userSections = value; }
        }

        /// <summary>
        ///     Gets and sets whether to resize all displays.
        /// </summary>
        public bool ResizeRequested
        {
            get { return this.requireResize; }
        }

        /// <summary>
        ///     Gets whether there are visible sections.
        /// </summary>
        public bool HasVisibleSections
        {
            get { return this.hasVisibleSections; }
        }

        /// <summary>
        ///     Gets whether there are attached sections.
        /// </summary>
        public bool HasAttachedSections
        {
            get { return this.hasAttachedSections; }
        }

        #endregion

        #region Initialisation

        private SectionList()
        {
            this.fixedSections.Add(new Section
            {
                Title = "Orbital",
                Readouts = ReadoutList.Instance.GetCategory(ReadoutCategory.Orbital),
                Categories = new List<ReadoutCategory>
                {
                    ReadoutCategory.Orbital
                },
                ShortTitle = "ORBT"
            });

            this.fixedSections.Add(new Section
            {
                Title = "Surface",
                Readouts = ReadoutList.Instance.GetCategory(ReadoutCategory.Surface),
                Categories = new List<ReadoutCategory>
                {
                    ReadoutCategory.Surface
                },
                ShortTitle = "SURF"
            });

            this.fixedSections.Add(new Section
            {
                Title = "Vessel",
                Readouts = ReadoutList.Instance.GetCategory(ReadoutCategory.Vessel),
                Categories = new List<ReadoutCategory>
                {
                    ReadoutCategory.Vessel
                },
                ShortTitle = "VESL"
            });

            this.fixedSections.Add(new Section
            {
                Title = "Rendezvous",
                Readouts = ReadoutList.Instance.GetCategory(ReadoutCategory.Rendezvous),
                Categories = new List<ReadoutCategory>
                {
                    ReadoutCategory.Rendezvous
                },
                ShortTitle = "RDZV"
            });
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Gets the fixed section with the provided name.
        /// </summary>
        public Section GetFixedSection(string name)
        {
            return this.fixedSections.FirstOrDefault(section => section.Title == name);
        }

        /// <summary>
        ///     Gets the user section with the provided name.
        /// </summary>
        public Section GetUserSection(string name)
        {
            return this.userSections.FirstOrDefault(section => section.Title == name);
        }

        public void RequestResize()
        {
            this.requireResize = true;
        }

        #endregion

        #region Update

        public void Update()
        {
            if (_hasLoadedSections)
            {
                _hasLoadedSections = false;
            }

            // If a resize is required propagate it to the handling objects.
            if (this.requireResize)
            {
                this.requireResize = false;
                FlightDisplay.Instance.RequireResize = true;

                foreach (var section in this.fixedSections)
                {
                    section.Window.RequireResize = true;
                }

                foreach (var section in this.userSections)
                {
                    section.Window.RequireResize = true;
                }
            }

            this.hasVisibleSections = false;
            this.hasAttachedSections = false;

            // Update all visible fixed sections.
            foreach (var section in this.fixedSections)
            {
                if (!section.Visible)
                {
                    continue;
                }

                if (!this.hasVisibleSections)
                {
                    this.hasVisibleSections = true;
                }
                if (!section.Window.Visible && !this.hasAttachedSections)
                {
                    this.hasAttachedSections = true;
                }
                section.Update();
            }

            // Update all visible user sections.
            foreach (var section in this.userSections)
            {
                if (!section.Visible)
                {
                    continue;
                }

                if (!this.hasVisibleSections)
                {
                    this.hasVisibleSections = true;
                }
                if (!section.Window.Visible && !this.hasAttachedSections)
                {
                    this.hasAttachedSections = true;
                }
                section.Update();
            }

            AtmosphericDetails.Instance.Update();
            SimulationManager.Gravity = FlightGlobals.getGeeForceAtPosition(FlightGlobals.ActiveVessel.GetWorldPos3D()).magnitude;
            SimulationManager.Atmosphere = FlightGlobals.getAtmDensity(FlightGlobals.ActiveVessel.staticPressure);
            SimulationManager.TryStartSimulation();
        }

        #endregion

        #region Save and Load

        /// <summary>
        ///     Saves the settings associated with the section list.
        /// </summary>
        public void Save()
        {
            var fixedSectionNames = new List<string>();
            var userSectionNames = new List<string>();

            foreach (var section in this.fixedSections)
            {
                fixedSectionNames.Add(section.Title);
                section.Save();
            }

            foreach (var section in this.userSections)
            {
                userSectionNames.Add(section.Title);
                section.Save();
            }

            try
            {
                var list = new SettingList();
                list.AddSetting("fixed_sections", fixedSectionNames);
                list.AddSetting("user_sections", userSectionNames);
                SettingList.SaveToFile(EngineerGlobals.AssemblyPath + "Settings/FlightSections", list);

                MonoBehaviour.print("[KerbalEngineer/FlightSections]: Successfully saved settings.");
            }
            catch
            {
                MonoBehaviour.print("[KerbalEngineer/FlightSections]: Failed to save settings.");
            }
        }

        /// <summary>
        ///     Loads the settings associated with the section list.
        /// </summary>
        public void Load()
        {
            try
            {
                if (_hasLoadedSections)
                {
                    return;
                }

                _hasLoadedSections = true;

                var list = SettingList.CreateFromFile(EngineerGlobals.AssemblyPath + "Settings/FlightSections");

                var fixedSectionNames = list.GetSetting("fixed_sections", new List<string>()) as List<string>;
                var userSectionNames = list.GetSetting("user_sections", new List<string>()) as List<string>;

                // Load fixed sections.
                foreach (var name in fixedSectionNames)
                {
                    var section = this.GetFixedSection(name);

                    if (section == null)
                    {
                        section = new Section(true, false)
                        {
                            Title = name
                        };
                        this.fixedSections.Add(section);
                    }
                    else
                    {
                        RenderingManager.AddToPostDrawQueue(0, section.Window.Draw);
                        RenderingManager.AddToPostDrawQueue(0, section.EditDisplay.Draw);
                    }

                    section.Load();
                }

                // Load user sections.
                foreach (var name in userSectionNames)
                {
                    var section = this.GetUserSection(name);

                    if (section == null)
                    {
                        section = new Section(true, false)
                        {
                            Title = name
                        };
                        this.userSections.Add(section);
                    }
                    else
                    {
                        RenderingManager.AddToPostDrawQueue(0, section.Window.Draw);
                        RenderingManager.AddToPostDrawQueue(0, section.EditDisplay.Draw);
                    }

                    section.Load();
                }

                MonoBehaviour.print("[KerbalEngineer/FlightSections]: Successfully loaded settings.");
            }
            catch
            {
                MonoBehaviour.print("[KerbalEngineer/FlightSections]: Failed to load settings.");
            }
        }

        #endregion
    }
}