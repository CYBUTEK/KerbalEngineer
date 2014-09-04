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

#endregion

namespace KerbalEngineer.Helpers
{
    public static class Units
    {
        #region Methods: public

        public static string ToAcceleration(double value, int decimals = 2)
        {
            return value.ToString("N" + decimals) + "m/s²";
        }

        public static string ToAcceleration(double value1, double value2, int decimals = 2)
        {
            return value1.ToString("N" + decimals) + " / " + value2.ToString("N" + decimals) + "m/s²";
        }

        public static string ToAngle(double value, int decimals = 5)
        {
            return value.ToString("F" + decimals) + "°";
        }

        public static string ToDistance(double value, int decimals = 1)
        {
            if (Math.Abs(value) < 1000000.0)
            {
                if (Math.Abs(value) >= 10.0)
                {
                    return value.ToString("N" + decimals) + "m";
                }

                value *= 100.0;
                if (Math.Abs(value) >= 100.0)
                {
                    return value.ToString("N" + decimals) + "cm";
                }

                value *= 10.0;
                return value.ToString("N" + decimals) + "mm";
            }

            value /= 1000.0;
            if (Math.Abs(value) < 1000000.0)
            {
                return value.ToString("N" + decimals) + "km";
            }

            value /= 1000.0;
            return value.ToString("N" + decimals) + "Mm";
        }

        public static string ToForce(double value)
        {
            return value.ToString((value < 100000.0) ? (value < 10000.0) ? (value < 100.0) ? "N3" : "N2" : "N1" : "N0") + "kN";
        }

        public static string ToForce(double value1, double value2)
        {
            var format = (value1 < 100000.0) ? (value1 < 10000.0) ? (value1 < 100.0) ? "N3" : "N2" : "N1" : "N0";
            return value1.ToString(format) + " / " + value2.ToString(format) + "kN";
        }

        public static string ToMass(double value, int decimals = 0)
        {
            value *= 1000.0;
            return value.ToString("N" + decimals) + "kg";
        }

        public static string ToMass(double value1, double value2, int decimals = 0)
        {
            value1 *= 1000.0;
            value2 *= 1000.0;
            return value1.ToString("N" + decimals) + " / " + value2.ToString("N" + decimals) + "kg";
        }

        public static string ToRate(double value, int decimals = 1)
        {
            return value > 0 ? value.ToString("F" + decimals) + "/sec" : (value * 60.0).ToString("F" + decimals);
        }

        public static string ToSpeed(double value, int decimals = 2)
        {
            if (Math.Abs(value) < 1.0)
            {
                return (value * 1000.0).ToString("N" + decimals) + "mm/s";
            }
            return value.ToString("N" + decimals) + "m/s";
        }

        public static string ToTime(double value)
        {
            return TimeFormatter.ConvertToString(value);
        }

        #endregion
    }
}