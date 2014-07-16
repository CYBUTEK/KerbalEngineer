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
            this.IsDefault = true;
        }

        public override void Update()
        {
            RendezvousProcessor.RequestUpdate();
        }

        public override void Draw()
        {
            if (!RendezvousProcessor.ShowDetails)
            {
                return;
            }

            this.DrawLine(RendezvousProcessor.RelativeInclination.ToAngle());
        }

        public override void Reset()
        {
            FlightEngineerCore.Instance.AddUpdatable(RendezvousProcessor.Instance);
        }
    }
}