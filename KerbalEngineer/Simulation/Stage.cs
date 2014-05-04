// Kerbal Engineer Redux
// Author:  CYBUTEK
// License: Attribution-NonCommercial-ShareAlike 3.0 Unported

namespace KerbalEngineer.Simulation
{
    public class Stage
    {
        public int number = 0;
        public int cost = 0;
        public int totalCost = 0;
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
#if LOG
        public void Dump()
        {
            StringBuilder str = new StringBuilder("", 512);
            str.AppendFormat("number        : {0:d}\n", number);
            str.AppendFormat("cost          : {0:d}\n", cost);
            str.AppendFormat("totalCost     : {0:d}\n", totalCost);
            str.AppendFormat("time          : {0:g6}\n", time);
            str.AppendFormat("totalTime     : {0:g6}\n", totalTime);
            str.AppendFormat("mass          : {0:g6}\n", mass);
            str.AppendFormat("totalMass     : {0:g6}\n", totalMass);
            str.AppendFormat("isp           : {0:g6}\n", isp);
            str.AppendFormat("thrust        : {0:g6}\n", thrust);
            str.AppendFormat("actualThrust  : {0:g6}\n", actualThrust);
            str.AppendFormat("thrustToWeight: {0:g6}\n", thrustToWeight);
            str.AppendFormat("maxTWR        : {0:g6}\n", maxThrustToWeight);
            str.AppendFormat("actualTWR     : {0:g6}\n", actualThrustToWeight);
            str.AppendFormat("deltaV        : {0:g6}\n", deltaV);
            str.AppendFormat("totalDeltaV   : {0:g6}\n", totalDeltaV);
            str.AppendFormat("invTotDeltaV  : {0:g6}\n", inverseTotalDeltaV);
            
            MonoBehaviour.print(str);
        }
#endif
    }
}
