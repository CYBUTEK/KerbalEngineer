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
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

    [RequireComponent(typeof(RectTransform))]
    public class FlightMenu : CanvasGroupFader, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField]
        private Toggle m_ShowEngineerToggle = null;

        [SerializeField]
        private Toggle m_ControlBarToggle = null;

        [SerializeField]
        private GameObject m_MenuSectionPrefab = null;

        [SerializeField]
        private Transform m_SectionsTransform = null;

        [SerializeField]
        private float m_FastFadeDuration = 0.2f;

        [SerializeField]
        private float m_SlowFadeDuration = 1.0f;

        private IFlightAppLauncher m_FlightAppLauncher;
        private RectTransform m_RectTransform;

        public void OnPointerEnter(PointerEventData eventData)
        {
            FadeIn();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            // slow-fade out if the application launcher button is off
            if (m_FlightAppLauncher != null && m_FlightAppLauncher.IsOn == false)
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
        ///     Creates a new custom section.
        /// </summary>
        public void NewCustomSection()
        {
            if (m_FlightAppLauncher != null)
            {
                CreateSectionControl(m_FlightAppLauncher.NewCustomSection());
            }
        }

        /// <summary>
        ///     Sets the control bar visiblity.
        /// </summary>
        public void SetControlBarVisible(bool visible)
        {
            if (m_FlightAppLauncher != null)
            {
                m_FlightAppLauncher.IsControlBarVisible = visible;
            }
        }

        /// <summary>
        ///     Sets the display stack visibility.
        /// </summary>
        public void SetDisplayStackVisible(bool visible)
        {
            if (m_FlightAppLauncher != null)
            {
                m_FlightAppLauncher.IsDisplayStackVisible = visible;
            }
        }

        /// <summary>
        ///     Sets a reference to the flight app launcher object.
        /// </summary>
        public void SetFlightAppLauncher(IFlightAppLauncher flightAppLauncher)
        {
            if (flightAppLauncher == null)
            {
                return;
            }

            m_FlightAppLauncher = flightAppLauncher;

            // create section controls
            CreateSectionControls(m_FlightAppLauncher.GetStockSections());
            CreateSectionControls(m_FlightAppLauncher.GetCustomSections());
        }

        protected override void Awake()
        {
            base.Awake();

            // cache components
            m_RectTransform = GetComponent<RectTransform>();
        }

        protected virtual void Start()
        {
            // set starting alpha to zero and fade in
            SetAlpha(0.0f);
            FadeIn();
        }

        protected virtual void Update()
        {
            if (m_FlightAppLauncher == null)
            {
                return;
            }

            // set toggle states to match the actual states
            SetToggle(m_ShowEngineerToggle, m_FlightAppLauncher.IsDisplayStackVisible);
            SetToggle(m_ControlBarToggle, m_FlightAppLauncher.IsControlBarVisible);

            // update anchor position
            if (m_RectTransform != null)
            {
                m_RectTransform.position = m_FlightAppLauncher.GetAnchor();
                m_FlightAppLauncher.ClampToScreen(m_RectTransform);
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
        ///     Creates a menu section control.
        /// </summary>
        private void CreateSectionControl(ISectionModule section)
        {
            GameObject menuSectionObject = Instantiate(m_MenuSectionPrefab);
            if (menuSectionObject != null)
            {
                // apply ksp theme to the created menu section object
                m_FlightAppLauncher.ApplyTheme(menuSectionObject);

                menuSectionObject.transform.SetParent(m_SectionsTransform, false);

                FlightMenuSection menuSection = menuSectionObject.GetComponent<FlightMenuSection>();
                if (menuSection != null)
                {
                    menuSection.SetAssignedSection(section);
                }
            }
        }

        /// <summary>
        ///     Creates a list of section controls from a given list of sections.
        /// </summary>
        private void CreateSectionControls(IList<ISectionModule> sections)
        {
            if (sections == null || m_MenuSectionPrefab == null || m_SectionsTransform == null)
            {
                return;
            }

            for (int i = 0; i < sections.Count; i++)
            {
                ISectionModule section = sections[i];
                if (section != null)
                {
                    CreateSectionControl(section);
                }
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