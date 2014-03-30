// Project:	KerbalEngineer
// Author:	CYBUTEK
// License:	Attribution-NonCommercial-ShareAlike 3.0 Unported

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