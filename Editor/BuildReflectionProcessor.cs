#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.Android;
using System.Linq;

public class BuildReflectionProcessor :
    IPreprocessBuildWithReport,
    IPostGenerateGradleAndroidProject
{
    public int callbackOrder => 0;

    public void OnPreprocessBuild(BuildReport report)
    {
        if (report.summary.platform == BuildTarget.iOS)
            ApplyIOS(report.summary.outputPath);
    }

    public void OnPostGenerateGradleAndroidProject(string path)
    {
        ApplyAndroid(path);
    }

    private static void ApplyAndroid(string gradlePath)
    {
        foreach (var cfg in LoadConfigs())
        {
            cfg.ApplyAndroid(gradlePath);
        }
    }

    private static void ApplyIOS(string iosPath)
    {
        foreach (var cfg in LoadConfigs())
        {
            cfg.ApplyIOS(iosPath);
        }
    }

    private static BuildReflectionConfigBase[] LoadConfigs()
    {
        return AssetDatabase.FindAssets("t:AdConfig")
            .Select(guid =>
                AssetDatabase.LoadAssetAtPath<BuildReflectionConfigBase>(
                    AssetDatabase.GUIDToAssetPath(guid)
                )
            ).ToArray();
    }
}
#endif