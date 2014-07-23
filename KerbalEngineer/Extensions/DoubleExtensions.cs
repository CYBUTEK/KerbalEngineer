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

namespace KerbalEngineer.Extensions
{
    public static class DoubleExtensions
    {
        /// <summary>
        ///     Convert to a single precision floating point number.
        /// </summary>
        public static float ToFloat(this double value)
        {
            return (float)value;
        }

        /// <summary>
        ///     Convert to a string formatted as a mass.
        /// </summary>
        public static string ToMass(this double value, bool showNotation = true)
        {
            return ToFloat(value).ToMass(showNotation);
        }

        /// <summary>
        ///     Convert to string formatted as a force.
        /// </summary>
        public static string ToForce(this double value, bool showNotation = true)
        {
            return ToFloat(value).ToForce(showNotation);
        }

        /// <summary>
        ///     Convert to string formatted as a speed.
        /// </summary>
        public static string ToSpeed(this double value, bool showNotation = true)
        {
            return ToFloat(value).ToSpeed(showNotation);
        }

        /// <summary>
        ///     Convert to string formatted as a distance.
        /// </summary>
        public static string ToDistance(this double value)
        {
            return ToFloat(value).ToDistance();
        }

        /// <summary>
        ///     Convert to string formatted as a rate.
        /// </summary>
        public static string ToRate(this double value)
        {
            return ToFloat(value).ToRate();
        }

        /// <summary>
        ///     Convert to string formatted as an angle.
        /// </summary>
        public static string ToAngle(this double value)
        {
            return ToFloat(value).ToAngle();
        }

        /// <summary>
        ///     Convert to string formatted as a time.
        /// </summary>
        public static string ToTime(this double value)
        {
            return ToFloat(value).ToTime();
        }
    }
}