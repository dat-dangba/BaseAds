using System;

namespace DBD.Ads
{
    [AttributeUsage(AttributeTargets.Field)]
    public class BuildKeyAttribute : Attribute
    {
        public readonly string customKey;
        public readonly bool isAddKeyConfig;
        public readonly bool isManifestPlaceHolder;

        public BuildKeyAttribute(string customKey = null, bool isAddKeyConfig = true, bool isManifestPlaceHolder = true)
        {
            this.isAddKeyConfig = isAddKeyConfig;
            this.customKey = customKey;
            this.isManifestPlaceHolder = isManifestPlaceHolder;
        }

        public BuildKeyAttribute(bool isAddKeyConfig, bool isManifestPlaceHolder)
        {
            this.isAddKeyConfig = isAddKeyConfig;
            this.isManifestPlaceHolder = isManifestPlaceHolder;
        }
    }
}