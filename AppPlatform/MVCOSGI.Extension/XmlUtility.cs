using System.Xml;

namespace UIShell.Extension
{
    public class XmlUtility
    {
        public static string ReadAttribute(XmlNode node, string attributeName)
        {
            return ReadAttribute(node.Attributes, attributeName);
        }

        public static string ReadAttribute(XmlAttributeCollection attributes, string attributeName)
        {
            XmlAttribute attr = attributes[attributeName];
            if (attr != null)
            {
                return attr.Value;
            }
            return string.Empty;
        }
    }
}