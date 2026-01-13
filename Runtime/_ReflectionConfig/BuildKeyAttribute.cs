using System;

[AttributeUsage(AttributeTargets.Field)]
public class BuildKeyAttribute : Attribute
{
    public readonly string customKey;

    public BuildKeyAttribute(string customKey = null)
    {
        this.customKey = customKey;
    }
}