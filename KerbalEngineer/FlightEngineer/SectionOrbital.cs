// Name:    Kerbal Engineer Redux
// Author:  CYBUTEK
// License: Attribution-NonCommercial-ShareAlike 3.0 Unported

namespace KerbalEngineer.FlightEngineer
{
    public class SectionOrbital : Section
    {
        #region Initialisation

        public SectionOrbital()
        {
            Title = "Orbital";
            Readouts.AddRange(ReadoutList.Instance.GetCategory(ReadoutCategory.Orbital));
        }

        #endregion
    }
}
