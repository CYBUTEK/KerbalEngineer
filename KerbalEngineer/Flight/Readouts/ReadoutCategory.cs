// Project:	KerbalEngineer
// Author:	CYBUTEK
// License:	Attribution-NonCommercial-ShareAlike 3.0 Unported

namespace KerbalEngineer.Flight.Readouts
{
    /// <summary>
    ///     Enumeration of categories that a readout module can associate with.
    /// </summary>
    public enum ReadoutCategory
    {
        None = 0,
        Orbital = 1,
        Surface = 2,
        Vessel = 4,
        Rendezvous = 8,
        Misc = 16
    }
}