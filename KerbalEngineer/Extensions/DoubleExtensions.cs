// Name:    Kerbal Engineer Redux
// Author:  CYBUTEK
// License: Attribution-NonCommercial-ShareAlike 3.0 Unported

namespace KerbalEngineer.Extensions
{
    public static class DoubleExtensions
    {
        /// <summary>
        /// Convert to a string formatted as a mass.
        /// </summary>
        public static string ToMass(this double value, bool showNotation = true)
        {
            value *= 1000;

            if (showNotation)
                return value.ToString("#,0.") + " kg";
            else
                return value.ToString("#,0.");
        }

        /// <summary>
        /// Convert to string formatted as a force.
        /// </summary>
        public static string ToForce(this double value, bool showNotation = true)
        {
            if (showNotation)
                return value.ToString("#,0.#") + " kN";
            else
                return value.ToString("#,0.#");
        }

        /// <summary>
        /// Convert to string formatted as a speed.
        /// </summary>
        public static string ToSpeed(this double value, bool showNotation = true)
        {
            if (showNotation)
                return value.ToString("0") + " m/s";
            else
                return value.ToString("0");
        }

        /// <summary>
        /// Convert to string formatted as a time.
        /// </summary>
        public static string ToTime(this double value)
        {
            double s = value;
            int m = 0;
            int h = 0;
            double d = 0d;
            double y = 0d;

            if (s >= 31536000)
            {
                while (s >= 31536000)
                {
                    y++;
                    s -= 31536000;
                }

                y += (s / 31536000);
                return y.ToString("0.000") + "y";
            }

            if (s >= 86400)
            {
                while (s >= 86400)
                {
                    d++;
                    s -= 86400;
                }

                d += (s / 86400);
                return d.ToString("0.000") + "d";
            }

            while (s >= 60)
            {
                m++;
                s -= 60;
            }

            while (m >= 60)
            {
                h++;
                m -= 60;
            }

            while (h >= 24)
            {
                d++;
                h -= 24;
            }

            if (h > 0)
            {
                return h + ":" + m.ToString("00") + ":" + s.ToString("00.0") + "s";
            }

            if (m > 0)
            {
                return m + ":" + s.ToString("00.0") + "s";
            }

            return s.ToString("0.0") + "s";
        }
    }
}
