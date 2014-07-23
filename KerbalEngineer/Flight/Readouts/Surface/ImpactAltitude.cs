// 
//     Kerbal Engineer Redux
// 
//     Copyright (C) 2014 CYBUTEK
// 
//     This program is free software: you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     This program is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General Public License for more details.
// 
//     You should have received a copy of the GNU General Public License
//     along with this program.  If not, see <http://www.gnu.org/licenses/>.
// 

#region Using Directives

using KerbalEngineer.Extensions;

#endregion

namespace KerbalEngineer.Flight.Readouts.Surface
{
    public class ImpactAltitude : ReadoutModule
    {
        private bool showing;

        public ImpactAltitude()
        {
            this.Name = "Impact Altitude";
            this.Category = ReadoutCategory.Surface;
            this.HelpString = string.Empty;
            this.IsDefault = true;
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
                this.DrawLine(ImpactProcessor.Altitude.ToAngle());
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