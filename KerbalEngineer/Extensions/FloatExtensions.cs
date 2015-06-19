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

    public static class FloatExtensions
    {
        public static string ToAcceleration(this float value)
        {
            return Units.ToAcceleration(value);
        }

        public static string ToAngle(this float value)
        {
            return Units.ToAngle(value);
        }

        public static string ToDistance(this float value)
        {
            return Units.ToDistance(value);
        }

        public static string ToFlux(this float value)
        {
            return Units.ToFlux(value);
        }

        public static string ToForce(this float value)
        {
            return Units.ToForce(value);
        }

        public static string ToMach(this float value)
        {
            return Units.ToMach(value);
        }

        public static string ToMass(this float value)
        {
            return Units.ToMass(value);
        }

        public static string ToPercent(this float value)
        {
            return Units.ToPercent(value);
        }

        public static string ToRate(this float value)
        {
            return Units.ToRate(value);
        }

        public static string ToSpeed(this float value)
        {
            return Units.ToSpeed(value);
        }

        public static string ToTemperature(this float value)
        {
            return Units.ToTemperature(value);
        }

        public static string ToTorque(this float value)
        {
            return Units.ToTorque(value);
        }
    }
}