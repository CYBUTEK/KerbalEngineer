// 
//     Copyright (C) 2015 CYBUTEK
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

namespace KerbalEngineer.Flight.Readouts.Thermal
{
    using System;

    public class ThermalProcessor : IUpdatable, IUpdateRequest
    {
        private static readonly ThermalProcessor instance = new ThermalProcessor();

        static ThermalProcessor()
        {
            HottestTemperature = 0.0;
            HottestTemperatureMax = 0.0;
            HottestSkinTemperature = 0.0;
            HottestSkinTemperatureMax = 0.0;
            CoolestTemperature = 0.0;
            CoolestTemperatureMax = 0.0;
            CoolestSkinTemperature = 0.0;
            CoolestSkinTemperatureMax = 0.0;
            CriticalTemperature = 0.0;
            CriticalTemperatureMax = 0.0;
            CriticalSkinTemperature = 0.0;
            CriticalSkinTemperatureMax = 0.0;
            HottestPartName = string.Empty;
            CoolestPartName = string.Empty;
            CriticalPartName = string.Empty;
        }

        public static double ConvectionFlux { get; private set; }

        public static string CoolestPartName { get; private set; }

        public static double CoolestSkinTemperature { get; private set; }

        public static double CoolestSkinTemperatureMax { get; private set; }

        public static double CoolestTemperature { get; private set; }

        public static double CoolestTemperatureMax { get; private set; }

        public static string CriticalPartName { get; private set; }

        public static double CriticalSkinTemperature { get; private set; }

        public static double CriticalSkinTemperatureMax { get; private set; }

        public static double CriticalTemperature { get; private set; }

        public static double CriticalTemperatureMax { get; private set; }

        public static double CriticalTemperaturePercentage { get; private set; }

        public static string HottestPartName { get; private set; }

        public static double HottestSkinTemperature { get; private set; }

        public static double HottestSkinTemperatureMax { get; private set; }

        public static double HottestTemperature { get; private set; }

        public static double HottestTemperatureMax { get; private set; }

        public static ThermalProcessor Instance
        {
            get
            {
                return instance;
            }
        }

        public static double InternalFlux { get; private set; }

        public static double RadiationFlux { get; private set; }

        public static bool ShowDetails { get; private set; }

        public void Update()
        {
            if (FlightGlobals.ActiveVessel.parts.Count == 0)
            {
                ShowDetails = false;
                return;
            }

            ShowDetails = true;

            ConvectionFlux = 0.0;
            RadiationFlux = 0.0;
            InternalFlux = 0.0;
            HottestTemperature = 0.0;
            HottestSkinTemperature = 0.0;
            CoolestTemperature = double.MaxValue;
            CoolestSkinTemperature = double.MaxValue;
            CriticalTemperature = double.MaxValue;
            CriticalSkinTemperature = double.MaxValue;
            CriticalTemperaturePercentage = 0.0;
            HottestPartName = string.Empty;
            CoolestPartName = string.Empty;
            CriticalPartName = string.Empty;

            for (int i = 0; i < FlightGlobals.ActiveVessel.parts.Count; ++i)
            {
                Part part = FlightGlobals.ActiveVessel.parts[i];

                ConvectionFlux = ConvectionFlux + part.thermalConvectionFlux;
                RadiationFlux = RadiationFlux + part.thermalRadiationFlux;
                InternalFlux = InternalFlux + part.thermalInternalFluxPrevious;

                if (part.temperature > HottestTemperature || part.skinTemperature > HottestSkinTemperature)
                {
                    HottestTemperature = part.temperature;
                    HottestTemperatureMax = part.maxTemp;
                    HottestSkinTemperature = part.skinTemperature;
                    HottestSkinTemperatureMax = part.skinMaxTemp;
                    HottestPartName = part.partInfo.title;
                }
                if (part.temperature < CoolestTemperature || part.skinTemperature < CoolestSkinTemperature)
                {
                    CoolestTemperature = part.temperature;
                    CoolestTemperatureMax = part.maxTemp;
                    CoolestSkinTemperature = part.skinTemperature;
                    CoolestSkinTemperatureMax = part.skinMaxTemp;
                    CoolestPartName = part.partInfo.title;
                }

                if (part.temperature / part.maxTemp > CriticalTemperaturePercentage || part.skinTemperature / part.skinMaxTemp > CriticalTemperaturePercentage)
                {
                    CriticalTemperature = part.temperature;
                    CriticalTemperatureMax = part.maxTemp;
                    CriticalSkinTemperature = part.skinTemperature;
                    CriticalSkinTemperatureMax = part.skinMaxTemp;
                    CriticalTemperaturePercentage = Math.Max(part.temperature / part.maxTemp, part.skinTemperature / part.skinMaxTemp);
                    CriticalPartName = part.partInfo.title;
                }
            }
        }

        public bool UpdateRequested { get; set; }

        public static void RequestUpdate()
        {
            instance.UpdateRequested = true;
        }
    }
}