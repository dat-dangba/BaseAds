using System;

namespace DBD.Ads
{
    [AttributeUsage(AttributeTargets.Field)]
    public class BuildKeyAttribute : Attribute
    {
        public readonly string customKey;

        public BuildKeyAttribute(string customKey = null)
        {
            this.customKey = customKey;
        }
    }
}