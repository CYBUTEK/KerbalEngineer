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

namespace KerbalEngineer.Flight
{
    using KSP.UI.Screens;

    /// <summary>
    ///     Graphical controller for section interaction in the form of a menu system.
    /// </summary>
    //[KSPAddon(KSPAddon.Startup.Flight, false)]
    public class ActionMenu : MonoBehaviour
    {
        #region Fields

        private ActionMenuGui actionMenuGui;
        private ApplicationLauncherButton button;

        #endregion

        #region Methods: protected

        protected void Awake()
        {
            try
            {
                GameEvents.onGUIApplicationLauncherReady.Add(this.OnGuiAppLauncherReady);
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
            Logger.Log("ActionMenu was created.");
        }

        protected void Start()
        {
            if (button == null)
            {
                OnGuiAppLauncherReady();
            }
        }

        protected void OnDestroy()
        {
            try
            {
                GameEvents.onGUIApplicationLauncherReady.Remove(this.OnGuiAppLauncherReady);
                GameEvents.onHideUI.Remove(this.OnHide);
                GameEvents.onShowUI.Remove(this.OnShow);
                if (button != null)
                {
                    ApplicationLauncher.Instance.RemoveModApplication(this.button);
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
            Logger.Log("ActionMenu was destroyed.");
        }

        protected void Update()
        {
            try
            {
                if (this.button == null)
                {
                    return;
                }
                if (FlightEngineerCore.IsDisplayable && this.button.toggleButton.Interactable == false)
                {
                    this.button.Enable();
                }
                else if (!FlightEngineerCore.IsDisplayable && this.button.toggleButton.Interactable)
                {
                    this.button.Disable();
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }

        #endregion

        #region Methods: private

        private void OnFalse()
        {
            try
            {
                this.actionMenuGui.enabled = false;
                this.actionMenuGui.StayOpen = false;
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }

        private void OnGuiAppLauncherReady()
        {
            try
            {
                this.button = ApplicationLauncher.Instance.AddModApplication(
                    this.OnTrue,
                    this.OnFalse,
                    this.OnHover,
                    this.OnHoverOut,
                    null,
                    null,
                    ApplicationLauncher.AppScenes.ALWAYS,
                    GameDatabase.Instance.GetTexture("KerbalEngineer/Textures/ToolbarIcon", false));
                this.actionMenuGui = this.button.gameObject.AddComponent<ActionMenuGui>();
                this.actionMenuGui.transform.parent = this.button.transform;
                ApplicationLauncher.Instance.EnableMutuallyExclusive(this.button);
                GameEvents.onHideUI.Add(this.OnHide);
                GameEvents.onShowUI.Add(this.OnShow);
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }

        private void OnHide()
        {
            try
            {
                this.actionMenuGui.Hidden = true;
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }

        private void OnHover()
        {
            try
            {
                this.actionMenuGui.enabled = true;
                this.actionMenuGui.Hovering = true;
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }

        private void OnHoverOut()
        {
            try
            {
                this.actionMenuGui.Hovering = false;
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }

        private void OnShow()
        {
            try
            {
                this.actionMenuGui.Hidden = false;
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }

        private void OnTrue()
        {
            try
            {
                this.actionMenuGui.enabled = true;
                this.actionMenuGui.StayOpen = true;
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }

        #endregion
    }
}