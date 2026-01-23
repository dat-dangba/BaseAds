#if UNITY_IOS && UNITY_EDITOR
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using System.IO;
using UnityEngine;

namespace DBD.Ads.Editor
{
    public static class InfoPlistMerger
    {
        [PostProcessBuild]
        public static void OnPostprocessBuild(BuildTarget buildTarget, string pathToBuiltProject)
        {
            if (buildTarget != BuildTarget.iOS)
                return;

            string plistPath = Path.Combine(pathToBuiltProject, "Info.plist");

            var plist = new PlistDocument();
            plist.ReadFromFile(plistPath);

            PlistElementDict root = plist.root;
            var configs = Resources.LoadAll<BuildReflectionConfigBase>("");
            foreach (var cfg in configs)
            {
                ApplyIOS(root, cfg);
            }

            File.WriteAllText(plistPath, plist.WriteToString());
        }

        private static void ApplyIOS(PlistElementDict root, BuildReflectionConfigBase config)
        {
            foreach (var field in config.GetBuildFields())
            {
                bool isAddKeyConfig = config.IsAddKeyConfig(field);
                if (!isAddKeyConfig) continue;

                string key = config.GetKey(field);
                object rawValue = field.GetValue(config);

                if (rawValue == null) continue;

                switch (rawValue)
                {
                    case bool b:
                        root.SetBoolean(key, b);
                        break;
                    case int i:
                        root.SetInteger(key, i);
                        break;
                    case float f:
                        root.SetReal(key, f);
                        break;
                    case string s:
                        root.SetString(key, s);
                        break;
                    default:
                        root.SetString(key, rawValue.ToString());
                        break;
                }
            }
        }
    }
}
#endif