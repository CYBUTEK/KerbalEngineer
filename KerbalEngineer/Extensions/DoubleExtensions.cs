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

using KerbalEngineer.Helpers;

#endregion

namespace KerbalEngineer.Extensions
{
    public static class DoubleExtensions
    {
        #region Methods: public

        public static double ClampTo(this double value, double min, double max)
        {
            while (value < min)
            {
                value += max;
            }

            while (value > max)
            {
                value -= max;
            }

            return value;
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

        public static float ToFloat(this double value)
        {
            try
            {
                return (float)value;
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "DoubleExtentions->ToFloat");
                return 0;
            }
        }

        public static string ToForce(this double value)
        {
            return Units.ToForce(value);
        }

        public static string ToMass(this double value)
        {
            return Units.ToMass(value);
        }

        public static string ToRate(this double value)
        {
            try
            {
                return value > 0 ? value.ToString("F1") + "/sec" : (60.0f * value).ToString("F1") + "/min";
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "DoubleExtentions->ToRate");
                return "ERR";
            }
        }

        public static string ToSpeed(this double value)
        {
            return Units.ToSpeed(value);
        }

        #endregion
    }
}