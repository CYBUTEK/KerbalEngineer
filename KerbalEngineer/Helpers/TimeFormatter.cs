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

using KerbalEngineer.Settings;

#endregion

namespace KerbalEngineer.Helpers
{
    public static class TimeFormatter
    {
        #region Constructors

        static TimeFormatter()
        {
            SetReference(false);
            Load();
        }

        #endregion

        #region Properties

        public static string Reference { get; set; }

        public static double SecondsPerDay { get; set; }

        public static double SecondsPerHour { get; set; }

        public static double SecondsPerMinute { get; set; }

        public static double SecondsPerYear { get; set; }

        #endregion

        #region Methods: public

        public static string ConvertToString(double seconds, string format = "F1")
        {
            var years = 0;
            while (seconds >= SecondsPerYear)
            {
                years++;
                seconds -= SecondsPerYear;
            }

            var days = 0;
            while (seconds >= SecondsPerDay)
            {
                days++;
                seconds -= SecondsPerDay;
            }

            var hours = 0;
            while (seconds >= SecondsPerHour)
            {
                hours++;
                seconds -= SecondsPerHour;
            }

            var minutes = 0;
            while (seconds >= SecondsPerMinute)
            {
                minutes++;
                seconds -= SecondsPerMinute;
            }

            if (years > 0)
            {
                return String.Format("{0}y {1}d {2}h {3}m {4}s", years, days, hours, minutes, seconds.ToString(format));
            }
            if (days > 0)
            {
                return String.Format("{0}d {1}h {2}m {3}s", days, hours, minutes, seconds.ToString(format));
            }
            if (hours > 0)
            {
                return String.Format("{0}h {1}m {2}s", hours, minutes, seconds.ToString(format));
            }

            return minutes > 0 ? String.Format("{0}m {1}s", minutes, seconds.ToString(format)) : String.Format("{0}s", seconds.ToString(format));
        }

        public static void Load()
        {
            var handler = SettingHandler.Load("TimeFormatter.xml");
            SecondsPerMinute = handler.Get("SecondsPerMinute", SecondsPerMinute);
            SecondsPerHour = handler.Get("SecondsPerHour", SecondsPerHour);
            SecondsPerDay = handler.Get("SecondsPerDay", SecondsPerDay);
            SecondsPerYear = handler.Get("SecondsPerYear", SecondsPerYear);
            Reference = handler.Get("Reference", Reference);
        }

        public static void Save()
        {
            var handler = SettingHandler.Load("TimeFormatter.xml");
            handler.Set("SecondsPerMinute", SecondsPerMinute);
            handler.Set("SecondsPerHour", SecondsPerHour);
            handler.Set("SecondsPerDay", SecondsPerDay);
            handler.Set("SecondsPerYear", SecondsPerYear);
            handler.Set("Reference", Reference);
            handler.Save("TimeFormatter.xml");
        }

        public static void SetReference(bool save = true)
        {
            const double minute = 60.0;
            const double hour = minute * 60.0;
            const double day = hour * 24.0;
            const double year = day * 365.0;
            SetReference(minute, hour, day, year, "Earth", save);
        }

        public static void SetReference(CelestialBody body, bool save = true)
        {
            SetReference(SecondsPerMinute, SecondsPerHour, body.rotationPeriod, body.orbit.period, body.bodyName, save);
        }

        public static void SetReference(double minute, double hour, double day, double year, string reference, bool save = true)
        {
            SecondsPerMinute = minute;
            SecondsPerHour = hour;
            SecondsPerDay = day;
            SecondsPerYear = year;
            Reference = reference;

            if (save)
            {
                Save();
            }
        }

        public new static string ToString()
        {
            return String.Format("SecondsPerMinute: {0}", SecondsPerMinute) + Environment.NewLine +
                   String.Format("SecondsPerHour: {0}", SecondsPerHour) + Environment.NewLine +
                   String.Format("SecondsPerDay: {0}", SecondsPerDay) + Environment.NewLine +
                   String.Format("SecondsPerYear: {0}", SecondsPerYear) + Environment.NewLine;
        }

        #endregion
    }
}