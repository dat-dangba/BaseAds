#if UNITY_EDITOR && UNITY_ANDROID
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using UnityEditor.Android;
using UnityEngine;

namespace DBD.Ads.Editor
{
    public class AndroidManifestMerger : IPostGenerateGradleAndroidProject
    {
        private const string ANDROID_NS = "http://schemas.android.com/apk/res/android";

        public int callbackOrder { get; }

        public void OnPostGenerateGradleAndroidProject(string pathToBuiltProject)
        {
            var launcherPath = GetLauncherPath(pathToBuiltProject);

            var configDict = GetAllConfig();

            bool success = MergeBuildGradle(launcherPath, configDict);
            if (!success) return;
            MergeManifest(launcherPath, configDict);
        }

        private string GetLauncherPath(string pathToBuiltProject)
        {
            string launcherPath;

            if (pathToBuiltProject.Contains("unityLibrary"))
            {
                var pathProject = pathToBuiltProject.Replace("unityLibrary", "");
                launcherPath = pathProject + "/launcher";
                // manifestPath = Path.Combine(pathProject, "launcher/src/main/AndroidManifest.xml");
            }
            else if (pathToBuiltProject.Contains("launcher"))
            {
                launcherPath = pathToBuiltProject;
                // manifestPath = Path.Combine(pathToBuiltProject, "src/main/AndroidManifest.xml");
            }
            else
            {
                launcherPath = pathToBuiltProject + "/launcher";
                // manifestPath = Path.Combine(pathToBuiltProject, "launcher/src/main/AndroidManifest.xml");
            }

            return launcherPath;
        }

        private Dictionary<string, (string, bool)> GetAllConfig()
        {
            var configs = Resources.LoadAll<BuildReflectionConfigBase>("");
            Dictionary<string, (string, bool)> configDict = new Dictionary<string, (string, bool)>();

            foreach (var config in configs)
            {
                foreach (var field in config.GetBuildFields())
                {
                    bool isAddKeyConfig = config.IsAddKeyConfig(field);
                    if (!isAddKeyConfig) continue;

                    string key = config.GetKey(field);
                    string value = ConvertValue(field.GetValue(config));
                    bool isManifestPlaceHolder = config.IsManifestPlaceHolder(field);

                    if (string.IsNullOrEmpty(value)) continue;

                    configDict.Add(key, (value, isManifestPlaceHolder));
                }
            }

            return configDict;
        }

        private void MergeManifest(string launcherPath, Dictionary<string, (string, bool)> configDict)
        {
            string manifestPath = Path.Combine(launcherPath, "src/main/AndroidManifest.xml");

            if (!File.Exists(manifestPath))
            {
                Debug.LogError($"Không tìm thấy file manifest: {manifestPath}");
                return;
            }

            var xml = new XmlDocument();
            xml.Load(manifestPath);
            var manifest = xml.SelectSingleNode("/manifest");
            var application = manifest.SelectSingleNode("application");
            foreach (var (key, (value, isManifestPlaceHolder)) in configDict)
            {
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
                meta.SetAttribute("value", ANDROID_NS, isManifestPlaceHolder ? $"${{{key}}}" : value);
                application.AppendChild(meta);
            }

            xml.Save(manifestPath);
        }

        private bool MergeBuildGradle(string launcherPath, Dictionary<string, (string, bool)> configDict)
        {
            string buildGradlePath = Path.Combine(launcherPath, "build.gradle");

            if (!File.Exists(buildGradlePath))
            {
                Debug.LogError($"Không tìm thấy file buildGradle: {buildGradlePath}");
                return false;
            }

            string content = File.ReadAllText(buildGradlePath);
            var placeHolders = GetPlaceHolders(configDict);

            if (content.Contains("defaultConfig"))
            {
                int startOfDefaultConfig = content.IndexOf("defaultConfig", StringComparison.Ordinal);
                if (startOfDefaultConfig == -1)
                {
                    Debug.LogError("Không thể chèn manifestPlaceholders vào defaultConfig trong build.gradle");
                    return false;
                }

                int firstOpenBrace = content.IndexOf('{', startOfDefaultConfig);
                int lastCloseBrace = FindClosingBrace(content, firstOpenBrace);
                if (lastCloseBrace == -1)
                {
                    Debug.LogError("Không thể chèn manifestPlaceholders vào defaultConfig trong build.gradle");
                    return false;
                }

                content = content.Insert(lastCloseBrace, placeHolders);
                File.WriteAllText(buildGradlePath, content);
                Debug.Log("Đã chèn manifestPlaceholders vào defaultConfig thành công!");
                return true;
            }

            Debug.LogError("Không tìm thấy thẻ defaultConfig trong build.gradle");
            return false;
        }

        private int FindClosingBrace(string text, int openBraceIndex)
        {
            int count = 0;
            for (int i = openBraceIndex; i < text.Length; i++)
            {
                if (text[i] == '{') count++;
                else if (text[i] == '}')
                {
                    count--;
                    if (count == 0) return i;
                }
            }

            return -1;
        }

        private string GetPlaceHolders(Dictionary<string, (string, bool)> configDict)
        {
            var lines = configDict
                .Where(x => x.Value.Item2)
                .Select(x => $"\t\t\t{x.Key}: \"{x.Value.Item1}\"");
            string placeHolders = "\n\t\tmanifestPlaceholders = [\n" +
                                  string.Join(",\n", lines) +
                                  "\n\t\t]\n\t";
            return placeHolders;
        }

        private string ConvertValue(object value)
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
    }
}
#endif