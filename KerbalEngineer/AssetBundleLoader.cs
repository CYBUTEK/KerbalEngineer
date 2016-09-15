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
    using System.IO;
    using UnityEngine;

    [KSPAddon(KSPAddon.Startup.Instantly, false)]
    public class AssetBundleLoader : MonoBehaviour
    {
        private static AssetBundle images;
        private static AssetBundle prefabs;

        /// <summary>
        ///     Gets the loaded images asset bundle.
        /// </summary>
        public static AssetBundle Images
        {
            get
            {
                return images;
            }
        }

        /// <summary>
        ///     Gets the loaded prefabs asset bundle.
        /// </summary>
        public static AssetBundle Prefabs
        {
            get
            {
                return prefabs;
            }
        }

        protected virtual void Start()
        {
            string bundlePath = EngineerGlobals.AssemblyPath + "/AssetBundles/";

            images = AssetBundle.LoadFromFile(bundlePath + "/images");
            prefabs = AssetBundle.LoadFromFile(bundlePath + "/prefabs");

            MyLogger.Log(images);
        }
    }
}