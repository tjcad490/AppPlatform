using System;
using System.Collections.Generic;
using System.Xml;
using UIShell.OSGi;
using UIShell.OSGi.Utility;

namespace UIShell.Extension
{
    public abstract class AbstractBuilder<T> : IExtensionBuilder
    {
        #region IExtensionBuilder Members

        public event EventHandler<EventArgs<object>> ItemRemoved;
        public event EventHandler<EventArgs<object>> ItemAdded;

        /// <summary>
        /// Build object from xml node,the node is usually defined in Extension. 
        /// </summary>
        /// <param name="xmlNode"></param>
        /// <param name="owner"></param>
        public abstract void Build(XmlNode xmlNode, IBundle owner);

        /// <summary>
        /// Reset all objects built before.
        /// </summary>
        public abstract void Reset();

        public void Build(IEnumerable<XmlNode> xmlNodes, IBundle owner)
        {
            foreach (XmlNode item in xmlNodes)
            {
                Build(item, owner);
            }
        }

        public void Build(XmlNodeList nodeList, IBundle owner)
        {
            for (int i = 0; i < nodeList.Count; i++)
            {
                Build(nodeList[i], owner);
            }
        }

        #endregion

        protected void OnItemAdded(T control)
        {
            if (ItemAdded != null)
            {
                ItemAdded(this, new EventArgs<object>(control));
            }
        }

        protected void OnItemRemoved(T control)
        {
            if (ItemRemoved != null)
            {
                ItemRemoved(this, new EventArgs<object>(control));
            }
        }
    }
}