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

using KerbalEngineer.Helpers;

#endregion

namespace KerbalEngineer.Flight.Readouts.Vessel
{
    public class DeltaVStaged : ReadoutModule
    {
        #region Fields

        private int numberOfStages;
        private bool showing;

        #endregion

        #region Constructors

        public DeltaVStaged()
        {
            this.Name = "DeltaV Staged";
            this.Category = ReadoutCategory.GetCategory("Vessel");
            this.HelpString = "Shows the vessel's delta velocity for each stage.";
            this.IsDefault = true;
        }

        #endregion

        #region Methods: public

        public override void Draw()
        {
            if (SimulationProcessor.ShowDetails)
            {
                this.showing = true;
                var newNumberOfStages = 0;
                foreach (var stage in SimulationProcessor.Stages)
                {
                    if (stage.deltaV > 0 || stage.number == Staging.CurrentStage)
                    {
                        this.DrawLine("DeltaV (S" + stage.number + ")", stage.deltaV.ToString("N0") + "m/s (" + TimeFormatter.ConvertToString(stage.time) + ")");
                        newNumberOfStages++;
                    }
                }

                if (newNumberOfStages != this.numberOfStages)
                {
                    this.numberOfStages = newNumberOfStages;
                    this.ResizeRequested = true;
                }
            }
            else if (this.showing)
            {
                this.showing = false;
                this.ResizeRequested = true;
            }
        }

        public override void Reset()
        {
            FlightEngineerCore.Instance.AddUpdatable(SimulationProcessor.Instance);
        }

        public override void Update()
        {
            SimulationProcessor.RequestUpdate();
        }

        #endregion
    }
}