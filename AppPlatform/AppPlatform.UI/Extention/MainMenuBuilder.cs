using System.Xml;
using MvcOSGi.Shell.Models;
using UIShell.Extension;
using UIShell.OSGi;

namespace MvcOSGi.Shell.Extension
{
    public class MainMenuBuilder : BuilderBase<MenuItem>
    {
        public override void Build(XmlNode xmlNode, IBundle owner)
        {
            MenuItem newItem = MenuItemParser.Build(xmlNode, owner);
            Items.Add(newItem);
            OnItemAdded(newItem);
        }
    }
}