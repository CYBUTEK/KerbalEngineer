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
    public static class FloatExtensions
    {
        /// <summary>
        ///     Convert to a double precision floating point number.
        /// </summary>
        public static double ToDouble(this float value)
        {
            return value;
        }

        /// <summary>
        ///     Convert to a string formatted as a mass.
        /// </summary>
        public static string ToMass(this float value, bool showNotation = true)
        {
            value *= 1000;

            return showNotation ? value.ToString("N0") + "kg" : value.ToString("N0");
        }

        /// <summary>
        ///     Convert to string formatted as a force.
        /// </summary>
        public static string ToForce(this float value, bool showNotation = true)
        {
            return showNotation ? value.ToString("N2") + "kN" : value.ToString("N2");
        }

        /// <summary>
        ///     Convert to string formatted as a speed.
        /// </summary>
        public static string ToSpeed(this float value, bool showNotation = true)
        {
            return showNotation ? value.ToString("N2") + "m/s" : value.ToString("N2");
        }

        /// <summary>
        ///     Convert to string formatted as a distance.
        /// </summary>
        public static string ToDistance(this float value)
        {
            var negative = value < 0;

            if (negative)
            {
                value = -value;
            }

            if (value < 1000000.0f)
            {
                if (value < 1.0f)
                {
                    value *= 1000.0f;

                    if (negative)
                    {
                        value = -value;
                    }
                    return value.ToString("N0") + "mm";
                }

                if (negative)
                {
                    value = -value;
                }
                return value.ToString("N0") + "m";
            }

            value /= 1000.0f;
            if (value >= 1000000.0f)
            {
                value /= 1000.0f;

                if (negative)
                {
                    value = -value;
                }
                return value.ToString("N0") + "Mm";
            }

            if (negative)
            {
                value = -value;
            }
            return value.ToString("N0") + "km";
        }

        /// <summary>
        ///     Convert to string formatted as a rate.
        /// </summary>
        public static string ToRate(this float value)
        {
            return value > 0 ? value.ToString("F1") + "/sec" : (60.0f * value).ToString("F1") + "/min";
        }

        /// <summary>
        ///     Convert to string formatted as an angle.
        /// </summary>
        public static string ToAngle(this float value)
        {
            return value.ToString("F3") + "°";
        }

        /// <summary>
        ///     Convert to string formatted as a time.
        /// </summary>
        public static string ToTime(this float value)
        {
            var s = value;
            var m = 0;
            var h = 0;
            var d = 0;
            var y = 0;

            // Years
            while (s >= 31536000)
            {
                y++;
                s -= 31536000;
            }

            // Days
            while (s >= 86400)
            {
                d++;
                s -= 86400;
            }

            // Hours
            while (s >= 3600)
            {
                h++;
                s -= 3600;
            }

            // Minutes
            while (s >= 60)
            {
                m++;
                s -= 60;
            }

            if (y > 0)
            {
                return y + "y " + d + "d " + h + "h " + m + "m " + s.ToString("F1") + "s";
            }

            if (d > 0)
            {
                return d + "d " + h + "h " + m + "m " + s.ToString("F1") + "s";
            }

            if (h > 0)
            {
                return h + "h " + m + "m " + s.ToString("F1") + "s";
            }

            if (m > 0)
            {
                return m + "m " + s.ToString("F1") + "s";
            }

            return s.ToString("F1") + "s";
        }
    }
}