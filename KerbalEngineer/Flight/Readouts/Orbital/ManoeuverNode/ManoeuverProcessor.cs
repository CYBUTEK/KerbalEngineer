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

using KerbalEngineer.Extensions;

#endregion

namespace KerbalEngineer.Flight.Readouts.Orbital.ManoeuverNode
{
    public class ManoeuverProcessor : IUpdatable, IUpdateRequest
    {
        #region Fields

        private static readonly ManoeuverProcessor instance = new ManoeuverProcessor();

        #endregion

        #region Properties

        public static double AngleToPrograde { get; private set; }

        public static double AngleToRetrograde { get; private set; }

        public static ManoeuverProcessor Instance
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
        }

        public void Update()
        {
            if (FlightGlobals.ActiveVessel.patchedConicSolver.maneuverNodes.Count == 0)
            {
                ShowDetails = false;
                return;
            }

            var node = FlightGlobals.ActiveVessel.patchedConicSolver.maneuverNodes[0].DeltaV;

            ProgradeDeltaV = node.z;
            NormalDeltaV = node.y;
            RadialDeltaV = node.x;
            TotalDeltaV = node.magnitude;

            UniversalTime = FlightGlobals.ActiveVessel.patchedConicSolver.maneuverNodes[0].UT;
            AngleToPrograde = FlightGlobals.ActiveVessel.patchedConicSolver.maneuverNodes[0].patch.GetAngleToPrograde(UniversalTime);
            AngleToRetrograde = FlightGlobals.ActiveVessel.patchedConicSolver.maneuverNodes[0].patch.GetAngleToRetrograde(UniversalTime);

            ShowDetails = true;
        }

        #endregion
    }
}