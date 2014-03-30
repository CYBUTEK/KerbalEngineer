// Project:	KerbalEngineer
// Author:	CYBUTEK
// License:	Attribution-NonCommercial-ShareAlike 3.0 Unported

namespace KerbalEngineer.Simulation
{
    public class Stage
    {
        public int Number = 0;
        public int Cost = 0;
        public int TotalCost = 0;
        public int PartCount = 0;
        public double Time = 0;
        public double TotalTime = 0;
        public double Mass = 0;
        public double TotalMass = 0;
        public double Isp = 0;
        public double Thrust = 0;
        public double ActualThrust = 0;
        public double ThrustToWeight = 0;
        public double ActualThrustToWeight = 0;
        public double DeltaV = 0;
        public double TotalDeltaV = 0;
        public double InverseTotalDeltaV = 0;
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
            str.AppendFormat("actualTWR     : {0:g6}\n", actualThrustToWeight);
            str.AppendFormat("deltaV        : {0:g6}\n", deltaV);
            str.AppendFormat("totalDeltaV   : {0:g6}\n", totalDeltaV);
            str.AppendFormat("invTotDeltaV  : {0:g6}\n", inverseTotalDeltaV);
            
            MonoBehaviour.print(str);
        }
#endif
    }
}