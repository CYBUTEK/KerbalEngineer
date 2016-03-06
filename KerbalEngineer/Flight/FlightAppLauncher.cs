namespace KerbalEngineer.Flight
{
    using UnityEngine;

    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class FlightAppLauncher : AppLauncherButton
    {
        private GameObject m_MenuObject;
        private GameObject m_MenuPrefab;

        protected override void Awake()
        {
            base.Awake();

            if (m_MenuPrefab == null && AssetBundleLoader.prefabs != null)
            {
                m_MenuPrefab = AssetBundleLoader.prefabs.LoadAsset<GameObject>("flight-menu");
            }
        }

        protected override void OnFalse()
        {
            if (m_MenuObject == null)
            {
                return;
            }

            Destroy(m_MenuObject);
        }

        protected override void OnTrue()
        {
            if (m_MenuPrefab == null)
            {
                return;
            }

            m_MenuObject = Instantiate(m_MenuPrefab, GetAnchor(), Quaternion.identity) as GameObject;
            if (m_MenuObject == null)
            {
                return;
            }

            m_MenuObject.transform.SetParent(MainCanvasUtil.MainCanvas.transform);
        }
    }
}