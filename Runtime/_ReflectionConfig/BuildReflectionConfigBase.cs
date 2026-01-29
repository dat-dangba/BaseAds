using UnityEngine;
using System.Reflection;
using System.Linq;

namespace DBD.Ads
{
    public abstract class BuildReflectionConfigBase : ScriptableObject
    {
        public FieldInfo[] GetBuildFields()
        {
            return GetType()
                .GetFields(BindingFlags.Public | BindingFlags.Instance)
                .Where(f => f.IsDefined(typeof(BuildKeyAttribute), false))
                .ToArray();
        }

        public string GetKey(FieldInfo field)
        {
            var attr = field.GetCustomAttribute<BuildKeyAttribute>();
            return string.IsNullOrEmpty(attr.customKey)
                ? field.Name
                : attr.customKey;
        }

        public bool IsAddKeyConfig(FieldInfo field)
        {
            var attr = field.GetCustomAttribute<BuildKeyAttribute>();
            return attr.isAddKeyConfig;
        }

        public bool IsManifestPlaceHolder(FieldInfo field)
        {
            var attr = field.GetCustomAttribute<BuildKeyAttribute>();
            return attr.isManifestPlaceHolder;
        }
    }
}