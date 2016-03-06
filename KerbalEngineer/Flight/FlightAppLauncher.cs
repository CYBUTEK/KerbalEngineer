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