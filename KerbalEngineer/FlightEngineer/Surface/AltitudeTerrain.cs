// Name:    Kerbal Engineer Redux
// Author:  CYBUTEK
// License: Attribution-NonCommercial-ShareAlike 3.0 Unported

using KerbalEngineer.Extensions;
using UnityEngine;

namespace KerbalEngineer.FlightEngineer.Surface
{
    public class AltitudeTerrain : Readout
    {
        protected override void Initialise()
        {
            Name = "Altitude (Terrain)";
            Description = "Shows your altitude above the terrain.";
            Category = ReadoutCategory.Surface;
        }

        public override void Draw()
        {
            DrawLine((FlightGlobals.ActiveVessel.altitude - FlightGlobals.ActiveVessel.terrainAltitude).ToDistance());
        }
    }
}
