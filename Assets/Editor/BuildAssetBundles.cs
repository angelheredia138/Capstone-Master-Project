using UnityEditor;
using UnityEngine;

public class BuildAssetBundles
{
    [MenuItem("Assets/Build AssetBundles/iOS Only")]
    public static void BuildAssetBundlesForIOS()
    {
        string assetBundleDirectory = "Assets/AssetBundles/iOS";
        if (!System.IO.Directory.Exists(assetBundleDirectory))
        {
            System.IO.Directory.CreateDirectory(assetBundleDirectory);
        }

        BuildPipeline.BuildAssetBundles(
            assetBundleDirectory,
            BuildAssetBundleOptions.None,
            BuildTarget.iOS
        );

        Debug.Log("Asset Bundles built for iOS.");
    }
}
