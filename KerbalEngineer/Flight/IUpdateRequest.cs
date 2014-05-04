// Project:	KerbalEngineer
// Author:	CYBUTEK
// License:	Attribution-NonCommercial-ShareAlike 3.0 Unported

namespace KerbalEngineer.Flight
{
    /// <summary>
    ///     Interface which enables requested updates on an updatable object.
    /// </summary>
    public interface IUpdateRequest
    {
        bool UpdateRequested { get; set; }
    }
}