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
        public double actualThrust = 0.0;
        public double actualThrustToWeight = 0.0;
        public double cost = 0.0;
        public double deltaV = 0.0;
        public double inverseTotalDeltaV = 0.0;
        public double isp = 0.0;
        public double mass = 0.0;
        public double rcsMass = 0.0;
        public double maxThrustToWeight = 0.0;
        public int number = 0;
        public double thrust = 0.0;
        public double thrustToWeight = 0.0;
        public double time = 0.0;
        public double totalCost = 0.0;
        public double totalDeltaV = 0.0;
        public double totalMass = 0.0;
        public double totalTime = 0.0;
        public int totalPartCount = 0;
        public int partCount = 0;
        public double resourceMass = 0.0;
        public double maxThrustTorque = 0.0;
        public double thrustOffsetAngle = 0.0;
        public float maxMach = 0.0f;

        //RCS
        public double RCSIsp = 0.0;
        public double RCSThrust = 0.0;
        public double RCSdeltaVStart = 0.0;
        public double RCSTWRStart= 0.0;
        public double RCSdeltaVEnd = 0.0;
        public double RCSTWREnd = 0.0;
        public double RCSBurnTime = 0.0;

        public void Dump(LogMsg log)
        {
            log.buf.AppendFormat("number        : {0:d}\n", this.number);
            log.buf.AppendFormat("cost          : {0:g6}\n", this.cost);
            log.buf.AppendFormat("totalCost     : {0:g6}\n", this.totalCost);
            log.buf.AppendFormat("time          : {0:g6}\n", this.time);
            log.buf.AppendFormat("totalTime     : {0:g6}\n", this.totalTime);
            log.buf.AppendFormat("mass          : {0:g6}\n", this.mass);
            log.buf.AppendFormat("totalMass     : {0:g6}\n", this.totalMass);
            log.buf.AppendFormat("isp           : {0:g6}\n", this.isp);
            log.buf.AppendFormat("thrust        : {0:g6}\n", this.thrust);
            log.buf.AppendFormat("actualThrust  : {0:g6}\n", this.actualThrust);
            log.buf.AppendFormat("thrustToWeight: {0:g6}\n", this.thrustToWeight);
            log.buf.AppendFormat("maxTWR        : {0:g6}\n", this.maxThrustToWeight);
            log.buf.AppendFormat("actualTWR     : {0:g6}\n", this.actualThrustToWeight);
            log.buf.AppendFormat("ThrustTorque  : {0:g6}\n", this.maxThrustTorque);
            log.buf.AppendFormat("ThrustOffset  : {0:g6}\n", this.thrustOffsetAngle);
            log.buf.AppendFormat("deltaV        : {0:g6}\n", this.deltaV);
            log.buf.AppendFormat("totalDeltaV   : {0:g6}\n", this.totalDeltaV);
            log.buf.AppendFormat("invTotDeltaV  : {0:g6}\n", this.inverseTotalDeltaV);
            log.buf.AppendFormat("RCSdeltaVStart        : {0:g6}\n", this.RCSdeltaVStart);
            log.buf.AppendFormat("RCSIsp   : {0:g6}\n", this.RCSIsp);
            log.buf.AppendFormat("RCSThrust  : {0:g6}\n", this.RCSThrust);
            log.buf.AppendFormat("RCSTWRStart        : {0:g6}\n", this.RCSTWRStart);
            log.buf.AppendFormat("RCSdeltaVEnd   : {0:g6}\n", this.RCSdeltaVEnd);
            log.buf.AppendFormat("RCSTWREnd  : {0:g6}\n", this.RCSTWREnd);
            log.Flush();
        }
    }
}