using UnityEngine;
using System.Reflection;
using System.Linq;

public abstract class BuildReflectionConfigBase : ScriptableObject
{
    // public bool enable = true;

    public void ApplyAndroid(string gradlePath)
    {
#if UNITY_ANDROID
        foreach (var field in GetBuildFields())
        {
            string key = GetKey(field);
            string value = ConvertValue(field.GetValue(this));

            if (string.IsNullOrEmpty(value)) continue;

            AndroidManifestUtil.AddOrUpdateMetaData(
                gradlePath,
                key,
                value
            );
        }
#endif
    }

    public void ApplyIOS(string iosPath)
    {
#if UNITY_EDITOR && UNITY_IOS
        foreach (var field in GetBuildFields())
        {
            string key = GetKey(field);
            object rawValue = field.GetValue(this);

            if (rawValue == null) continue;

            IOSPlistUtil.AddOrUpdate(
                iosPath,
                key,
                rawValue
            );
        }
#endif
    }

    // ----------------------

    private FieldInfo[] GetBuildFields()
    {
        return GetType()
            .GetFields(BindingFlags.Public | BindingFlags.Instance)
            .Where(f => f.IsDefined(typeof(BuildKeyAttribute), false))
            .ToArray();
    }

    private string GetKey(FieldInfo field)
    {
        var attr = field.GetCustomAttribute<BuildKeyAttribute>();
        return string.IsNullOrEmpty(attr.customKey)
            ? field.Name
            : attr.customKey;
    }

    private string ConvertValue(object value)
    {
        if (value == null) return null;

        switch (value)
        {
            case bool b: return b ? "true" : "false";
            case int i: return i.ToString();
            case float f: return f.ToString(System.Globalization.CultureInfo.InvariantCulture);
            case string s: return s;
            default:
                Debug.LogWarning($"Unsupported build value type: {value.GetType()}");
                return null;
        }
    }
}