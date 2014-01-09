using System.Collections.Generic;
using System.Xml;
using MvcOSGi.Shell.Models;
using UIShell.Extension;
using UIShell.OSGi;

namespace MvcOSGi.Shell.Extension
{
    public class MenuItemParser
    {
        public static List<MenuItem> Build(XmlNodeList nodes, IBundle owner)
        {
            var result = new List<MenuItem>();
            for (int i = 0; i < nodes.Count; i++)
            {
                if (nodes[i].NodeType == XmlNodeType.Element)
                    result.Add(Build(nodes[i], owner));
            }
            return result;
        }

        public static List<MenuItem> Build(IEnumerable<XmlNode> nodes, IBundle owner)
        {
            var result = new List<MenuItem>();
            foreach (XmlNode item in nodes)
            {
                if (item.NodeType == XmlNodeType.Element)
                    result.Add(Build(item, owner));
            }
            return result;
        }

        public static MenuItem Build(XmlNode node, IBundle owner)
        {
            var result = new MenuItem
                            {
                                URL = XmlUtility.ReadAttribute(node, "url"),
                                Text = XmlUtility.ReadAttribute(node, "text"),
                                Owner = owner.SymbolicName
                            };

            if (!string.IsNullOrWhiteSpace(XmlUtility.ReadAttribute(node, "order")))
            {
                result.Order = decimal.Parse(XmlUtility.ReadAttribute(node, "order"));
            }

            return result;
        }
    }
}