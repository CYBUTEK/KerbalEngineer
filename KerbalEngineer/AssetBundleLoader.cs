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
    using UnityEngine;

    [KSPAddon(KSPAddon.Startup.Instantly, false)]
    public class AssetBundleLoader : MonoBehaviour
    {
        private static AssetBundle s_Images;
        private static AssetBundle s_Prefabs;

        /// <summary>
        ///     Gets the loaded images asset bundle.
        /// </summary>
        public static AssetBundle Images
        {
            get
            {
                return s_Images;
            }
        }

        /// <summary>
        ///     Gets the loaded prefabs asset bundle.
        /// </summary>
        public static AssetBundle Prefabs
        {
            get
            {
                return s_Prefabs;
            }
        }

        protected virtual void Awake()
        {
            string bundlePath = EngineerGlobals.AssemblyPath;

            s_Images = AssetBundle.CreateFromFile(bundlePath + "/images");
            s_Prefabs = AssetBundle.CreateFromFile(bundlePath + "/prefabs");
        }
    }
}