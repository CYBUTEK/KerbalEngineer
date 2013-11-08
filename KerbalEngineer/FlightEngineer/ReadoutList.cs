// Name:    Kerbal Engineer Redux
// Author:  CYBUTEK
// License: Attribution-NonCommercial-ShareAlike 3.0 Unported

using System.Collections.Generic;

namespace KerbalEngineer.FlightEngineer
{
    public class ReadoutList
    {
        #region Instance

        private static ReadoutList _instance;
        /// <summary>
        /// Gets the current instance of the readout list.
        /// </summary>
        public static ReadoutList Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new ReadoutList();

                return _instance;
            }
        }

        #endregion

        #region Properties

        private List<Readout> _readouts = new List<Readout>();
        /// <summary>
        /// Gets and sets the available readouts.
        /// </summary>
        public List<Readout> Readouts
        {
            get { return _readouts; }
            set { _readouts = value; }
        }

        #endregion

        #region Initialisation

        private ReadoutList()
        {
            _readouts.Add(new Orbital.ApoapsisHeight());
            _readouts.Add(new Orbital.PeriapsisHeight());
            _readouts.Add(new Orbital.TimeToApoapsis());
            _readouts.Add(new Orbital.TimeToPeriapsis());
            _readouts.Add(new Orbital.Inclination());
            _readouts.Add(new Orbital.Eccentricity());
            _readouts.Add(new Orbital.OrbitalPeriod());
            _readouts.Add(new Orbital.LongitudeOfAN());
            _readouts.Add(new Orbital.LongitudeOfPe());
            _readouts.Add(new Orbital.SemiMajorAxis());
            _readouts.Add(new Orbital.SemiMinorAxis());

            _readouts.Add(new Surface.AltitudeSeaLevel());
            _readouts.Add(new Surface.AltitudeTerrain());
            _readouts.Add(new Surface.VerticalSpeed());
            _readouts.Add(new Surface.HorizontalSpeed());
            _readouts.Add(new Surface.Longitude());
            _readouts.Add(new Surface.Latitude());
            _readouts.Add(new Surface.TerminalVelocity());
            _readouts.Add(new Surface.AtmosEfficiency());

            _readouts.Add(new Vessel.DeltaVStaged());
            _readouts.Add(new Vessel.DeltaVTotal());
            _readouts.Add(new Vessel.SpecificImpulse());
            _readouts.Add(new Vessel.TotalMass());
            _readouts.Add(new Vessel.ThrustTotal());
            _readouts.Add(new Vessel.ThrustActual());
            _readouts.Add(new Vessel.ThrustToWeight());

            _readouts.Add(new Rendezvous.TargetSelector());
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets a readout matching the name provided.
        /// </summary>
        public Readout GetReadout(string name)
        {
            foreach (Readout readout in _readouts)
                if (readout.Name == name)
                    return readout;

            return null;
        }

        /// <summary>
        /// Gets a list of readouts based on the category provided.
        /// </summary>
        public List<Readout> GetCategory(ReadoutCategory category)
        {
            List<Readout> readouts = new List<Readout>();

            foreach (Readout readout in _readouts)
                if (readout.Category == category)
                    readouts.Add(readout);

            return readouts;
        }

        #endregion
    }
}
