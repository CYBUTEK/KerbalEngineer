// Project:	KerbalEngineer
// Author:	CYBUTEK
// License:	Attribution-NonCommercial-ShareAlike 3.0 Unported

#region Using Directives

using System.Collections.Generic;
using System.Linq;

using KerbalEngineer.Flight.Readouts.Orbital;
using KerbalEngineer.Flight.Readouts.Rendezvous;
using KerbalEngineer.Flight.Readouts.Surface;
using KerbalEngineer.Flight.Readouts.Vessel;
using KerbalEngineer.Settings;

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
            this.readoutModules.Add(new Orbital.ApoapsisHeight());
            this.readoutModules.Add(new Orbital.PeriapsisHeight());
            this.readoutModules.Add(new Orbital.TimeToApoapsis());
            this.readoutModules.Add(new Orbital.TimeToPeriapsis());
            this.readoutModules.Add(new Inclination());
            this.readoutModules.Add(new Eccentricity());
            this.readoutModules.Add(new OrbitalSpeed());
            this.readoutModules.Add(new Orbital.OrbitalPeriod());
            this.readoutModules.Add(new LongitudeOfAscendingNode());
            this.readoutModules.Add(new LongitudeOfPeriapsis());
            this.readoutModules.Add(new SemiMajorAxis());
            this.readoutModules.Add(new SemiMinorAxis());

            // Surface
            this.readoutModules.Add(new Surface.AltitudeSeaLevel());
            this.readoutModules.Add(new AltitudeTerrain());
            this.readoutModules.Add(new VerticalSpeed());
            this.readoutModules.Add(new HorizontalSpeed());
            this.readoutModules.Add(new Longitude());
            this.readoutModules.Add(new Latitude());
            this.readoutModules.Add(new GeeForce());
            this.readoutModules.Add(new TerminalVelocity());
            this.readoutModules.Add(new AtmosphericEfficiency());
            this.readoutModules.Add(new ImpactTime());
            this.readoutModules.Add(new ImpactLongitude());
            this.readoutModules.Add(new ImpactLatitude());
            this.readoutModules.Add(new ImpactAltitude());

            // Vessel
            this.readoutModules.Add(new DeltaVStaged());
            this.readoutModules.Add(new DeltaVTotal());
            this.readoutModules.Add(new SpecificImpulse());
            this.readoutModules.Add(new Mass());
            this.readoutModules.Add(new Thrust());
            this.readoutModules.Add(new ThrustToWeight());

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