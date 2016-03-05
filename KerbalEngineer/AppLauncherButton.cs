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

namespace KerbalEngineer
{
    using KSP.UI.Screens;
    using UnityEngine;

    public class AppLauncherButton : MonoBehaviour
    {
        public enum ButtonState
        {
            Disabled,
            On,
            Off
        }

        private ApplicationLauncherButton m_Button;

        /// <summary>
        ///     Sets the state of the button.
        /// </summary>
        public void SetState(ButtonState state)
        {
            if (m_Button == null)
            {
                return;
            }

            switch (state)
            {
                case ButtonState.Disabled:
                    Disable();
                    break;

                case ButtonState.On:
                    Enable();
                    m_Button.SetTrue();
                    break;

                case ButtonState.Off:
                    Enable();
                    m_Button.SetFalse();
                    break;
            }
        }

        protected virtual void Awake()
        {
            // subscribe event listeners
            GameEvents.onGUIApplicationLauncherReady.Add(OnGUIApplicationLauncherReady);
            GameEvents.onGUIApplicationLauncherUnreadifying.Add(OnGUIApplicationLauncherUnreadifying);
        }

        protected virtual void OnDestroy()
        {
            // unsubscribe event listeners
            GameEvents.onGUIApplicationLauncherReady.Remove(OnGUIApplicationLauncherReady);
            GameEvents.onGUIApplicationLauncherUnreadifying.Remove(OnGUIApplicationLauncherUnreadifying);
        }

        /// <summary>
        ///     Called on button being disabled.
        /// </summary>
        protected virtual void OnDisable() { }

        /// <summary>
        ///     Called on button being enabled.
        /// </summary>
        protected virtual void OnEnable() { }

        /// <summary>
        ///     Called on button being toggled off.
        /// </summary>
        protected virtual void OnFalse() { }

        /// <summary>
        ///     Called on mouse hovering.
        /// </summary>
        protected virtual void OnHover() { }

        /// <summary>
        ///     Called on mouse exiting hover.
        /// </summary>
        protected virtual void OnHoverOut() { }

        /// <summary>
        ///     Called on button being ready.
        /// </summary>
        protected virtual void OnReady() { }

        /// <summary>
        ///     Called after the application launcher is ready and the button created.
        /// </summary>
        protected virtual void OnTrue() { }

        /// <summary>
        ///     Called after the application launcher is unreadified and the button removed.
        /// </summary>
        protected virtual void OnUnreadifying() { }

        /// <summary>
        ///     Disables the button if not already disabled.
        /// </summary>
        private void Disable()
        {
            if (m_Button != null && m_Button.toggleButton.Button.interactable)
            {
                m_Button.Disable();
            }
        }

        /// <summary>
        ///     Enables the button if not already enabled.
        /// </summary>
        private void Enable()
        {
            if (m_Button != null && m_Button.toggleButton.Button.interactable == false)
            {
                m_Button.Enable();
            }
        }

        private void OnGUIApplicationLauncherReady()
        {
            // create button
            if (ApplicationLauncher.Instance != null)
            {
                Texture iconTexture = GameDatabase.Instance.GetTexture("KerbalEngineer/Textures/ToolbarIcon", false);
                m_Button = ApplicationLauncher.Instance.AddModApplication(OnTrue, OnFalse, OnHover, OnHoverOut, OnEnable, OnDisable, ApplicationLauncher.AppScenes.ALWAYS, iconTexture);
            }

            OnReady();
        }

        private void OnGUIApplicationLauncherUnreadifying(GameScenes scene)
        {
            // remove button
            if (ApplicationLauncher.Instance != null && m_Button != null)
            {
                ApplicationLauncher.Instance.RemoveModApplication(m_Button);
            }

            OnUnreadifying();
        }
    }
}