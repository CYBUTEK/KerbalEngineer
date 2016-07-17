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

namespace KerbalEngineer.Flight.Readouts
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Miscellaneous;
    using Orbital;
    using Orbital.ManoeuvreNode;
    using Rendezvous;
    using Settings;
    using Surface;
    using Thermal;
    using Vessel;
    using AltitudeSeaLevel = Surface.AltitudeSeaLevel;
    using ApoapsisHeight = Orbital.ApoapsisHeight;
    using OrbitalPeriod = Orbital.OrbitalPeriod;
    using PeriapsisHeight = Orbital.PeriapsisHeight;
    using SemiMajorAxis = Orbital.SemiMajorAxis;
    using SemiMinorAxis = Orbital.SemiMinorAxis;
    using TimeToApoapsis = Orbital.TimeToApoapsis;
    using TimeToPeriapsis = Orbital.TimeToPeriapsis;

    public static class ReadoutLibrary
    {
        private static List<ReadoutModule> readouts = new List<ReadoutModule>();

        /// <summary>
        ///     Sets up and populates the readout library with the stock readouts.
        /// </summary>
        static ReadoutLibrary()
        {
            try
            {
                ReadoutCategory.SetCategory("Orbital", "Readout for orbital manovoeures.");
                ReadoutCategory.SetCategory("Surface", "Surface and atmospheric readouts.");
                ReadoutCategory.SetCategory("Vessel", "Vessel performance statistics.");
                ReadoutCategory.SetCategory("Rendezvous", "Readouts for rendezvous manovoeures.");
                ReadoutCategory.SetCategory("Thermal", "Thermal characteristics readouts.");
                ReadoutCategory.SetCategory("Miscellaneous", "Miscellaneous readouts.");
                ReadoutCategory.Selected = ReadoutCategory.GetCategory("Orbital");

                // Orbital
                readouts.Add(new ApoapsisHeight());
                readouts.Add(new PeriapsisHeight());
                readouts.Add(new TimeToApoapsis());
                readouts.Add(new TimeToPeriapsis());
                readouts.Add(new Inclination());
                readouts.Add(new TimeToEquatorialAscendingNode());
                readouts.Add(new TimeToEquatorialDescendingNode());
                readouts.Add(new AngleToEquatorialAscendingNode());
                readouts.Add(new AngleToEquatorialDescendingNode());
                readouts.Add(new Eccentricity());
                readouts.Add(new OrbitalSpeed());
                readouts.Add(new OrbitalPeriod());
                readouts.Add(new CurrentSoi());
                readouts.Add(new LongitudeOfAscendingNode());
                readouts.Add(new LongitudeOfPeriapsis());
                readouts.Add(new ArgumentOfPeriapsis());
                readouts.Add(new TrueAnomaly());
                readouts.Add(new MeanAnomaly());
                readouts.Add(new MeanAnomalyAtEpoc());
                readouts.Add(new EccentricAnomaly());
                readouts.Add(new SemiMajorAxis());
                readouts.Add(new SemiMinorAxis());
                readouts.Add(new AngleToPrograde());
                readouts.Add(new AngleToRetrograde());
                readouts.Add(new NodeProgradeDeltaV());
                readouts.Add(new NodeNormalDeltaV());
                readouts.Add(new NodeRadialDeltaV());
                readouts.Add(new NodeTotalDeltaV());
                readouts.Add(new NodeBurnTime());
                readouts.Add(new NodeHalfBurnTime());
                readouts.Add(new NodeTimeToManoeuvre());
                readouts.Add(new NodeTimeToHalfBurn());
                readouts.Add(new NodeAngleToPrograde());
                readouts.Add(new NodeAngleToRetrograde());
                readouts.Add(new PostBurnApoapsis());
                readouts.Add(new PostBurnPeriapsis());
                readouts.Add(new SpeedAtApoapsis());
                readouts.Add(new SpeedAtPeriapsis());
                readouts.Add(new TimeToAtmosphere());

                // Surface
				readouts.Add(new AltitudeSeaLevel());
				readouts.Add(new AltitudeTerrain());
                readouts.Add(new VerticalSpeed());
                readouts.Add(new VerticalAcceleration());
                readouts.Add(new HorizontalSpeed());
                readouts.Add(new HorizontalAcceleration());
                readouts.Add(new MachNumber());
                readouts.Add(new Latitude());
                readouts.Add(new Longitude());
                readouts.Add(new GeeForce());
                readouts.Add(new TerminalVelocity());
                readouts.Add(new AtmosphericEfficiency());
                readouts.Add(new Biome());
                readouts.Add(new Situation());
                readouts.Add(new Slope());
                readouts.Add(new ImpactTime());
                readouts.Add(new ImpactLongitude());
                readouts.Add(new ImpactLatitude());
                readouts.Add(new ImpactAltitude());
                readouts.Add(new ImpactBiome());

                // Vessel
                readouts.Add(new Name());
                readouts.Add(new DeltaVStaged());
                readouts.Add(new DeltaVCurrent());
                readouts.Add(new DeltaVTotal());
                readouts.Add(new DeltaVCurrentTotal());
                readouts.Add(new SpecificImpulse());
                readouts.Add(new Mass());
                readouts.Add(new Thrust());
                readouts.Add(new ThrustToWeight());
                readouts.Add(new ThrustOffsetAngle());
                readouts.Add(new ThrustTorque());
                readouts.Add(new SurfaceThrustToWeight());
                readouts.Add(new Acceleration());
                readouts.Add(new SuicideBurnAltitude());
                readouts.Add(new SuicideBurnDistance());
                readouts.Add(new SuicideBurnDeltaV());
                readouts.Add(new IntakeAirUsage());
                readouts.Add(new IntakeAirDemand());
                readouts.Add(new IntakeAirSupply());
                readouts.Add(new IntakeAirDemandSupply());
                readouts.Add(new PartCount());
                readouts.Add(new Heading());
                readouts.Add(new Pitch());
                readouts.Add(new Roll());
                readouts.Add(new HeadingRate());
                readouts.Add(new PitchRate());
                readouts.Add(new RollRate());

                // Rendezvous
                readouts.Add(new TargetSelector());
                readouts.Add(new PhaseAngle());
                readouts.Add(new InterceptAngle());
                readouts.Add(new RelativeVelocity());
                readouts.Add(new RelativeSpeed());
                readouts.Add(new RelativeInclination());
                readouts.Add(new TimeToRelativeAscendingNode());
                readouts.Add(new TimeToRelativeDescendingNode());
                readouts.Add(new AngleToRelativeAscendingNode());
                readouts.Add(new AngleToRelativeDescendingNode());
                readouts.Add(new Rendezvous.AltitudeSeaLevel());
                readouts.Add(new Rendezvous.ApoapsisHeight());
                readouts.Add(new Rendezvous.PeriapsisHeight());
                readouts.Add(new Rendezvous.TimeToApoapsis());
                readouts.Add(new Rendezvous.TimeToPeriapsis());
                readouts.Add(new Distance());
                readouts.Add(new Rendezvous.OrbitalPeriod());
                readouts.Add(new Rendezvous.SemiMajorAxis());
                readouts.Add(new Rendezvous.SemiMinorAxis());
                readouts.Add(new Rendezvous.RelativeRadialVelocity());
                readouts.Add(new Rendezvous.TimeToRendezvous());
                readouts.Add(new TargetLatitude());
                readouts.Add(new TargetLongitude());

                // Thermal
                readouts.Add(new InternalFlux());
                readouts.Add(new ConvectionFlux());
                readouts.Add(new RadiationFlux());
                readouts.Add(new CriticalPart());
                readouts.Add(new CriticalTemperature());
                readouts.Add(new CriticalSkinTemperature());
                readouts.Add(new CriticalThermalPercentage());
                readouts.Add(new HottestPart());
                readouts.Add(new HottestTemperature());
                readouts.Add(new HottestSkinTemperature());
                readouts.Add(new CoolestPart());
                readouts.Add(new CoolestTemperature());
                readouts.Add(new CoolestSkinTemperature());

                // Misc
                readouts.Add(new Separator());
                readouts.Add(new GuiSizeAdjustor());
                readouts.Add(new SimulationDelay());
                readouts.Add(new VectoredThrustToggle());
                readouts.Add(new SystemTime());
                readouts.Add(new LogSimToggle());

                LoadHelpStrings();
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }

        /// <summary>
        ///     Gets and sets the available readout modules.
        /// </summary>
        public static List<ReadoutModule> Readouts
        {
            get
            {
                return readouts;
            }
            set
            {
                readouts = value;
            }
        }

        /// <summary>
        ///     Gets a list of readout modules which are associated with the specified category.
        /// </summary>
        public static List<ReadoutModule> GetCategory(ReadoutCategory category)
        {
            return readouts.Where(r => r.Category == category).ToList();
        }

        /// <summary>
        ///     Gets a readout module with the specified name or class name. (Returns null if not found.)
        /// </summary>
        public static ReadoutModule GetReadout(string name)
        {
            return readouts.FirstOrDefault(r => r.Name == name || r.GetType().Name == name || r.Category + "." + r.GetType().Name == name);
        }

        /// <summary>
        ///     Resets all the readout modules.
        /// </summary>
        public static void Reset()
        {
            foreach (ReadoutModule readout in readouts)
            {
                readout.Reset();
            }
        }

        /// <summary>
        ///     Loads the help strings from file.
        /// </summary>
        private static void LoadHelpStrings()
        {
            try
            {
                SettingHandler handler = SettingHandler.Load("HelpStrings.xml");
                foreach (ReadoutModule readout in readouts)
                {
                    readout.HelpString = handler.GetSet(readout.Category + "." + readout.GetType().Name, readout.HelpString);
                }
                handler.Save("HelpStrings.xml");
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }
    }
}