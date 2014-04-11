// Project:	KerbalEngineer
// Author:	CYBUTEK
// License:	Attribution-NonCommercial-ShareAlike 3.0 Unported

#region Using Directives

using KerbalEngineer.Extensions;

#endregion

namespace KerbalEngineer.Flight.Readouts.Rendezvous
{
    public class RelativeInclination : ReadoutModule
    {
        public RelativeInclination()
        {
            this.Name = "Relative Inclination";
            this.Category = ReadoutCategory.Rendezvous;
            this.HelpString = "Shows the relative inclination between your vessel and the target object.";
        }

        public override void Draw()
        {
            if (FlightGlobals.fetch.VesselTarget != null)
            {
                this.DrawLine(Vector3d.Angle(FlightGlobals.ship_orbit.GetOrbitNormal(), FlightGlobals.ActiveVessel.targetObject.GetOrbit().GetOrbitNormal()).ToAngle());
            }
        }
    }
}