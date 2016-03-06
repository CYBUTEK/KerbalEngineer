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
    using System;
    using UnityEngine;

    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class FlightAppLauncher : AppLauncherButton
    {
        private static FlightAppLauncher m_Instance;
        private GameObject m_MenuObject;
        private GameObject m_MenuPrefab;

        public static event Action ButtonHover;
        public static event Action MenuClosed;

        /// <summary>
        ///     Gets the current instance of the FlightAppLauncher object.
        /// </summary>
        public static FlightAppLauncher instance
        {
            get
            {
                return m_Instance;
            }
        }

        protected override void Awake()
        {
            base.Awake();

            // set singleton instance
            m_Instance = this;

            // cache menu prefab
            if (m_MenuPrefab == null && AssetBundleLoader.prefabs != null)
            {
                m_MenuPrefab = AssetBundleLoader.prefabs.LoadAsset<GameObject>("FlightMenu");
            }
        }

        protected override void OnFalse()
        {
            Close();
        }

        protected override void OnHover()
        {
            Open();
            ButtonHover?.Invoke();
        }

        protected override void OnHoverOut()
        {
            if (isOn == false)
            {
                Close();
            }
        }

        protected override void OnTrue()
        {
            Open();
        }

        /// <summary>
        ///     Closes the menu.
        /// </summary>
        private void Close()
        {
            if (m_MenuObject == null)
            {
                return;
            }

            if (MenuClosed != null)
            {
                MenuClosed();
            }
            else
            {
                Destroy(m_MenuObject);
            }
        }

        /// <summary>
        ///     Opens the menu.
        /// </summary>
        private void Open()
        {
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

            // set object as a child of the main canvas
            m_MenuObject.transform.SetParent(MainCanvasUtil.MainCanvas.transform);
        }
    }
}