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
    public static class TimeFormatter
    {
        public static string ConvertToString(double seconds, string format = "F1")
        {
            int years = 0;
            int days = 0;
            int hours = 0;
            int minutes = 0;

            if (seconds > 0.0)
            {
                years = (int)(seconds / KSPUtil.dateTimeFormatter.Year);
                seconds -= years * KSPUtil.dateTimeFormatter.Year;

                days = (int)(seconds / KSPUtil.dateTimeFormatter.Day);
                seconds -= days * KSPUtil.dateTimeFormatter.Day;

                hours = (int)(seconds / 3600.0);
                seconds -= hours * 3600.0;

                minutes = (int)(seconds / 60.0);
                seconds -= minutes * 60.0;
            }

            if (years > 0)
            {
                return string.Format("{0}y {1}d {2}h {3}m {4}s", years, days, hours, minutes, seconds.ToString(format));
            }
            if (days > 0)
            {
                return string.Format("{0}d {1}h {2}m {3}s", days, hours, minutes, seconds.ToString(format));
            }
            if (hours > 0)
            {
                return string.Format("{0}h {1}m {2}s", hours, minutes, seconds.ToString(format));
            }

            return minutes > 0 ? string.Format("{0}m {1}s", minutes, seconds.ToString(format)) : string.Format("{0}s", seconds.ToString(format));
        }
    }
}