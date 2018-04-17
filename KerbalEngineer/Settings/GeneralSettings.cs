namespace KerbalEngineer.Settings
{
    using UnityEngine;

    [KSPAddon(KSPAddon.Startup.Instantly, false)]
    public class GeneralSettings : MonoBehaviour
    {
        private readonly string fileName = "GeneralSettings.xml";

        public static SettingHandler Handler { get; private set; }

        public static GeneralSettings Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void OnDisable()
        {
            if (Handler != null)
            {
                Handler.Save(fileName);
            }
        }

        private void OnEnable()
        {
            Handler = SettingHandler.Load(fileName);
        }
    }
}