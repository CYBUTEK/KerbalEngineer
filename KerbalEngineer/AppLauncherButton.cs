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
    using KSP.UI;
    using KSP.UI.Screens;
    using UnityEngine;

    public class AppLauncherButton : MonoBehaviour
    {
        private static Texture s_IconTexture;
        private ApplicationLauncherButton m_Button;

        /// <summary>
        ///     Gets the wrapped application launcher button object.
        /// </summary>
        public ApplicationLauncherButton Button
        {
            get
            {
                return m_Button;
            }
        }

        /// <summary>
        ///     Gets or sets the toggle button state.
        /// </summary>
        public bool IsOn
        {
            get
            {
                return m_Button != null &&
                       m_Button.IsEnabled &&
                       m_Button.toggleButton.CurrentState == UIRadioButton.State.True;
            }
            set
            {
                if (m_Button == null)
                {
                    return;
                }

                if (value)
                {
                    SetOn();
                }
                else
                {
                    SetOff();
                }
            }
        }

        /// <summary>
        ///     Disables the button if not already disabled.
        /// </summary>
        public void Disable()
        {
            if (m_Button != null && m_Button.IsEnabled)
            {
                m_Button.Disable();
            }
        }

        /// <summary>
        ///     Enables the button if not already enabled.
        /// </summary>
        public void Enable()
        {
            if (m_Button != null && !m_Button.IsEnabled)
            {
                m_Button.Enable();
            }
        }

        /// <summary>
        ///     Gets the anchor position for pop-up content.
        /// </summary>
        public Vector3 GetAnchor()
        {
            if (m_Button == null)
            {
                return Vector3.zero;
            }

            Vector3 anchor = m_Button.GetAnchor();

            anchor.x -= 3.0f;

            return anchor;
        }

        /// <summary>
        ///     Enables and sets the button to off.
        /// </summary>
        public void SetOff()
        {
            Enable();

            if (m_Button != null && m_Button.toggleButton.CurrentState != UIRadioButton.State.False)
            {
                m_Button.SetFalse();
            }
        }

        /// <summary>
        ///     Enables and sets the button to on.
        /// </summary>
        public void SetOn()
        {
            Enable();

            if (m_Button != null && m_Button.toggleButton.CurrentState != UIRadioButton.State.True)
            {
                m_Button.SetTrue();
            }
        }

        protected virtual void Awake()
        {
            // cache icon texture
            if (s_IconTexture == null && AssetBundleLoader.Images != null)
            {
                s_IconTexture = AssetBundleLoader.Images.LoadAsset<Texture2D>("app-launcher-icon");
            }

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

        private void OnGUIApplicationLauncherReady()
        {
            // create button
            if (ApplicationLauncher.Instance != null)
            {
                m_Button = ApplicationLauncher.Instance.AddModApplication(OnTrue, OnFalse, OnHover, OnHoverOut, OnEnable, OnDisable, ApplicationLauncher.AppScenes.ALWAYS, s_IconTexture);
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