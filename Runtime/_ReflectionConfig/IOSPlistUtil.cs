#if UNITY_EDITOR && UNITY_IOS
using UnityEditor.iOS.Xcode;
using System.IO;

public static class IOSPlistUtil
{
    public static void AddOrUpdate(
        string iosBuildPath,
        string key,
        object value
    )
    {
        string plistPath = Path.Combine(iosBuildPath, "Info.plist");

        var plist = new PlistDocument();
        plist.ReadFromFile(plistPath);

        var root = plist.root;

        switch (value)
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
                UnityEngine.Debug.LogWarning(
                    $"Unsupported plist value type: {value.GetType()}"
                );
                break;
        }

        File.WriteAllText(plistPath, plist.WriteToString());
    }
}
#endif