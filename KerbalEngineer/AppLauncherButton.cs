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
        private static Texture iconTexture;
        private ApplicationLauncherButton button;

        /// <summary>
        ///     Gets the wrapped application launcher button object.
        /// </summary>
        public ApplicationLauncherButton Button
        {
            get
            {
                return button;
            }
        }

        /// <summary>
        ///     Gets or sets the toggle button state.
        /// </summary>
        public bool IsOn
        {
            get
            {
                return button != null &&
                       button.IsEnabled &&
                       button.toggleButton.CurrentState == UIRadioButton.State.True;
            }
            set
            {
                if (button == null)
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
            if (button != null && button.IsEnabled)
            {
                button.Disable();
            }
        }

        /// <summary>
        ///     Enables the button if not already enabled.
        /// </summary>
        public void Enable()
        {
            if (button != null && !button.IsEnabled)
            {
                button.Enable();
            }
        }

        /// <summary>
        ///     Gets the anchor position for pop-up content.
        /// </summary>
        public Vector3 GetAnchor()
        {
            if (button == null)
            {
                return Vector3.zero;
            }

            Vector3 anchor = button.GetAnchor();

            anchor.x -= 3.0f;

            return anchor;
        }

        /// <summary>
        ///     Enables and sets the button to off.
        /// </summary>
        public void SetOff()
        {
            Enable();

            if (button != null && button.toggleButton.CurrentState != UIRadioButton.State.False)
            {
                button.SetFalse();
            }
        }

        /// <summary>
        ///     Enables and sets the button to on.
        /// </summary>
        public void SetOn()
        {
            Enable();

            if (button != null && button.toggleButton.CurrentState != UIRadioButton.State.True)
            {
                button.SetTrue();
            }
        }

        protected virtual void Awake()
        {
            // cache icon texture
            if (iconTexture == null && AssetBundleLoader.Images != null)
            {
                iconTexture = AssetBundleLoader.Images.LoadAsset<Texture2D>("app-launcher-icon");
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
                button = ApplicationLauncher.Instance.AddModApplication(OnTrue, OnFalse, OnHover, OnHoverOut, OnEnable, OnDisable, ApplicationLauncher.AppScenes.ALWAYS, iconTexture);
            }

            OnReady();
        }

        private void OnGUIApplicationLauncherUnreadifying(GameScenes scene)
        {
            // remove button
            if (ApplicationLauncher.Instance != null && button != null)
            {
                ApplicationLauncher.Instance.RemoveModApplication(button);
            }

            OnUnreadifying();
        }
    }
}