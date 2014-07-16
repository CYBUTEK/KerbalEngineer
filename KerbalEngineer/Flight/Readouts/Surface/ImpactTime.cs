// Project:	KerbalEngineer
// Author:	CYBUTEK
// License:	Attribution-NonCommercial-ShareAlike 3.0 Unported

using KerbalEngineer.Extensions;

namespace KerbalEngineer.Flight.Readouts.Surface
{
    public class ImpactTime : ReadoutModule
    {
        private bool showing;

        public ImpactTime()
        {
            this.Name = "Impact Time";
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
                this.DrawLine(ImpactProcessor.Time.ToTime());
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