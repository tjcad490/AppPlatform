using System;
using UIShell.OSGi.Utility;

namespace UIShell.Extension
{
    public class ExtensionChangeHandler : IDisposable
    {
        public ExtensionChangeHandler()
        {
            
        }
        public ExtensionChangeHandler(IExtensionBuilder builder, Action<object> newItemHandler,
                                       Action<object> extensionRemovedHandler)
        {
            Initialize(builder, newItemHandler, extensionRemovedHandler);
        }

        protected void Initialize(IExtensionBuilder builder, Action<object> newItemHandler, Action<object> extensionRemovedHandler)
        {
            Builder = builder;
            NewItemHandler = newItemHandler;
            RemoveItemHandler = extensionRemovedHandler;

            builder.ItemAdded += builder_ItemAdded;
            builder.ItemRemoved += builder_ItemRemoved;
        }

        void builder_ItemRemoved(object sender, EventArgs<object> e)
        {
            if (RemoveItemHandler != null)
            {
                RemoveItemHandler(e.Item);
            }
        }

        void builder_ItemAdded(object sender, EventArgs<object> e)
        {
            if (NewItemHandler != null)
            {
                NewItemHandler(e.Item);
            }
        }

        public IExtensionBuilder Builder { get; protected set; }
        public Action<object> NewItemHandler { get; protected set; }
        public Action<object> RemoveItemHandler { get; protected set; }

        public void Dispose()
        {
            Builder.ItemAdded -= builder_ItemAdded;
            Builder.ItemRemoved -= builder_ItemRemoved;
        }
    }
}