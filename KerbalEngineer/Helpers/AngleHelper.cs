// Kerbal Engineer Redux
//
// Copyright (C) 2014 CYBUTEK
//
// This program is free software: you can redistribute it and/or modify it under the terms of the GNU
// General Public License as published by the Free Software Foundation, either version 3 of the
// License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without
// even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
// General Public License for more details.
//
// You should have received a copy of the GNU General Public License along with this program. If not,
// see <http://www.gnu.org/licenses/>.

namespace KerbalEngineer.Helpers
{
    using UnityEngine;
    using Extensions;

    public static class AngleHelper
    {
        /// <summary>
        ///     Same as ModuloBetween(angle, -180, 180).
        /// </summary>
        public static double Modulo180(double angle)
        {
            return ModuloBetween(angle, -180, 180);
        }

        /// <summary>
        ///     Same as ModuloBetween(angle, 0, 360).
        /// </summary>
        public static double Modulo360(double angle)
        {
            return ModuloBetween(angle, 0, 360);
        }

        public static double ModuloBetween(double value, double minimum, double maximum)
        {
            if (value.IsValid() && minimum.IsValid() && maximum.IsValid())
            {
                double mod = maximum - minimum;

                while (value < minimum)
                    value += mod;

                while (value > maximum)
                    value -= mod;
            }

            return value;
        }

        public static double GetAngleBetweenVectors(Vector3d left, Vector3d right)
        {
            double angle = Vector3d.Angle(left, right);
            Vector3d rotated = QuaternionD.AngleAxis(90, Vector3d.forward) * right;

            if (Vector3d.Angle(rotated, left) > 90)
                return 360 - angle;

            return angle;
        }

    }
}
