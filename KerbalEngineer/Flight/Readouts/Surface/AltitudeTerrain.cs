// Project:	KerbalEngineer
// Author:	CYBUTEK
// License:	Attribution-NonCommercial-ShareAlike 3.0 Unported

#region Using Directives

using KerbalEngineer.Extensions;

#endregion

namespace KerbalEngineer.Flight.Readouts.Surface
{
    public class AltitudeTerrain : ReadoutModule
    {
        public AltitudeTerrain()
        {
            this.Name = "Altitude (Terrain)";
            this.Category = ReadoutCategory.Surface;
            this.HelpString = "Shows the vessel's altitude above the terrain.";
        }

        public override void Draw()
        {
            this.DrawLine(FlightGlobals.ActiveVessel.terrainAltitude.ToDistance());
        }
    }
}