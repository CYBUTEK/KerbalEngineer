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

using KerbalEngineer.Flight.Readouts;
using KerbalEngineer.Settings;

#endregion

namespace KerbalEngineer.Flight.Sections
{
    public class SectionLibrary
    {
        #region Instance

        private static readonly SectionLibrary instance = new SectionLibrary();

        /// <summary>
        ///     Gets the current instance of the section library.
        /// </summary>
        public static SectionLibrary Instance
        {
            get { return instance; }
        }

        #endregion

        #region Constructors

        /// <summary>
        ///     Sets up and populates the library with the stock sections on creation.
        /// </summary>
        private SectionLibrary()
        {
            this.StockSections = new List<SectionModule>();
            this.CustomSections = new List<SectionModule>();

            this.StockSections.Add(new SectionModule
            {
                Name = "ORBITAL",
                Abbreviation = "ORBT",
                ReadoutModules = ReadoutLibrary.Instance.GetCategory(ReadoutCategory.Orbital).Where(r => r.IsDefault).ToList()
            });

            this.StockSections.Add(new SectionModule
            {
                Name = "SURFACE",
                Abbreviation = "SURF",
                ReadoutModules = ReadoutLibrary.Instance.GetCategory(ReadoutCategory.Surface).Where(r => r.IsDefault).ToList()
            });

            this.StockSections.Add(new SectionModule
            {
                Name = "VESSEL",
                Abbreviation = "VESL",
                ReadoutModules = ReadoutLibrary.Instance.GetCategory(ReadoutCategory.Vessel).Where(r => r.IsDefault).ToList()
            });

            this.StockSections.Add(new SectionModule
            {
                Name = "RENDEZVOUS",
                Abbreviation = "RDZV",
                ReadoutModules = ReadoutLibrary.Instance.GetCategory(ReadoutCategory.Rendezvous).Where(r => r.IsDefault).ToList()
            });
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets and sets a list of stock sections
        /// </summary>
        public List<SectionModule> StockSections { get; set; }

        /// <summary>
        ///     Gets and sets a list of custom sections.
        /// </summary>
        public List<SectionModule> CustomSections { get; set; }

        /// <summary>
        ///     Gets the number of sections that are being drawn on the display stack.
        /// </summary>
        public int NumberOfStackSections { get; private set; }

        /// <summary>
        ///     Gets the number of total sections that are stored in the library.
        /// </summary>
        public int NumberOfSections { get; private set; }

        #endregion

        #region Updating

        /// <summary>
        ///     Update all of the sections and process section counts.
        /// </summary>
        public void Update()
        {
            this.NumberOfStackSections = 0;
            this.NumberOfSections = 0;

            this.UpdateSections(this.StockSections);
            this.UpdateSections(this.CustomSections);
        }

        /// <summary>
        ///     Updates a list of sections and increments the section counts.
        /// </summary>
        private void UpdateSections(IEnumerable<SectionModule> sections)
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

                        this.NumberOfStackSections++;
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

                this.NumberOfSections++;
            }
        }

        #endregion

        #region Saving and Loading

        /// <summary>
        ///     Saves the state of all the stored sections.
        /// </summary>
        public void Save()
        {
            var handler = new SettingHandler();
            handler.Set("StockSections", this.StockSections);
            handler.Set("CustomSections", this.CustomSections);
            handler.Save("SectionLibrary.xml");
        }

        /// <summary>
        ///     Loads the state of all stored sections.
        /// </summary>
        public void Load()
        {
            var handler = SettingHandler.Load("SectionLibrary.xml", new[] {typeof(List<SectionModule>)});
            this.StockSections = handler.Get("StockSections", this.StockSections);
            this.CustomSections = handler.Get("CustomSections", this.CustomSections);
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Gets a section that has the specified name.
        /// </summary>
        public SectionModule GetSection(string name)
        {
            return this.GetStockSection(name) ?? this.GetCustomSection(name);
        }

        /// <summary>
        ///     Gets a stock section that has the specified name.
        /// </summary>
        public SectionModule GetStockSection(string name)
        {
            return this.StockSections.FirstOrDefault(s => s.Name == name);
        }

        /// <summary>
        ///     Gets a custom section that has the specified name.
        /// </summary>
        public SectionModule GetCustomSection(string name)
        {
            return this.CustomSections.FirstOrDefault(s => s.Name == name);
        }

        /// <summary>
        ///     Removes a section with the specified name.
        /// </summary>
        public bool RemoveSection(string name)
        {
            return this.RemoveStockSection(name) || this.RemoveCustomSection(name);
        }

        /// <summary>
        ///     Removes as stock section with the specified name.
        /// </summary>
        public bool RemoveStockSection(string name)
        {
            return this.StockSections.Remove(this.GetStockSection(name));
        }

        /// <summary>
        ///     Removes a custom section witht he specified name.
        /// </summary>
        public bool RemoveCustomSection(string name)
        {
            return this.CustomSections.Remove(this.GetCustomSection(name));
        }

        #endregion
    }
}