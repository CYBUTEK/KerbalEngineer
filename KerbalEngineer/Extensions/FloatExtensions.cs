// Name:    Kerbal Engineer Redux
// Author:  CYBUTEK
// License: Attribution-NonCommercial-ShareAlike 3.0 Unported

namespace KerbalEngineer.Extensions
{
    public static class FloatExtensions
    {
        /// <summary>
        /// Convert to a single precision floating point number.
        /// </summary>
        public static double ToDouble(this float value)
        {
            return (double)value;
        }
    }
}
