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

namespace KerbalEngineer.Flight.Readouts.Vessel
{
	using System;

	public class DecellerationProcessor : IUpdatable, IUpdateRequest // Shamelessly Stolen From Maneuver Node Processor
	{
		#region Properties

		private static readonly DecellerationProcessor instance = new DecellerationProcessor();

		public static double AvailableDeltaV { get; private set; }

		public static double DecellerationTime { get; private set; }

		public static int FinalStage { get; private set; }

		public static bool HasDeltaV { get; private set; }

		private static double ProgradeDeltaV;
		private static double RadialDeltaV;

		private double m_Gravity;
		private double m_RadarAltitude;

		public static bool ShowDetails { get; set; }
		public static double DecellerationDeltaV { get; private set; }

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
			if (FlightGlobals.currentMainBody == null || FlightGlobals.ActiveVessel == null || SimulationProcessor.LastStage == null || !SimulationProcessor.ShowDetails)
			{
				ShowDetails = false;
				return;
			}

			m_Gravity = FlightGlobals.currentMainBody.gravParameter / Math.Pow(FlightGlobals.currentMainBody.Radius, 2.0);
			m_RadarAltitude = FlightGlobals.ActiveVessel.terrainAltitude > 0.0
				? FlightGlobals.ship_altitude - FlightGlobals.ActiveVessel.terrainAltitude
				: FlightGlobals.ship_altitude;
			ProgradeDeltaV = FlightGlobals.ActiveVessel.horizontalSrfSpeed;
			RadialDeltaV = Math.Sqrt((2 * m_Gravity * m_RadarAltitude) + Math.Pow(FlightGlobals.ship_verticalSpeed, 2.0));
			DecellerationDeltaV = Math.Sqrt(Math.Exp(ProgradeDeltaV, 2.0)+Math.Pow(RadialDeltaV, 2.0));
			HasDeltaV = GetSuicideBurnTime(DecellerationDeltaV, ref DecellerationTime);
		}

		#endregion

		#region Methods: private

		private static bool GetSuicideBurnTime(double deltaV, ref double burnTime)
		{
			for (var i = SimulationProcessor.Stages.Length - 1; i > -1; i--)
			{
				var stage = SimulationProcessor.Stages[i];
				var stageDeltaV = stage.deltaV;
				var startMass = stage.totalMass;

				ProcessStageDrain:
				if (deltaV <= Double.Epsilon)
				{
					break;
				}
				if (stageDeltaV <= Double.Epsilon)
				{
					continue;
				}

				FinalStage = i;

				double deltaVDrain = deltaV.Clamp(0.0, stageDeltaV);

				var exhaustVelocity = stage.isp * Units.GRAVITY;
				var flowRate = stage.thrust / exhaustVelocity;
				var endMass = Math.Exp(Math.Log(startMass) - deltaVDrain / exhaustVelocity);
				var deltaMass = (startMass - endMass) * Math.Exp(-(deltaVDrain * 0.001) / exhaustVelocity);
				burnTime += deltaMass / flowRate;

				deltaV -= deltaVDrain;
				stageDeltaV -= deltaVDrain;
				startMass -= deltaMass;
			}
			return deltaV <= Double.Epsilon;
		}

		#endregion
	}
}