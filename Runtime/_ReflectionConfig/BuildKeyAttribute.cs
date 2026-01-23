using System;

namespace DBD.Ads
{
    [AttributeUsage(AttributeTargets.Field)]
    public class BuildKeyAttribute : Attribute
    {
        public readonly string customKey;
        public readonly bool isAddKeyConfig;

        public BuildKeyAttribute(string customKey = null, bool isAddKeyConfig = true)
        {
            this.isAddKeyConfig = isAddKeyConfig;
            this.customKey = customKey;
        }

        public BuildKeyAttribute(bool isAddKeyConfig)
        {
            this.isAddKeyConfig = isAddKeyConfig;
        }
    }
}