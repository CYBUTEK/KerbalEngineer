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

namespace KerbalEngineer.Helpers
{
    using System;

    public static class Units
    {
        public const double GRAVITY = 9.80665;
        public const double RAD_TO_DEG = 180 / Math.PI;
        public const double DEG_TO_RAD = Math.PI / 180;

        public static string Concat(int value1, int value2)
        {
            return value1 + " / " + value2;
        }

        public static string ConcatF(double value1, double value2, int decimals = 1)
        {
            string f = "F" + decimals;
            return value1.ToString(f) + " / " + value2.ToString(f);
        }

        public static string ConcatF(double value1, double value2, double value3, int decimals = 1)
        {
            string f = "F" + decimals;
            return value1.ToString(f) + " / " + value2.ToString(f) + " / " + value3.ToString(f);
        }

        public static string ConcatN(double value1, double value2, int decimals = 1)
        {
            string f = "N" + decimals;
            return value1.ToString(f) + " / " + value2.ToString(f);
        }

        public static string ConcatN(double value1, double value2, double value3, int decimals = 1)
        {
            string f = "N" + decimals;
            return value1.ToString(f) + " / " + value2.ToString(f) + " / " + value3.ToString(f);
        }

        public static string Cost(double value, int decimals = 1)
        {
            string f = "N" + decimals;

            if (value >= 1000000)
                return (value / 1000).ToString(f) + "K";

            return value.ToString(f);
        }

        public static string Cost(double value1, double value2, int decimals = 1)
        {
            string f = "N" + decimals;

            if (value1 >= 1000000 || value2 >= 1000000)
                return (value1 / 1000).ToString(f) + " / " + (value2 / 1000).ToString(f) + "K";

            return value1.ToString(f) + " / " + value2.ToString(f);
        }

        public static string ToAcceleration(double value, int decimals = 2)
        {
            string f = "N" + decimals;
            return value.ToString(f) + "m/s²";
        }

        public static string ToAcceleration(double value1, double value2, int decimals = 2)
        {
            string f = "N" + decimals;
            return value1.ToString(f) + " / " + value2.ToString(f) + "m/s²";
        }

        public static string ToAngle(double value, int decimals = 5)
        {
            string f = "F" + decimals;
            return value.ToString(f) + "°";
        }

        public static string ToAngleDMS(double value)
        {
            double absAngle = Math.Abs(value);
            int deg = (int)Math.Floor(absAngle);
            double rem = absAngle - deg;
            int min = (int)Math.Floor(rem * 60);
            rem -= ((double)min / 60d);
            int sec = (int)Math.Floor(rem * 3600);
            return string.Format("{0:0}° {1:00}' {2:00}\"", deg, min, sec);
        }

        public static string ToDistance(double value, int decimals = 1)
        {
            string f = "N" + decimals;
            if (Math.Abs(value) < 1000000)
            {
                if (Math.Abs(value) >= 10)
                    return value.ToString(f) + "m";

                value *= 100;
                if (Math.Abs(value) >= 100)
                    return value.ToString(f) + "cm";

                value *= 10;
                return value.ToString(f) + "mm";
            }

            value /= 1000;
            if (Math.Abs(value) < 1000000)
                return value.ToString(f) + "km";

            value /= 1000;
            return value.ToString(f) + "Mm";
        }

        public static string ToFlux(double value)
        {
            return value.ToString("#,0.00") + "kW";
        }

        public static string ToForce(double value)
        {
            return value.ToString((value < 100000) ? (value < 10000) ? (value < 100) ? (Math.Abs(value) < double.Epsilon) ? "N0" : "N3" : "N2" : "N1" : "N0") + "kN";
        }

        public static string ToForce(double value1, double value2)
        {
            string format1 = (value1 < 100000) ? (value1 < 10000) ? (value1 < 100) ? (Math.Abs(value1) < double.Epsilon) ? "N0" : "N3" : "N2" : "N1" : "N0";
            string format2 = (value2 < 100000) ? (value2 < 10000) ? (value2 < 100) ? (Math.Abs(value2) < double.Epsilon) ? "N0" : "N3" : "N2" : "N1" : "N0";
            return value1.ToString(format1) + " / " + value2.ToString(format2) + "kN";
        }

        public static string ToMach(double value)
        {
            return value.ToString("0.00") + "Ma";
        }

        public static string ToMass(double value, int decimals = 0)
        {
            string f = "N" + decimals;
            string f2 = "N" + decimals + 2;

            if (value >= 1000)
                return value.ToString(f2) + "t";

            return (value * 1000).ToString(f) + "kg";
        }

        public static string ToMass(double value1, double value2, int decimals = 0)
        {
            if (value1 >= 1000 || value2 >= 1000)
            {
                string f2 = "N" + decimals + 2;
                return value1.ToString(f2) + " / " + value2.ToString(f2) + "t";
            }

            string f = "N" + decimals;
            return (value1 * 1000).ToString(f) + " / " + (value2 * 1000).ToString(f) + "kg";
        }

        public static string ToPercent(double value, int decimals = 2)
        {
            string f = "F" + decimals;
            return (value * 100).ToString(f) + "%";
        }

        public static string ToPressure(double value)
        {
            return value.ToString((value < 100000) ? (value < 10000) ? (value < 100) ? (Math.Abs(value) < double.Epsilon) ? "N0" : "N3" : "N2" : "N1" : "N0") + "kN/m²";
        }

        public static string ToRate(double value, int decimals = 1)
        {
            string f = "F" + decimals;
            return value < 1 ? (value * 60).ToString(f) + "/min" : value.ToString(f) + "/sec";
        }

        public static string ToSpeed(double value, int decimals = 2)
        {
            string f = "N" + decimals;

            if (Math.Abs(value) < 1)
                return (value * 1000).ToString(f) + "mm/s";

            return value.ToString(f) + "m/s";
        }

        public static string ToTemperature(double value)
        {
            return value.ToString("#,0") + "K";
        }

        public static string ToTemperature(double value1, double value2)
        {
            return value1.ToString("#,0") + " / " + value2.ToString("#,0") + "K";
        }

        public static string ToTime(double value)
        {
            return TimeFormatter.ConvertToString(value);
        }

        public static string ToTorque(double value)
        {
            return value.ToString((value < 100) ? (Math.Abs(value) < double.Epsilon) ? "N0" : "N2" : "N0") + "kNm";
        }
    }
}
