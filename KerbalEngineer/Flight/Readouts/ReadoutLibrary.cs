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

using KerbalEngineer.Flight.Readouts.Orbital;
using KerbalEngineer.Flight.Readouts.Rendezvous;
using KerbalEngineer.Flight.Readouts.Surface;
using KerbalEngineer.Flight.Readouts.Vessel;
using KerbalEngineer.Settings;

using AltitudeSeaLevel = KerbalEngineer.Flight.Readouts.Surface.AltitudeSeaLevel;
using ApoapsisHeight = KerbalEngineer.Flight.Readouts.Orbital.ApoapsisHeight;
using OrbitalPeriod = KerbalEngineer.Flight.Readouts.Orbital.OrbitalPeriod;
using PeriapsisHeight = KerbalEngineer.Flight.Readouts.Orbital.PeriapsisHeight;
using TimeToApoapsis = KerbalEngineer.Flight.Readouts.Orbital.TimeToApoapsis;
using TimeToPeriapsis = KerbalEngineer.Flight.Readouts.Orbital.TimeToPeriapsis;

#endregion

namespace KerbalEngineer.Flight.Readouts
{
    public class ReadoutLibrary
    {
        #region Instance

        private static readonly ReadoutLibrary instance = new ReadoutLibrary();

        /// <summary>
        ///     Gets the current instance of the readout library.
        /// </summary>
        public static ReadoutLibrary Instance
        {
            get { return instance; }
        }

        #endregion

        #region Fields

        private List<ReadoutModule> readoutModules = new List<ReadoutModule>();

        #endregion

        #region Constructors

        /// <summary>
        ///     Sets up and populates the readout library with the stock readouts.
        /// </summary>
        private ReadoutLibrary()
        {
            // Orbital
            this.readoutModules.Add(new ApoapsisHeight());
            this.readoutModules.Add(new PeriapsisHeight());
            this.readoutModules.Add(new TimeToApoapsis());
            this.readoutModules.Add(new TimeToPeriapsis());
            this.readoutModules.Add(new Inclination());
            this.readoutModules.Add(new Eccentricity());
            this.readoutModules.Add(new OrbitalSpeed());
            this.readoutModules.Add(new OrbitalPeriod());
            this.readoutModules.Add(new LongitudeOfAscendingNode());
            this.readoutModules.Add(new LongitudeOfPeriapsis());
            this.readoutModules.Add(new SemiMajorAxis());
            this.readoutModules.Add(new SemiMinorAxis());

            // Surface
            this.readoutModules.Add(new AltitudeSeaLevel());
            this.readoutModules.Add(new AltitudeTerrain());
            this.readoutModules.Add(new VerticalSpeed());
            this.readoutModules.Add(new HorizontalSpeed());
            this.readoutModules.Add(new Longitude());
            this.readoutModules.Add(new Latitude());
            this.readoutModules.Add(new GeeForce());
            this.readoutModules.Add(new TerminalVelocity());
            this.readoutModules.Add(new AtmosphericEfficiency());
            this.readoutModules.Add(new Biome());
            this.readoutModules.Add(new Slope());
            this.readoutModules.Add(new ImpactTime());
            this.readoutModules.Add(new ImpactLongitude());
            this.readoutModules.Add(new ImpactLatitude());
            this.readoutModules.Add(new ImpactAltitude());
            this.readoutModules.Add(new ImpactBiome());

            // Vessel
            this.readoutModules.Add(new DeltaVStaged());
            this.readoutModules.Add(new DeltaVTotal());
            this.readoutModules.Add(new SpecificImpulse());
            this.readoutModules.Add(new Mass());
            this.readoutModules.Add(new Thrust());
            this.readoutModules.Add(new ThrustToWeight());
            this.readoutModules.Add(new SimulationDelay());

            // Rendezvous
            this.readoutModules.Add(new TargetSelector());
            this.readoutModules.Add(new PhaseAngle());
            this.readoutModules.Add(new InterceptAngle());
            this.readoutModules.Add(new RelativeInclination());
            this.readoutModules.Add(new AngleToAscendingNode());
            this.readoutModules.Add(new AngleToDescendingNode());
            this.readoutModules.Add(new Rendezvous.AltitudeSeaLevel());
            this.readoutModules.Add(new Rendezvous.ApoapsisHeight());
            this.readoutModules.Add(new Rendezvous.PeriapsisHeight());
            this.readoutModules.Add(new Rendezvous.TimeToApoapsis());
            this.readoutModules.Add(new Rendezvous.TimeToPeriapsis());
            this.readoutModules.Add(new Distance());
            this.readoutModules.Add(new Rendezvous.OrbitalPeriod());

            this.LoadHelpStrings();
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets and sets the available readout modules.
        /// </summary>
        public List<ReadoutModule> ReadoutModules
        {
            get { return this.readoutModules; }
            set { this.readoutModules = value; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Gets a readout module with the specified name or class name. (Returns null if not found.)
        /// </summary>
        public ReadoutModule GetReadoutModule(string name)
        {
            return this.readoutModules.FirstOrDefault(r => r.Name == name || r.GetType().Name == name || r.Category + "." + r.GetType().Name == name);
        }

        /// <summary>
        ///     Gets a list of readout modules which are associated with the specified category.
        /// </summary>
        public List<ReadoutModule> GetCategory(ReadoutCategory category)
        {
            return this.readoutModules.Where(r => r.Category == category).ToList();
        }

        /// <summary>
        ///     Resets all the readout modules.
        /// </summary>
        public void Reset()
        {
            foreach (var readout in this.readoutModules)
            {
                readout.Reset();
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Loads the help strings from file.
        /// </summary>
        private void LoadHelpStrings()
        {
            var handler = SettingHandler.Load("HelpStrings.xml");
            foreach (var readout in this.readoutModules)
            {
                readout.HelpString = handler.GetSet(readout.Category + "." + readout.GetType().Name, readout.HelpString);
            }
            handler.Save("HelpStrings.xml");
        }

        #endregion
    }
}