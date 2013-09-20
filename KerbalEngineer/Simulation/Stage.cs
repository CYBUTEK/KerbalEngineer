// Kerbal Engineer Redux
// Author:  CYBUTEK
// License: Attribution-NonCommercial-ShareAlike 3.0 Unported

using KerbalEngineer.Extensions;

namespace KerbalEngineer.Simulation
{
    public class Stage
    {
        #region Fields

        public int number = 0;
        public int cost = 0;
        public int totalCost = 0;
        public int partCount = 0;
        public double time = 0d;
        public double totalTime = 0d;
        public double mass = 0d;
        public double totalMass = 0d;
        public double isp = 0d;
        public double thrust = 0d;
        public double actualThrust = 0d;
        public double thrustToWeight = 0d;
        public double actualThrustToWeight = 0d;
        public double deltaV = 0d;
        public double totalDeltaV = 0d;
        public double inverseTotalDeltaV = 0d;

        #endregion

        #region Properties

        public string Number
        {
            get { return "S" + number; }
        }

        public string Parts
        {
            get { return partCount.ToString(); }
        }

        public string Cost
        {
            get { return cost + " / " + totalCost; }
        }

        public string Mass
        {
            get { return mass.ToMass(false) + " / " + totalMass.ToMass(); }
        }

        public string Isp
        {
            get { return isp + "s"; }
        }

        public string Thrust
        {
            get { return thrust.ToForce(); }
        }

        public string TWR
        {
            get
            {
                if (HighLogic.LoadedSceneIsFlight)
                    return actualThrustToWeight.ToString("0.00");
                else
                    return thrustToWeight.ToString("0.00");
            }
        }

        public string DeltaV
        {
            get
            {
                if (HighLogic.LoadedSceneIsFlight)
                    return deltaV.ToString("#,0.") + " m/s";
                else
                    return deltaV.ToString("#,0.") + " / " + inverseTotalDeltaV.ToString("#,0.") + " m/s";
            }
        }

        public string Time
        {
            get
            {
                return time.ToTime();
            }
        }

        #endregion

        #region Initialisation

        public Stage() { }

        public Stage(int stageNumber)
        {
            number = stageNumber;
        }

        #endregion
    }
}
