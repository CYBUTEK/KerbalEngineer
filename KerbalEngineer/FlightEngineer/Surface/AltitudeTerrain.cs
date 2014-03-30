// Project:	KerbalEngineer
// Author:	CYBUTEK
// License:	Attribution-NonCommercial-ShareAlike 3.0 Unported

#region Using Directives

using KerbalEngineer.Extensions;

#endregion

namespace KerbalEngineer.FlightEngineer.Surface
{
    public class AltitudeTerrain : Readout
    {
        protected override void Initialise()
        {
            this.Name = "Altitude (Terrain)";
            this.Description = "Shows your altitude above the terrain.";
            this.Category = ReadoutCategory.Surface;
        }

        public override void Draw()
        {
            this.DrawLine((FlightGlobals.ActiveVessel.altitude - FlightGlobals.ActiveVessel.terrainAltitude).ToDistance());
        }
    }
}