#if UNITY_ANDROID
using System.IO;
using System.Xml;

public static class AndroidManifestUtil
{
    private const string ANDROID_NS =
        "http://schemas.android.com/apk/res/android";

    public static void AddOrUpdateMetaData(
        string gradleProjectPath,
        string key,
        string value
    )
    {
        string manifestPath = Path.Combine(
            gradleProjectPath,
            "src/main/AndroidManifest.xml"
        );

        var xml = new XmlDocument();
        xml.Load(manifestPath);

        var manifest = xml.SelectSingleNode("/manifest");
        var application = manifest.SelectSingleNode("application");

        foreach (XmlNode node in application.SelectNodes("meta-data"))
        {
            if (node.Attributes["android:name"]?.Value == key)
            {
                node.Attributes["android:value"].Value = value;
                xml.Save(manifestPath);
                return;
            }
        }

        var meta = xml.CreateElement("meta-data");
        meta.SetAttribute("android:name", ANDROID_NS, key);
        meta.SetAttribute("android:value", ANDROID_NS, value);
        application.AppendChild(meta);

        xml.Save(manifestPath);
    }
}
#endif