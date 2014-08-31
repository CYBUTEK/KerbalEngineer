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

using System;
using System.Collections.Generic;
using System.Linq;

using KerbalEngineer.Flight.Readouts.Miscellaneous;
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
    public static class ReadoutLibrary
    {
        #region Fields

        private static List<ReadoutModule> readouts = new List<ReadoutModule>();

        #endregion

        #region Constructors

        /// <summary>
        ///     Sets up and populates the readout library with the stock readouts.
        /// </summary>
        static ReadoutLibrary()
        {
            try
            {
                ReadoutCategory.SetCategory("Orbital", "Readout for orbital manovoeures.");
                ReadoutCategory.SetCategory("Surface", "Surface and atmospheric readouts.");
                ReadoutCategory.SetCategory("Vessel", "Vessel performance statistics.");
                ReadoutCategory.SetCategory("Rendezvous", "Readouts for rendezvous manovoeures.");
                ReadoutCategory.SetCategory("Miscellaneous", "Miscellaneous readouts.");

                // Orbital
                readouts.Add(new ApoapsisHeight());
                readouts.Add(new PeriapsisHeight());
                readouts.Add(new TimeToApoapsis());
                readouts.Add(new TimeToPeriapsis());
                readouts.Add(new Inclination());
                readouts.Add(new TimeToEquatorialAscendingNode());
                readouts.Add(new TimeToEquatorialDescendingNode());
                readouts.Add(new AngleToEquatorialAscendingNode());
                readouts.Add(new AngleToEquatorialDescendingNode());
                readouts.Add(new Eccentricity());
                readouts.Add(new OrbitalSpeed());
                readouts.Add(new OrbitalPeriod());
                readouts.Add(new LongitudeOfAscendingNode());
                readouts.Add(new LongitudeOfPeriapsis());
                readouts.Add(new TrueAnomaly());
                readouts.Add(new SemiMajorAxis());
                readouts.Add(new SemiMinorAxis());

                // Surface
                readouts.Add(new AltitudeSeaLevel());
                readouts.Add(new AltitudeTerrain());
                readouts.Add(new VerticalSpeed());
                readouts.Add(new HorizontalSpeed());
                readouts.Add(new Longitude());
                readouts.Add(new Latitude());
                readouts.Add(new GeeForce());
                readouts.Add(new TerminalVelocity());
                readouts.Add(new AtmosphericEfficiency());
                readouts.Add(new Biome());
                readouts.Add(new Slope());
                readouts.Add(new ImpactTime());
                readouts.Add(new ImpactLongitude());
                readouts.Add(new ImpactLatitude());
                readouts.Add(new ImpactAltitude());
                readouts.Add(new ImpactBiome());

                // Vessel
                readouts.Add(new DeltaVStaged());
                readouts.Add(new DeltaVTotal());
                readouts.Add(new SpecificImpulse());
                readouts.Add(new Mass());
                readouts.Add(new Thrust());
                readouts.Add(new ThrustToWeight());
                readouts.Add(new Acceleration());
                readouts.Add(new IntakeAirDemand());
                readouts.Add(new IntakeAirSupply());
                readouts.Add(new IntakeAirSupplyDemand());
                readouts.Add(new SimulationDelay());

                // Rendezvous
                readouts.Add(new TargetSelector());
                readouts.Add(new PhaseAngle());
                readouts.Add(new InterceptAngle());
                readouts.Add(new RelativeInclination());
                readouts.Add(new TimeToRelativeAscendingNode());
                readouts.Add(new TimeToRelativeDescendingNode());
                readouts.Add(new AngleToRelativeAscendingNode());
                readouts.Add(new AngleToRelativeDescendingNode());
                readouts.Add(new Rendezvous.AltitudeSeaLevel());
                readouts.Add(new Rendezvous.ApoapsisHeight());
                readouts.Add(new Rendezvous.PeriapsisHeight());
                readouts.Add(new Rendezvous.TimeToApoapsis());
                readouts.Add(new Rendezvous.TimeToPeriapsis());
                readouts.Add(new Distance());
                readouts.Add(new Rendezvous.OrbitalPeriod());

                // Misc
                readouts.Add(new Separator());
                readouts.Add(new GuiSizeAdjustor());

                LoadHelpStrings();
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets and sets the available readout modules.
        /// </summary>
        public static List<ReadoutModule> Readouts
        {
            get { return readouts; }
            set { readouts = value; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Gets a readout module with the specified name or class name. (Returns null if not found.)
        /// </summary>
        public static ReadoutModule GetReadout(string name)
        {
            return readouts.FirstOrDefault(r => r.Name == name || r.GetType().Name == name || r.Category + "." + r.GetType().Name == name);
        }

        /// <summary>
        ///     Gets a list of readout modules which are associated with the specified category.
        /// </summary>
        public static List<ReadoutModule> GetCategory(ReadoutCategory category)
        {
            return readouts.Where(r => r.Category == category).ToList();
        }

        /// <summary>
        ///     Resets all the readout modules.
        /// </summary>
        public static void Reset()
        {
            foreach (var readout in readouts)
            {
                readout.Reset();
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Loads the help strings from file.
        /// </summary>
        private static void LoadHelpStrings()
        {
            try
            {
                var handler = SettingHandler.Load("HelpStrings.xml");
                foreach (var readout in readouts)
                {
                    readout.HelpString = handler.GetSet(readout.Category + "." + readout.GetType().Name, readout.HelpString);
                }
                handler.Save("HelpStrings.xml");
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }

        #endregion
    }
}