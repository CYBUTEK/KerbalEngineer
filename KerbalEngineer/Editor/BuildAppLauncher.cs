// 
//     Kerbal Engineer Redux
// 
//     Copyright (C) 2016 CYBUTEK
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
//     You should have received a copy of the GNU General Public License
//     along with this program.  If not, see <http://www.gnu.org/licenses/>.
//  

namespace KerbalEngineer.Editor
{
    [KSPAddon(KSPAddon.Startup.EditorAny, false)]
    public class BuildAppLauncher : AppLauncherButton
    {
        protected override void OnFalse()
        {
            if (BuildAdvanced.Instance != null)
            {
                BuildAdvanced.Instance.Visible = false;
            }
        }

        protected override void OnTrue()
        {
            if (BuildAdvanced.Instance != null)
            {
                BuildAdvanced.Instance.Visible = true;
            }
        }

        protected virtual void Update()
        {
            if (BuildAdvanced.Instance == null)
            {
                return;
            }

            // check if vessel is currently under construction with the presence of a root part
            if (EditorLogic.RootPart != null)
            {
                // set button state based on existing visibility
                IsOn = BuildAdvanced.Instance.Visible;
            }
            else
            {
                Disable();
            }
        }
    }
}