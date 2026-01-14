#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Build;

namespace DBD.Ads.Editor
{
    public class PlatformChangeListener : IActiveBuildTargetChanged
    {
        public int callbackOrder { get; }

        public void OnActiveBuildTargetChanged(BuildTarget previousTarget, BuildTarget newTarget)
        {
            Config(newTarget);
        }

        private void Config(BuildTarget newTarget)
        {
            AdsConfig config = LoadAdsConfig();
            if (config == null) return;

            switch (newTarget)
            {
                case BuildTarget.Android:
                    config.ApplyAndroidConfig();
                    break;

                case BuildTarget.iOS:
                    config.ApplyIOSConfig();
                    break;
            }

            EditorUtility.SetDirty(config);
            AssetDatabase.SaveAssets();
        }

        private AdsConfig LoadAdsConfig()
        {
            string[] guids = AssetDatabase.FindAssets("t:AdsConfig");

            if (guids.Length == 0)
            {
                return null;
            }

            string path = AssetDatabase.GUIDToAssetPath(guids[0]);
            return AssetDatabase.LoadAssetAtPath<AdsConfig>(path);
        }
    }
}
#endif