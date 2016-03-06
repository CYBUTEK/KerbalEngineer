using UnityEditor;
using UnityEngine;

public class BuildAssetBundles : MonoBehaviour
{
    [MenuItem("Assets/Build Asset Bundles")]
    public static void Build()
    {
        BuildPipeline.BuildAssetBundles(Application.dataPath + "/../Output/KerbalEngineer", BuildAssetBundleOptions.UncompressedAssetBundle);
    }
}