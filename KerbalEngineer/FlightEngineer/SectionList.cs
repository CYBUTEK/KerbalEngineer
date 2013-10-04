// Name:    Kerbal Engineer Redux
// Author:  CYBUTEK
// License: Attribution-NonCommercial-ShareAlike 3.0 Unported

using System.Collections.Generic;

namespace KerbalEngineer.FlightEngineer
{
    public class SectionList
    {
        #region Instance

        private static SectionList _instance;
        /// <summary>
        /// Gets the current instance of the section list.
        /// </summary>
        public static SectionList Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new SectionList();

                return _instance;
            }
        }

        #endregion

        #region Properties

        private List<Section> _fixedSections = new List<Section>();
        /// <summary>
        /// Gets and sets the available fixed sections.
        /// </summary>
        public List<Section> FixedSections
        {
            get { return _fixedSections; }
            set { _fixedSections = value; }
        }

        private List<Section> _userSections = new List<Section>();
        /// <summary>
        /// Gets and sets the available user sections.
        /// </summary>
        public List<Section> UserSections
        {
            get { return _userSections; }
            set { _userSections = value; }
        }

        #endregion

        #region Initialisation

        private SectionList()
        {
            _fixedSections.Add(new SectionOrbital());
            _fixedSections.Add(new SectionOrbital());
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets whether there are sections that are set to be visible.
        /// </summary>
        public bool HasVisibleSections()
        {
            foreach (Section section in _fixedSections)
                if (section.Visible)
                    return true;

            foreach (Section section in _userSections)
                if (section.Visible)
                    return true;

            return false;
        }

        #endregion
    }
}
