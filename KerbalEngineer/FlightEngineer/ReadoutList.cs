// Project:	KerbalEngineer
// Author:	CYBUTEK
// License:	Attribution-NonCommercial-ShareAlike 3.0 Unported

#region Using Directives

using System.Collections.Generic;
using System.Linq;

using KerbalEngineer.FlightEngineer.Orbital;
using KerbalEngineer.FlightEngineer.Rendezvous;
using KerbalEngineer.FlightEngineer.Surface;
using KerbalEngineer.FlightEngineer.Vessel;

#endregion

namespace KerbalEngineer.FlightEngineer
{
    public class ReadoutList
    {
        #region Instance

        private static ReadoutList _instance;

        /// <summary>
        ///     Gets the current instance of the readout list.
        /// </summary>
        public static ReadoutList Instance
        {
            get { return _instance ?? (_instance = new ReadoutList()); }
        }

        #endregion

        #region Properties

        private List<Readout> readouts = new List<Readout>();

        /// <summary>
        ///     Gets and sets the available readouts.
        /// </summary>
        public List<Readout> Readouts
        {
            get { return this.readouts; }
            set { this.readouts = value; }
        }

        #endregion

        #region Initialisation

        private ReadoutList()
        {
            this.readouts.Add(new ApoapsisHeight());
            this.readouts.Add(new PeriapsisHeight());
            this.readouts.Add(new TimeToApoapsis());
            this.readouts.Add(new TimeToPeriapsis());
            this.readouts.Add(new Inclination());
            this.readouts.Add(new Eccentricity());
            this.readouts.Add(new OrbitalPeriod());
            this.readouts.Add(new LongitudeOfAN());
            this.readouts.Add(new LongitudeOfPe());
            this.readouts.Add(new SemiMajorAxis());
            this.readouts.Add(new SemiMinorAxis());

            this.readouts.Add(new AltitudeSeaLevel());
            this.readouts.Add(new AltitudeTerrain());
            this.readouts.Add(new VerticalSpeed());
            this.readouts.Add(new HorizontalSpeed());
            this.readouts.Add(new Longitude());
            this.readouts.Add(new Latitude());
            this.readouts.Add(new TerminalVelocity());
            this.readouts.Add(new AtmosEfficiency());

            this.readouts.Add(new DeltaVStaged());
            this.readouts.Add(new DeltaVTotal());
            this.readouts.Add(new SpecificImpulse());
            this.readouts.Add(new TotalMass());
            this.readouts.Add(new ThrustTotal());
            this.readouts.Add(new ThrustActual());
            this.readouts.Add(new ThrustToWeight());

            this.readouts.Add(new TargetSelector());
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Gets a readout matching the name provided.
        /// </summary>
        public Readout GetReadout(string name)
        {
            return this.readouts.FirstOrDefault(readout => readout.Name == name);
        }

        /// <summary>
        ///     Gets a list of readouts based on the category provided.
        /// </summary>
        public List<Readout> GetCategory(ReadoutCategory category)
        {
            return this.readouts.Where(readout => readout.Category == category).ToList();
        }

        #endregion
    }
}