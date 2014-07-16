// Project:	KerbalEngineer
// Author:	CYBUTEK
// License:	Attribution-NonCommercial-ShareAlike 3.0 Unported

#region Using Directives

using KerbalEngineer.Extensions;

#endregion

namespace KerbalEngineer.Flight.Readouts.Rendezvous
{
    public class TimeToApoapsis : ReadoutModule
    {
        public TimeToApoapsis()
        {
            this.Name = "Time to Apoapsis";
            this.Category = ReadoutCategory.Rendezvous;
            //this.HelpString = "";
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

            this.DrawLine(RendezvousProcessor.TimeToApoapsis.ToTime());
        }

        public override void Reset()
        {
            FlightEngineerCore.Instance.AddUpdatable(RendezvousProcessor.Instance);
        }
    }
}