// Project:	KerbalEngineer
// Author:	CYBUTEK
// License:	Attribution-NonCommercial-ShareAlike 3.0 Unported

using KerbalEngineer.Extensions;

namespace KerbalEngineer.Flight.Readouts.Surface
{
    public class ImpactLongitude : ReadoutModule
    {
        private bool showing;

        public ImpactLongitude()
        {
            this.Name = "Impact Longitude";
            this.Category = ReadoutCategory.Surface;
            //this.HelpString = "";
        }

        public override void Update()
        {
            ImpactProcessor.RequestUpdate();
        }

        public override void Draw()
        {
            if (ImpactProcessor.ShowDetails)
            {
                this.showing = true;
                this.DrawLine(ImpactProcessor.Longitude.ToAngle());
            }
            else if (this.showing)
            {
                this.showing = false;
                this.ResizeRequested = true;
            }
        }

        public override void Reset()
        {
            FlightEngineerCore.Instance.AddUpdatable(ImpactProcessor.Instance);
        }
    }
}