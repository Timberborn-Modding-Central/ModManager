using System.IO;
using UnityEngine;

namespace ModManagerUI.UiSystem
{
    public abstract class AssetBundleLoader
    {
        private static readonly string BundleName = "modmanagerui.bundle";

        private static AssetBundle? _assetBundle;

        public static AssetBundle AssetBundle
        {
            get
            {
                if (_assetBundle == null)
                {
                    _assetBundle = AssetBundle.LoadFromFile($"{Path.Combine(UIPaths.ModManagerUI.Assets, BundleName)}");
                }
                
                return _assetBundle;
            }
        }
    }
}