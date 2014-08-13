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

using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

using KerbalEngineer.Flight.Readouts;
using KerbalEngineer.Settings;

#endregion

namespace KerbalEngineer.Flight.Sections
{
    public static class SectionLibrary
    {
        #region Constructors

        /// <summary>
        ///     Sets up and populates the library with the stock sections on creation.
        /// </summary>
        static SectionLibrary()
        {
            StockSections = new List<SectionModule>();
            CustomSections = new List<SectionModule>();

            StockSections.Add(new SectionModule
            {
                Name = "ORBITAL",
                Abbreviation = "ORBT",
                ReadoutModules = ReadoutLibrary.GetCategory(ReadoutCategory.GetCategory("Orbital")).Where(r => r.IsDefault).ToList()
            });

            StockSections.Add(new SectionModule
            {
                Name = "SURFACE",
                Abbreviation = "SURF",
                ReadoutModules = ReadoutLibrary.GetCategory(ReadoutCategory.GetCategory("Surface")).Where(r => r.IsDefault).ToList()
            });

            StockSections.Add(new SectionModule
            {
                Name = "VESSEL",
                Abbreviation = "VESL",
                ReadoutModules = ReadoutLibrary.GetCategory(ReadoutCategory.GetCategory("Vessel")).Where(r => r.IsDefault).ToList()
            });

            StockSections.Add(new SectionModule
            {
                Name = "RENDEZVOUS",
                Abbreviation = "RDZV",
                ReadoutModules = ReadoutLibrary.GetCategory(ReadoutCategory.GetCategory("Rendezvous")).Where(r => r.IsDefault).ToList()
            });
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets and sets a list of stock sections
        /// </summary>
        public static List<SectionModule> StockSections { get; set; }

        /// <summary>
        ///     Gets and sets a list of custom sections.
        /// </summary>
        public static List<SectionModule> CustomSections { get; set; }

        /// <summary>
        ///     Gets the number of sections that are being drawn on the display stack.
        /// </summary>
        public static int NumberOfStackSections { get; private set; }

        /// <summary>
        ///     Gets the number of total sections that are stored in the library.
        /// </summary>
        public static int NumberOfSections { get; private set; }

        #endregion

        #region Updating

        /// <summary>
        ///     Fixed update all of the sections.
        /// </summary>
        public static void FixedUpdate()
        {
            FixedUpdateSections(StockSections);
            FixedUpdateSections(CustomSections);
        }

        /// <summary>
        ///     Fixed updates a list of sections.
        /// </summary>
        private static void FixedUpdateSections(IEnumerable<SectionModule> sections)
        {
            foreach (var section in sections)
            {
                if (section.IsVisible)
                {
                    section.FixedUpdate();
                }
            }
        }

        /// <summary>
        ///     Update all of the sections and process section counts.
        /// </summary>
        public static void Update()
        {
            NumberOfStackSections = 0;
            NumberOfSections = 0;

            UpdateSections(StockSections);
            UpdateSections(CustomSections);
        }

        /// <summary>
        ///     Updates a list of sections and increments the section counts.
        /// </summary>
        private static void UpdateSections(IEnumerable<SectionModule> sections)
        {
            foreach (var section in sections)
            {
                if (section.IsVisible)
                {
                    if (!section.IsFloating)
                    {
                        foreach (var readout in section.ReadoutModules)
                        {
                            if (readout.ResizeRequested)
                            {
                                DisplayStack.Instance.RequestResize();
                                readout.ResizeRequested = false;
                            }
                        }

                        NumberOfStackSections++;
                    }
                    else
                    {
                        foreach (var readout in section.ReadoutModules)
                        {
                            if (readout.ResizeRequested)
                            {
                                section.Window.RequestResize();
                                readout.ResizeRequested = false;
                            }
                        }
                    }
                    section.Update();
                }

                NumberOfSections++;
            }
        }

        #endregion

        #region Saving and Loading

        /// <summary>
        ///     Saves the state of all the stored sections.
        /// </summary>
        public static void Save()
        {
            var handler = new SettingHandler();
            handler.Set("StockSections", StockSections);
            handler.Set("CustomSections", CustomSections);
            handler.Save("SectionLibrary.xml");
        }

        /// <summary>
        ///     Loads the state of all stored sections.
        /// </summary>
        public static void Load()
        {
            var handler = SettingHandler.Load("SectionLibrary.xml", new[] {typeof(List<SectionModule>)});
            StockSections = handler.Get("StockSections", StockSections);
            CustomSections = handler.Get("CustomSections", CustomSections);

            foreach (var section in StockSections)
            {
                section.ClearNullReadouts();
            }
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Gets a section that has the specified name.
        /// </summary>
        public static SectionModule GetSection(string name)
        {
            return GetStockSection(name) ?? GetCustomSection(name);
        }

        /// <summary>
        ///     Gets a stock section that has the specified name.
        /// </summary>
        public static SectionModule GetStockSection(string name)
        {
            return StockSections.FirstOrDefault(s => s.Name == name);
        }

        /// <summary>
        ///     Gets a custom section that has the specified name.
        /// </summary>
        public static SectionModule GetCustomSection(string name)
        {
            return CustomSections.FirstOrDefault(s => s.Name == name);
        }

        /// <summary>
        ///     Removes a section with the specified name.
        /// </summary>
        public static bool RemoveSection(string name)
        {
            return RemoveStockSection(name) || RemoveCustomSection(name);
        }

        /// <summary>
        ///     Removes as stock section with the specified name.
        /// </summary>
        public static bool RemoveStockSection(string name)
        {
            return StockSections.Remove(GetStockSection(name));
        }

        /// <summary>
        ///     Removes a custom section witht he specified name.
        /// </summary>
        public static bool RemoveCustomSection(string name)
        {
            return CustomSections.Remove(GetCustomSection(name));
        }

        #endregion
    }
}