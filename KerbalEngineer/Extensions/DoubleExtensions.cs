// 
//     Kerbal Engineer Redux
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

namespace KerbalEngineer.Extensions
{
    using Helpers;

    public static class DoubleExtensions
    {
        public static double Clamp(this double value, double lower, double higher)
        {
            return value < lower ? lower : value > higher ? higher : value;
        }

        public static string ToAcceleration(this double value)
        {
            return Units.ToAcceleration(value);
        }

        public static string ToAngle(this double value)
        {
            return Units.ToAngle(value);
        }

        public static string ToDistance(this double value)
        {
            return Units.ToDistance(value);
        }

        public static string ToFlux(this double value)
        {
            return Units.ToFlux(value);
        }

        public static string ToForce(this double value)
        {
            return Units.ToForce(value);
        }

        public static string ToMach(this double value)
        {
            return Units.ToMach(value);
        }

        public static string ToMass(this double value)
        {
            return Units.ToMass(value);
        }

        public static string ToPercent(this double value)
        {
            return Units.ToPercent(value);
        }

        public static string ToRate(this double value)
        {
            return Units.ToRate(value);
        }

        public static string ToSpeed(this double value)
        {
            return Units.ToSpeed(value);
        }

        public static string ToTemperature(this double value)
        {
            return Units.ToTemperature(value);
        }

        public static string ToTorque(this double value)
        {
            return Units.ToTorque(value);
        }
    }
}