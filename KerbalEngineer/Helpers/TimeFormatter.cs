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

using System;

namespace KerbalEngineer.Helpers
{
    public static class TimeFormatter
    {
        public static string ConvertToString(double seconds, string format = "F1")
        {
            bool negative = seconds < 0;

            seconds = Math.Abs(seconds);

            if (!negative)
            {

                int years = 0;
                int days = 0;
                int hours = 0;
                int minutes = 0;

                years = (int)(seconds / KSPUtil.dateTimeFormatter.Year);
                seconds -= years * KSPUtil.dateTimeFormatter.Year;

                days = (int)(seconds / KSPUtil.dateTimeFormatter.Day);
                seconds -= days * KSPUtil.dateTimeFormatter.Day;

                hours =(int)(seconds / 3600.0);
                seconds -= hours * 3600.0;

                minutes = (int)(seconds / 60.0);
                seconds -= minutes * 60.0;

                if (years > 0)
                {
                    return string.Format("{0}y {1}d", years, days);
                }
                if (days > 0)
                {
                    return string.Format("{0}d {1}h", days, hours);
                }
                if (hours > 0)
                {
                    return string.Format("{0}h {1}m", hours, minutes);
                }
                if (minutes > 0)
                {
                    return string.Format("{0}m {1}s", minutes, seconds.ToString("F0"));
                }
                return string.Format("{0}s", seconds.ToString(format));
            }
            else
            {
                return  "-"  + string.Format("{0}s", seconds.ToString(format));
            }

        }
    }
}