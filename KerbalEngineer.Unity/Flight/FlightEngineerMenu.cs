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
    using KerbalEngineer.Flight;
    using UnityEngine;
    using UnityEngine.EventSystems;

    public class FlightEngineerMenu : CanvasGroupFader, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField]
        private float m_FastFadeDuration = 0.2f;

        [SerializeField]
        private float m_SlowFadeDuration = 1.0f;

        public void OnPointerEnter(PointerEventData eventData)
        {
            FadeIn();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            // slow-fade out if the application launcher button is off
            if (FlightAppLauncher.instance != null && FlightAppLauncher.instance.isOn == false)
            {
                FadeTo(0.0f, m_SlowFadeDuration, Destroy);
            }
        }

        protected override void Awake()
        {
            base.Awake();

            // subscribe events
            FlightAppLauncher.MenuClosed += MenuClosed;
            FlightAppLauncher.ButtonHover += ButtonHover;
        }

        protected virtual void OnDestroy()
        {
            // unsubscribe events
            FlightAppLauncher.MenuClosed -= MenuClosed;
            FlightAppLauncher.ButtonHover -= ButtonHover;
        }

        protected virtual void OnEnable()
        {
            // set starting alpha to zero and fade in
            SetAlpha(0.0f);
            FadeIn();
        }

        /// <summary>
        ///     Called when the application launcher button is hovered over.
        /// </summary>
        private void ButtonHover()
        {
            FadeIn();
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

        private void FadeIn()
        {
            FadeTo(1.0f, m_FastFadeDuration);
        }

        private void MenuClosed()
        {
            FadeTo(0.0f, m_FastFadeDuration, Destroy);
        }
    }
}