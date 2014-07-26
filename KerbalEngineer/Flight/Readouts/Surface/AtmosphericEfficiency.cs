﻿// 
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

namespace KerbalEngineer.Flight.Readouts.Surface
{
    public class AtmosphericEfficiency : ReadoutModule
    {
        private bool showing;

        public AtmosphericEfficiency()
        {
            this.Name = "Atmos. Efficiency";
            this.Category = ReadoutCategory.Surface;
            this.HelpString = "Shows you vessel's efficiency as a ratio of the current velocity and terminal velocity.  Less than 1 means that you are losing efficiency due to gravity and greater than 1 is due to drag.";
            this.IsDefault = true;
        }

        public override void Update()
        {
            AtmosphericProcessor.RequestUpdate();
        }

        public override void Draw()
        {
            if (AtmosphericProcessor.ShowDetails)
            {
                this.showing = true;
                this.DrawLine(AtmosphericProcessor.Efficiency.ToString("F2"));
            }
            else if (this.showing)
            {
                this.showing = false;
                this.ResizeRequested = true;
            }
        }

        public override void Reset()
        {
            FlightEngineerCore.Instance.AddUpdatable(AtmosphericProcessor.Instance);
        }
    }
}