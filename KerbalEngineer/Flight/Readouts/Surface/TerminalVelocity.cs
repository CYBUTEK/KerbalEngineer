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
    public class TerminalVelocity : ReadoutModule
    {
        private bool showing;

        public TerminalVelocity()
        {
            this.Name = "Terminal Velocity";
            this.Category = ReadoutCategory.GetCategory("Surface");
            this.HelpString = "Shows the velocity where the efforts of thrust and drag are equalled out.";
            this.IsDefault = true;
        }

        public override void Update()
        {
            AtmosphericProcessor.RequestUpdate();
        }

        public override void Draw()
        {
            var tempShowing = this.showing;
            this.showing = false;

            if (FlightGlobals.ActiveVessel.atmDensity > 0)
            {
                this.showing = true;
                this.DrawLine(AtmosphericProcessor.TerminalVelocity.ToSpeed());
            }

            if (this.showing != tempShowing)
            {
                this.ResizeRequested = true;
            }
        }

        public override void Reset()
        {
            FlightEngineerCore.Instance.AddUpdatable(AtmosphericProcessor.Instance);
        }
    }
}