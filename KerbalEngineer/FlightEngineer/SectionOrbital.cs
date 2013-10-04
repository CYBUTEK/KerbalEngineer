// Name:    Kerbal Engineer Redux
// Author:  CYBUTEK
// License: Attribution-NonCommercial-ShareAlike 3.0 Unported

namespace KerbalEngineer.FlightEngineer
{
    public class SectionOrbital : Section
    {
        #region Instance

        private static SectionOrbital _instance;
        /// <summary>
        /// Gets the current instance of the orbital section.
        /// </summary>
        public static SectionOrbital Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new SectionOrbital();

                return _instance;
            }
        }

        #endregion

        #region Initialisation

        private SectionOrbital()
        {
            Title = "Orbital";
            Readouts.AddRange(ReadoutList.Instance.GetCategory(ReadoutCategory.Orbital));
        }

        #endregion
    }
}
