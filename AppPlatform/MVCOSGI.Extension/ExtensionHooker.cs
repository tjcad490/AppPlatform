using System;
using System.Collections.Generic;
using UIShell.OSGi;
using UIShell.OSGi.Core.Service;
using UIShell.OSGi.Utility;

namespace UIShell.Extension
{
    public class ExtensionHooker:IDisposable
    {
        private readonly Dictionary<string, ExtensionChangeHandler> _extensionBuilders =
            new Dictionary<string, ExtensionChangeHandler>();

        private readonly IExtensionManager _extensionManager;

        public ExtensionHooker(IExtensionManager extensionManager)
        {
            AssertUtility.ArgumentNotNull(extensionManager, "extensionManager");
            _extensionManager = extensionManager;
            _extensionManager.ExtensionChanged += _extensionManager_ExtensionChanged;
        }

        private void _extensionManager_ExtensionChanged(object sender, ExtensionEventArgs e)
        {
            if (BundleRuntime.Instance.State != BundleRuntimeState.Starting && BundleRuntime.Instance.State!= BundleRuntimeState.Started  )
            {
                return;
            }
            if (e.Action == CollectionChangedAction.Add)
            {
                ExtensionChangeHandler extensionChangeBuilder;
                if (_extensionBuilders.TryGetValue(e.ExtensionPoint, out extensionChangeBuilder))
                {
                    extensionChangeBuilder.Builder.Build(e.Extension.Data, e.Extension.Owner);
                }
            }
            else if (e.Action == CollectionChangedAction.Remove)
            {
                ExtensionChangeHandler extensionChangeBuilder;
                if (_extensionBuilders.TryGetValue(e.ExtensionPoint, out extensionChangeBuilder))
                {
                    //Rebuild controls.
                    extensionChangeBuilder.Builder.Reset();
                    LoadExistingExtensions(e.ExtensionPoint, extensionChangeBuilder);
                }
            }
        }

        private void LoadExistingExtensions(string extensionPoint, ExtensionChangeHandler extensionChangeBuilder)
        {
            _extensionManager.GetExtensions(extensionPoint).ForEach(
                extension => extensionChangeBuilder.Builder.Build(extension.Data, extension.Owner));
        }

        public void HookExtension(string extensionPointName, ExtensionChangeHandler handler)
        {
            _extensionBuilders[extensionPointName] = handler;

            //load the extensions which bundle already be active.
            LoadExistingExtensions(extensionPointName, _extensionBuilders[extensionPointName]);
        }

        public void Dispose()
        {
            _extensionManager.ExtensionChanged -= _extensionManager_ExtensionChanged;
            foreach (var extensionChangeHandlder in _extensionBuilders)
            {
                extensionChangeHandlder.Value.Dispose();
            }
        }
    }
}