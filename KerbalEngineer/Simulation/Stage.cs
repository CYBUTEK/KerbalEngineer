// Project:	KerbalEngineer
// Author:	CYBUTEK
// License:	Attribution-NonCommercial-ShareAlike 3.0 Unported

#region Using Directives

using KerbalEngineer.Extensions;

#endregion

namespace KerbalEngineer.Simulation
{
    public class Stage
    {
        #region Fields

        public double actualThrust = 0;
        public double actualThrustToWeight = 0;
        public int cost = 0;
        public double deltaV = 0;
        public double inverseTotalDeltaV = 0;
        public double isp = 0;
        public double mass = 0;
        public int number = 0;
        public int partCount = 0;
        public double thrust = 0;
        public double thrustToWeight = 0;
        public double time = 0;
        public int totalCost = 0;
        public double totalDeltaV = 0;
        public double totalMass = 0;
        public double totalTime = 0;

        #endregion

        #region Properties

        public string Number
        {
            get { return "S" + this.number; }
        }

        public string Parts
        {
            get { return this.partCount.ToString(); }
        }

        public string Cost
        {
            get { return this.cost + " / " + this.totalCost; }
        }

        public string Mass
        {
            get
            {
                if (HighLogic.LoadedSceneIsFlight)
                {
                    return this.totalMass.ToMass();
                }
                return this.mass.ToMass(false) + " / " + this.totalMass.ToMass();
            }
        }

        public string Isp
        {
            get { return this.isp.ToString("#,0.00") + "s"; }
        }

        public string Thrust
        {
            get { return this.thrust.ToForce(); }
        }

        public string ActualThrust
        {
            get { return this.actualThrust.ToForce(); }
        }

        public string TWR
        {
            get
            {
                if (HighLogic.LoadedSceneIsFlight)
                {
                    return this.actualThrustToWeight.ToString("0.00") + " / " + this.thrustToWeight.ToString("0.00");
                }
                return this.thrustToWeight.ToString("0.00");
            }
        }

        public string DeltaV
        {
            get
            {
                if (HighLogic.LoadedSceneIsFlight)
                {
                    return this.deltaV.ToSpeed();
                }
                return this.deltaV.ToString("#,0.") + " / " + this.inverseTotalDeltaV.ToString("#,0.") + "m/s";
            }
        }

        public string TotalDeltaV
        {
            get
            {
                if (HighLogic.LoadedSceneIsFlight)
                {
                    return this.totalDeltaV.ToSpeed();
                }
                return this.inverseTotalDeltaV.ToString("#,0.") + "m/s";
            }
        }

        public string Time
        {
            get { return this.time.ToTime(); }
        }

        #endregion

        #region Initialisation

        public Stage() { }

        public Stage(int stageNumber)
        {
            this.number = stageNumber;
        }

        #endregion
    }
}