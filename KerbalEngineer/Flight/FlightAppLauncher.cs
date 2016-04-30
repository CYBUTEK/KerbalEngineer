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

namespace KerbalEngineer.Flight
{
    using System.Collections.Generic;
    using KSP.UI;
    using Sections;
    using Unity.Flight;
    using UnityEngine;

    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class FlightAppLauncher : AppLauncherButton, IFlightAppLauncher
    {
        private static FlightAppLauncher s_Instance;
        private FlightMenu m_FlightMenu;
        private GameObject m_MenuObject;
        private GameObject m_MenuPrefab;

        /// <summary>
        ///     Gets the current instance of the FlightAppLauncher object.
        /// </summary>
        public static FlightAppLauncher Instance
        {
            get
            {
                return s_Instance;
            }
        }

        /// <summary>
        ///     Applies the KSP theme to a game object and its children.
        /// </summary>
        public void ApplyTheme(GameObject gameObject)
        {
            StyleManager.Process(gameObject);
        }

        /// <summary>
        ///     Clamps the given rect transform within the screen bounds.
        /// </summary>
        public void ClampToScreen(RectTransform rectTransform)
        {
            UIMasterController.ClampToScreen(rectTransform, Vector2.zero);
        }

        /// <summary>
        ///     Gets a list of custom sections.
        /// </summary>
        IList<ISectionModule> IFlightAppLauncher.GetCustomSections()
        {
            return new List<ISectionModule>(SectionLibrary.CustomSections.ToArray());
        }

        /// <summary>
        ///     Gets a list of stock sections.
        /// </summary>
        IList<ISectionModule> IFlightAppLauncher.GetStockSections()
        {
            return new List<ISectionModule>(SectionLibrary.StockSections.ToArray());
        }

        /// <summary>
        ///     Gets or sets the control bar's visibility.
        /// </summary>
        public bool IsControlBarVisible
        {
            get
            {
                if (DisplayStack.Instance != null)
                {
                    return DisplayStack.Instance.ShowControlBar;
                }

                return false;
            }
            set
            {
                if (DisplayStack.Instance != null)
                {
                    DisplayStack.Instance.ShowControlBar = value;
                }
            }
        }

        /// <summary>
        ///     Gets or sets the display stack's visibility.
        /// </summary>
        public bool IsDisplayStackVisible
        {
            get
            {
                if (DisplayStack.Instance != null)
                {
                    return DisplayStack.Instance.Hidden == false;
                }

                return false;
            }
            set
            {
                if (DisplayStack.Instance != null)
                {
                    DisplayStack.Instance.Hidden = !value;
                }
            }
        }

        /// <summary>
        ///     Creates and initialises a new custom section.
        /// </summary>
        public ISectionModule NewCustomSection()
        {
            SectionModule section = new SectionModule
            {
                Name = "Custom " + (SectionLibrary.CustomSections.Count + 1),
                Abbreviation = "CUST " + (SectionLibrary.CustomSections.Count + 1),
                IsVisible = true,
                IsCustom = true,
                IsEditorVisible = true
            };

            SectionLibrary.CustomSections.Add(section);

            return section;
        }

        protected override void Awake()
        {
            base.Awake();

            // set singleton instance
            s_Instance = this;

            // cache menu prefab
            if (m_MenuPrefab == null && AssetBundleLoader.Prefabs != null)
            {
                m_MenuPrefab = AssetBundleLoader.Prefabs.LoadAsset<GameObject>("FlightMenu");
            }
        }

        protected override void OnFalse()
        {
            Close();
        }

        protected override void OnHover()
        {
            Open();
        }

        protected override void OnHoverOut()
        {
            if (IsOn == false)
            {
                Close();
            }
        }

        protected override void OnTrue()
        {
            Open();
        }

        protected virtual void Update()
        {
            if (FlightEngineerCore.IsDisplayable)
            {
                Enable();
            }
            else if (FlightEngineerCore.IsDisplayable == false)
            {
                Disable();
            }
        }

        /// <summary>
        ///     Closes the menu.
        /// </summary>
        private void Close()
        {
            if (m_FlightMenu != null)
            {
                m_FlightMenu.Close();
            }
            else if (m_MenuObject != null)
            {
                Destroy(m_MenuObject);
            }
        }

        /// <summary>
        ///     Opens the menu.
        /// </summary>
        private void Open()
        {
            // fade menu in if already open
            if (m_FlightMenu != null)
            {
                m_FlightMenu.FadeIn();
                return;
            }

            if (m_MenuPrefab == null || m_MenuObject != null)
            {
                return;
            }

            // create object
            m_MenuObject = Instantiate(m_MenuPrefab, GetAnchor(), Quaternion.identity) as GameObject;
            if (m_MenuObject == null)
            {
                return;
            }

            StyleManager.Process(m_MenuObject);

            // set object as a child of the main canvas
            m_MenuObject.transform.SetParent(MainCanvasUtil.MainCanvas.transform);

            // set menu's reference to this object for cross-communication
            m_FlightMenu = m_MenuObject.GetComponent<FlightMenu>();
            if (m_FlightMenu != null)
            {
                m_FlightMenu.SetFlightAppLauncher(this);
            }
        }
    }
}