// Project:	KerbalEngineer
// Author:	CYBUTEK
// License:	Attribution-NonCommercial-ShareAlike 3.0 Unported

#region Using Directives

using KerbalEngineer.Extensions;

#endregion

namespace KerbalEngineer.Flight.Readouts.Rendezvous
{
    public class TimeToPeriapsis : ReadoutModule
    {
        public TimeToPeriapsis()
        {
            this.Name = "Time to Periapsis";
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

            this.DrawLine(RendezvousProcessor.TimeToPeriapsis.ToTime());
        }

        public override void Reset()
        {
            FlightEngineerCore.Instance.AddUpdatable(RendezvousProcessor.Instance);
        }
    }
}