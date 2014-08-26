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

using System.Text;

using UnityEngine;

#endregion

namespace KerbalEngineer.VesselSimulator
{
    public class Stage
    {
        public double actualThrust = 0f;
        public double actualThrustToWeight = 0f;
        public float cost = 0;
        public double deltaV = 0f;
        public double inverseTotalDeltaV = 0f;
        public double isp = 0f;
        public double mass = 0f;
        public double maxThrustToWeight = 0f;
        public int number = 0;
        public double thrust = 0f;
        public double thrustToWeight = 0f;
        public double time = 0f;
        public float totalCost = 0;
        public double totalDeltaV = 0f;
        public double totalMass = 0f;
        public double totalTime = 0f;
        public int partCount = 0;

        public void Dump()
        {
            StringBuilder str = new StringBuilder("", 512);
            str.AppendFormat("number        : {0:d}\n", this.number);
            str.AppendFormat("cost          : {0:g6}\n", this.cost);
            str.AppendFormat("totalCost     : {0:g6}\n", this.totalCost);
            str.AppendFormat("time          : {0:g6}\n", this.time);
            str.AppendFormat("totalTime     : {0:g6}\n", this.totalTime);
            str.AppendFormat("mass          : {0:g6}\n", this.mass);
            str.AppendFormat("totalMass     : {0:g6}\n", this.totalMass);
            str.AppendFormat("isp           : {0:g6}\n", this.isp);
            str.AppendFormat("thrust        : {0:g6}\n", this.thrust);
            str.AppendFormat("actualThrust  : {0:g6}\n", this.actualThrust);
            str.AppendFormat("thrustToWeight: {0:g6}\n", this.thrustToWeight);
            str.AppendFormat("maxTWR        : {0:g6}\n", this.maxThrustToWeight);
            str.AppendFormat("actualTWR     : {0:g6}\n", this.actualThrustToWeight);
            str.AppendFormat("deltaV        : {0:g6}\n", this.deltaV);
            str.AppendFormat("totalDeltaV   : {0:g6}\n", this.totalDeltaV);
            str.AppendFormat("invTotDeltaV  : {0:g6}\n", this.inverseTotalDeltaV);

            MonoBehaviour.print(str);
        }
    }
}