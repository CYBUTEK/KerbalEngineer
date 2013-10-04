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
            _readouts.Add(new Orbital.TimeToApoapsis());
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
