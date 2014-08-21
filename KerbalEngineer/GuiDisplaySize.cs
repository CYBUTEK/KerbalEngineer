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

namespace KerbalEngineer
{
    public class GuiDisplaySize
    {
        #region Delegates

        public delegate void SizeChanged();

        #endregion

        #region Fields

        private static float multiplier = 1.1f;
        private static int increment;
        private static float offset;
        public static event SizeChanged OnSizeChanged;

        #endregion

        #region Constructor

        static GuiDisplaySize()
        {
            try
            {
                var handler = SettingHandler.Load("GuiDisplaySize.xml");
                handler.GetSet("multiplier", ref multiplier);
                handler.GetSet("increment", ref increment);
                handler.Save("GuiDisplaySize.xml");
                offset = 1 + (increment * multiplier) - increment;
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "GuiDisplaySize->GuiDisplaySize");
            }
        }

        #endregion

        #region Properties

        public static float Multiplier
        {
            get { return multiplier; }
            set
            {
                try
                {
                    if (multiplier == value)
                    {
                        return;
                    }

                    multiplier = value;
                    var handler = SettingHandler.Load("GuiDisplaySize.xml");
                    handler.Set("multiplier", multiplier);
                    handler.Save("GuiDisplaySize.xml");
                    offset = 1 + (increment * multiplier) - increment;
                    OnSizeChanged();
                }
                catch (Exception ex)
                {
                    Logger.Exception(ex, "GuiDisplaySize->Multiplier");
                }
            }
        }

        public static int Increment
        {
            get { return increment; }
            set
            {
                try
                {
                    if (increment == value)
                    {
                        return;
                    }

                    increment = value;
                    var handler = SettingHandler.Load("GuiDisplaySize.xml");
                    handler.Set("increment", increment);
                    handler.Save("GuiDisplaySize.xml");
                    offset = 1 + (increment * multiplier) - increment;
                    OnSizeChanged();
                }
                catch (Exception ex)
                {
                    Logger.Exception(ex, "GuiDisplaySize->Increment");
                }
            }
        }

        public static float Offset
        {
            get { return offset; }
        }

        #endregion
    }
}