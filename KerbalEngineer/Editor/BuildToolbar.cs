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

using UnityEngine;

#endregion

namespace KerbalEngineer.Editor
{
    [KSPAddon(KSPAddon.Startup.EditorAny, false)]
    public class BuildToolbar : MonoBehaviour
    {
        #region Fields

        private ApplicationLauncherButton button;

        #endregion

        #region Methods: private

        private void Awake()
        {
            GameEvents.onGUIApplicationLauncherReady.Add(this.OnGuiAppLauncherReady);
            Logger.Log("BuildToolbar->Awake");
        }

        private void Start()
        {
            if (button == null)
            {
                OnGuiAppLauncherReady();
            }
        }

        private void OnDestroy()
        {
            GameEvents.onGUIApplicationLauncherReady.Remove(this.OnGuiAppLauncherReady);
            if (this.button != null)
            {
                ApplicationLauncher.Instance.RemoveModApplication(this.button);
            }
            Logger.Log("BuildToolbar->OnDestroy");
        }

        private void OnGuiAppLauncherReady()
        {
            try
            {
                this.button = ApplicationLauncher.Instance.AddModApplication(
                    () => BuildAdvanced.Instance.Visible = true,
                    () => BuildAdvanced.Instance.Visible = false,
                    null,
                    null,
                    null,
                    null,
                    ApplicationLauncher.AppScenes.ALWAYS,
                    GameDatabase.Instance.GetTexture("KerbalEngineer/Textures/ToolbarIcon", false)
                    );
                Logger.Log("BuildToolbar->OnGuiAppLauncherReady");
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "BuildToolbar->OnGuiAppLauncherReady");
            }
        }

        private void Update()
        {
            try
            {
                if (this.button == null)
                {
                    return;
                }

                if (EditorLogic.fetch != null && EditorLogic.fetch.ship.parts.Count > 0)
                {
                    if (BuildAdvanced.Instance.Visible && this.button.State != RUIToggleButton.ButtonState.TRUE)
                    {
                        this.button.SetTrue();
                    }
                    else if (!BuildAdvanced.Instance.Visible && this.button.State != RUIToggleButton.ButtonState.FALSE)
                    {
                        this.button.SetFalse();
                    }
                }
                else if (this.button.State != RUIToggleButton.ButtonState.DISABLED)
                {
                    this.button.Disable();
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "BuildToolbar->Update");
            }
        }

        #endregion
    }
}