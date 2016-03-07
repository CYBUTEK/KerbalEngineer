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

namespace KerbalEngineer.Unity.Flight
{
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

    public class FlightMenu : CanvasGroupFader, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField]
        private Toggle m_ShowEngineerToggle = null;

        [SerializeField]
        private Toggle m_ControlBarToggle = null;

        [SerializeField]
        private float m_FastFadeDuration = 0.2f;

        [SerializeField]
        private float m_SlowFadeDuration = 1.0f;

        private IFlightAppLauncher m_FlightAppLauncher;

        public void OnPointerEnter(PointerEventData eventData)
        {
            FadeIn();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            // slow-fade out if the application launcher button is off
            if (m_FlightAppLauncher != null && m_FlightAppLauncher.isOn == false)
            {
                FadeTo(0.0f, m_SlowFadeDuration, Destroy);
            }
        }

        /// <summary>
        ///     Fades out and destroys the menu.
        /// </summary>
        public void Close()
        {
            FadeTo(0.0f, m_FastFadeDuration, Destroy);
        }

        /// <summary>
        ///     Fades in the menu.
        /// </summary>
        public void FadeIn()
        {
            FadeTo(1.0f, m_FastFadeDuration);
        }

        /// <summary>
        ///     Sets the control bar visiblity.
        /// </summary>
        public void SetControlBar(bool visible)
        {
            if (m_FlightAppLauncher != null)
            {
                m_FlightAppLauncher.controlBar = visible;
            }
        }

        /// <summary>
        ///     Sets a reference to the flight app launcher object.
        /// </summary>
        public void SetFlightAppLauncher(IFlightAppLauncher flightAppLauncher)
        {
            m_FlightAppLauncher = flightAppLauncher;
        }

        /// <summary>
        ///     Sets the display stack visibility.
        /// </summary>
        public void SetShowEngineer(bool visible)
        {
            if (m_FlightAppLauncher != null)
            {
                m_FlightAppLauncher.showEngineer = visible;
            }
        }

        protected virtual void Start()
        {
            // set starting alpha to zero and fade in
            SetAlpha(0.0f);
            FadeIn();
        }

        protected virtual void Update()
        {
            // set toggle states to match the actual states
            if (m_FlightAppLauncher != null)
            {
                SetToggle(m_ShowEngineerToggle, m_FlightAppLauncher.showEngineer);
                SetToggle(m_ControlBarToggle, m_FlightAppLauncher.controlBar);
            }
        }

        /// <summary>
        ///     Sets a given toggle to the specified state with null checking.
        /// </summary>
        private static void SetToggle(Toggle toggle, bool state)
        {
            if (toggle != null)
            {
                toggle.isOn = state;
            }
        }

        /// <summary>
        ///     Destroys the game object.
        /// </summary>
        private void Destroy()
        {
            // disable game object first due to an issue within unity 5.2.4f1 that shows a single frame at full opaque alpha just before destruction
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
    }
}