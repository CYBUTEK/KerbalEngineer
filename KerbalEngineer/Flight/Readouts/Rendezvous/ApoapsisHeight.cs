// Project:	KerbalEngineer
// Author:	CYBUTEK
// License:	Attribution-NonCommercial-ShareAlike 3.0 Unported

#region Using Directives

using KerbalEngineer.Extensions;

#endregion

namespace KerbalEngineer.Flight.Readouts.Rendezvous
{
    public class ApoapsisHeight : ReadoutModule
    {
        public ApoapsisHeight()
        {
            this.Name = "Apoapsis Height";
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

            this.DrawLine(RendezvousProcessor.ApoapsisHeight.ToAngle());
        }

        public override void Reset()
        {
            FlightEngineerCore.Instance.AddUpdatable(RendezvousProcessor.Instance);
        }
    }
}