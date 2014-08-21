// Kerbal Engineer Redux
// Author:  CYBUTEK
// License: Attribution-NonCommercial-ShareAlike 3.0 Unported

using System.Text;

using UnityEngine;

namespace KerbalEngineer.VesselSimulator
{
    public class Stage
    {
        public int number = 0;
        public float cost = 0;
        public float totalCost = 0;
        public double time = 0f;
        public double totalTime = 0f;
        public double mass = 0f;
        public double totalMass = 0f;
        public double isp = 0f;
        public double thrust = 0f;
        public double actualThrust = 0f;
        public double thrustToWeight = 0f;
        public double maxThrustToWeight = 0f;
        public double actualThrustToWeight = 0f;
        public double deltaV = 0f;
        public double totalDeltaV = 0f;
        public double inverseTotalDeltaV = 0f;

        public void Dump()
        {
            StringBuilder str = new StringBuilder("", 512);
            str.AppendFormat("number        : {0:d}\n", this.number);
            str.AppendFormat("cost          : {0:d}\n", this.cost);
            str.AppendFormat("totalCost     : {0:d}\n", this.totalCost);
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
