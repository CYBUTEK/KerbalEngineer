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

using System;

using KerbalEngineer.Extensions;
using KerbalEngineer.Flight.Readouts.Vessel;

using UnityEngine;

#endregion

namespace KerbalEngineer.Flight.Readouts.Orbital.ManoeuvreNode
{
    public class ManoeuvreProcessor : IUpdatable, IUpdateRequest
    {
        #region Fields

        private static readonly ManoeuvreProcessor instance = new ManoeuvreProcessor();

        #endregion

        #region Properties

        public static double AngleToPrograde { get; private set; }

        public static double AngleToRetrograde { get; private set; }

        public static double AvailableDeltaV { get; private set; }

        public static double BurnTime { get; private set; }
        public static int FinalStage { get; private set; }

        public static double HalfBurnTime { get; private set; }

        public static bool HasDeltaV { get; private set; }

        public static ManoeuvreProcessor Instance
        {
            get { return instance; }
        }

        public static double NormalDeltaV { get; private set; }

        public static double ProgradeDeltaV { get; private set; }

        public static double RadialDeltaV { get; private set; }

        public static bool ShowDetails { get; set; }

        public static double TotalDeltaV { get; private set; }

        public static double UniversalTime { get; private set; }

        public bool UpdateRequested { get; set; }

        #endregion

        #region Methods: public

        public static void RequestUpdate()
        {
            instance.UpdateRequested = true;
            SimulationProcessor.RequestUpdate();
        }

        public static void Reset()
        {
            FlightEngineerCore.Instance.AddUpdatable(SimulationProcessor.Instance);
            FlightEngineerCore.Instance.AddUpdatable(instance);
        }

        public void Update()
        {
            if (FlightGlobals.ActiveVessel.patchedConicSolver.maneuverNodes.Count == 0 || !SimulationProcessor.ShowDetails)
            {
                ShowDetails = false;
                return;
            }

            var node = FlightGlobals.ActiveVessel.patchedConicSolver.maneuverNodes[0];
            var deltaV = node.DeltaV;

            ProgradeDeltaV = deltaV.z;
            NormalDeltaV = deltaV.y;
            RadialDeltaV = deltaV.x;
            TotalDeltaV = node.GetBurnVector(FlightGlobals.ship_orbit).magnitude;

            UniversalTime = FlightGlobals.ActiveVessel.patchedConicSolver.maneuverNodes[0].UT;
            AngleToPrograde = FlightGlobals.ActiveVessel.patchedConicSolver.maneuverNodes[0].patch.GetAngleToPrograde(UniversalTime);
            AngleToRetrograde = FlightGlobals.ActiveVessel.patchedConicSolver.maneuverNodes[0].patch.GetAngleToRetrograde(UniversalTime);

            var burnTime = 0.0;
            var midPointTime = 0.0;
            HasDeltaV = GetBurnTime((float)TotalDeltaV, ref burnTime, ref midPointTime);
            AvailableDeltaV = SimulationProcessor.LastStage.totalDeltaV;

            BurnTime = burnTime;
            HalfBurnTime = midPointTime;

            ShowDetails = true;
        }

        #endregion

        #region Methods: private

        private static bool GetBurnTime(float deltaV, ref double burnTime, ref double midPointTime)
        {
            var setMidPoint = false;
            var deltaVMidPoint = deltaV * 0.5f;

            for (var i = SimulationProcessor.Stages.Length - 1; i >= 0; i--)
            {
                var stage = SimulationProcessor.Stages[i];
                var stageDeltaV = (float)stage.deltaV;

                ProcessStageDrain:
                if (deltaV <= Single.Epsilon)
                {
                    FinalStage = ++i;
                    return true;
                }
                if (stageDeltaV <= Single.Epsilon)
                {
                    continue;
                }

                float deltaVDrain;
                if (deltaVMidPoint > 0.0)
                {
                    deltaVDrain = Mathf.Clamp(deltaV, 0.0f, Mathf.Clamp(deltaVMidPoint, 0.0f, stageDeltaV));
                    deltaVMidPoint -= deltaVDrain;
                    setMidPoint = deltaVMidPoint <= Single.Epsilon;
                }
                else
                {
                    deltaVDrain = Mathf.Clamp(deltaV, 0.0f, stageDeltaV);
                }

                var startMass = stage.totalMass - (stage.resourceMass * (1.0f - (stageDeltaV / stage.deltaV)));
                var endMass = startMass - (stage.resourceMass * (deltaVDrain / stageDeltaV));
                var minimumAcceleration = stage.thrust / startMass;
                var maximumAcceleration = stage.thrust / endMass;

                burnTime += deltaVDrain / ((minimumAcceleration + maximumAcceleration) * 0.5);
                deltaV -= deltaVDrain;
                stageDeltaV -= deltaVDrain;

                if (setMidPoint)
                {
                    midPointTime = burnTime;
                    setMidPoint = false;
                    goto ProcessStageDrain;
                }
            }
            return false;
        }

        #endregion
    }
}