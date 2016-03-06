namespace KerbalEngineer
{
    using UnityEngine;

    [KSPAddon(KSPAddon.Startup.Instantly, false)]
    public class AssetBundleLoader : MonoBehaviour
    {
        private static AssetBundle m_Images;
        private static AssetBundle m_Prefabs;

        /// <summary>
        ///     Gets the loaded images asset bundle.
        /// </summary>
        public static AssetBundle images
        {
            get
            {
                return m_Images;
            }
        }

        /// <summary>
        ///     Gets the loaded prefabs asset bundle.
        /// </summary>
        public static AssetBundle prefabs
        {
            get
            {
                return m_Prefabs;
            }
        }

        protected virtual void Awake()
        {
            string bundlePath = EngineerGlobals.AssemblyPath;

            m_Images = AssetBundle.CreateFromFile(bundlePath + "/images");
            m_Prefabs = AssetBundle.CreateFromFile(bundlePath + "/prefabs");
        }
    }
}