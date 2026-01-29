#if UNITY_EDITOR && UNITY_ANDROID
using System.IO;
using System.Xml;
using UnityEditor.Android;
using UnityEngine;

namespace DBD.Ads.Editor
{
    public class AndroidManifestMerger : IPostGenerateGradleAndroidProject
    {
        private const string ANDROID_NS = "http://schemas.android.com/apk/res/android";

        private static void ApplyAndroid(XmlDocument xml, BuildReflectionConfigBase config)
        {
            foreach (var field in config.GetBuildFields())
            {
                bool isAddKeyConfig = config.IsAddKeyConfig(field);
                if (!isAddKeyConfig) continue;

                string key = config.GetKey(field);
                string value = ConvertValue(field.GetValue(config));

                if (string.IsNullOrEmpty(value)) continue;

                var manifest = xml.SelectSingleNode("/manifest");
                var application = manifest.SelectSingleNode("application");

                foreach (XmlNode node in application.SelectNodes("meta-data"))
                {
                    if (node.Attributes["android:name"]?.Value == key)
                    {
                        node.Attributes["android:value"].Value = value;
                        return;
                    }
                }

                var meta = xml.CreateElement("meta-data");
                meta.SetAttribute("name", ANDROID_NS, key);
                meta.SetAttribute("value", ANDROID_NS, value);
                application.AppendChild(meta);
            }
        }

        private static string ConvertValue(object value)
        {
            if (value == null) return null;

            return value switch
            {
                bool b => b ? "true" : "false",
                int i => i.ToString(),
                float f => f.ToString(System.Globalization.CultureInfo.InvariantCulture),
                string s => s,
                _ => value.ToString()
            };
        }

        public int callbackOrder { get; }

        public void OnPostGenerateGradleAndroidProject(string pathToBuiltProject)
        {
            string manifestPath;
            if (pathToBuiltProject.Contains("unityLibrary"))
            {
                var pathProject = pathToBuiltProject.Replace("unityLibrary", "");
                manifestPath = Path.Combine(pathProject, "launcher/src/main/AndroidManifest.xml");
            }
            else if (pathToBuiltProject.Contains("launcher"))
            {
                manifestPath = Path.Combine(pathToBuiltProject, "src/main/AndroidManifest.xml");
            }
            else
            {
                manifestPath = Path.Combine(pathToBuiltProject, "launcher/src/main/AndroidManifest.xml");
            }

            var xml = new XmlDocument();
            xml.Load(manifestPath);

            var configs = Resources.LoadAll<BuildReflectionConfigBase>("");
            foreach (var cfg in configs)
            {
                ApplyAndroid(xml, cfg);
            }

            xml.Save(manifestPath);
        }
    }
}
#endif