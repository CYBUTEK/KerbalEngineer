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

using UnityEngine;

#endregion

namespace KerbalEngineer.Helpers
{
    public static class AngleHelper
    {
        #region Methods: public

        public static double Clamp360(double value)
        {
            return ClampBetween(value, 0.0, 360.0);
        }

        public static double ClampBetween(double value, double minimum, double maximum)
        {
            while (value < minimum)
            {
                value += maximum;
            }

            while (value > maximum)
            {
                value -= maximum;
            }

            return value;
        }

        public static double GetAngleBetweenVectors(Vector3d vector1, Vector3d vector2)
        {
            var angle = Vector3d.Angle(vector1, vector2);
            var rotated = QuaternionD.AngleAxis(90.0, Vector3d.forward) * vector1;

            if (Vector3d.Angle(rotated, vector2) > 90.0)
            {
                angle = 360.0 - angle;
            }

            return angle;
        }

        #endregion
    }
}