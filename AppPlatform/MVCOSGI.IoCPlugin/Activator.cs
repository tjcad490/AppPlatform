using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Autofac;
using UIShell.OSGi;
using UIShell.OSGi.Core.Service;
using UIShell.OSGi.Loader;
using UIShell.OSGi.MvcCore;
using UIShell.OSGi.Utility;

namespace UIShell.IoCPlugin
{
    public class Activator : IBundleActivator
    {
        readonly ConcurrentDictionary<long, List<Assembly>> _registerHistory = new ConcurrentDictionary<long, List<Assembly>>();
        public void Start(IBundleContext context)
        {
            context.BundleStateChanged += context_BundleStateChanged;

            //provide the container builder so that each plugin can register the dependancy when starting.
           var containerBuilder = BundleRuntime.Instance.Initialize();

            //Register active bundle assemblies.
            foreach (var bundle in context.Framework.Bundles)
            {
                if (bundle.State == BundleState.Active)
                {
                    var service = BundleRuntime.Instance.GetFirstOrDefaultService<IRuntimeService>();
                    var assemblies = service.LoadBundleAssembly(bundle.SymbolicName);
                    RegisterBundleAssemblies(bundle.BundleID,containerBuilder, assemblies);
                }
            }

            if (BundleRuntime.Instance.State == BundleRuntimeState.Started)
            {
                BundleRuntime.Instance.Complete();
            }
            else if(BundleRuntime.Instance.State == BundleRuntimeState.Starting)
            {
                context.FrameworkStateChanged += context_FrameworkStateChanged;
            }

            context.AddService(typeof(IControllerResolver), new ControllerResolver());
        }

        void context_FrameworkStateChanged(object sender, FrameworkEventArgs e)
        {
            if (e.EventType == FrameworkEventType.Started)
            {
                BundleRuntime.Instance.Complete();
            }
        }

        void context_BundleStateChanged(object sender, BundleStateChangedEventArgs args)
        {
            var bundleData = BundleRuntime.Instance.GetFirstOrDefaultService<IBundleInstallerService>().GetBundleDataByName(args.Bundle.SymbolicName);
            if (bundleData == null)
            {
                return;
            }
            bool needLoad = (args.CurrentState == BundleState.Active);

            if (needLoad)
            {
                //register bundle assemblies to BuildManager.
                var service = BundleRuntime.Instance.GetFirstOrDefaultService<IRuntimeService>();
                var assemblies = service.LoadBundleAssembly(bundleData.SymbolicName);
                if (assemblies != null && assemblies.Count > 0)
                {
                    var builderContainer = BundleRuntime.Instance.GetFirstOrDefaultService<ContainerBuilder>();
                    if (builderContainer != null)
                    {
                        RegisterBundleAssemblies(args.Bundle.BundleID, builderContainer, assemblies);
                    }
                }
            }
            else if (args.CurrentState == BundleState.Stopping)
            {
                //如果是Stopping，就不再需要更新ContainerBuilder，因为这个服务即将不可用。
                if (BundleRuntime.Instance.State != BundleRuntimeState.Stopping)
                {
                    var builderContainer = BundleRuntime.Instance.GetFirstOrDefaultService<ContainerBuilder>();
                    if (builderContainer != null)
                    {
                        List<Assembly> assemblies;
                        if (_registerHistory.TryGetValue(args.Bundle.BundleID, out assemblies))
                        {
                            builderContainer.UnRegisterControllers(assemblies.ToArray());
                        }
                    }
                }
            }
        }

        private void RegisterBundleAssemblies(long bundleId, ContainerBuilder builderContainer,
                                              List<Assembly> assemblies)
        {
            if (assemblies == null || assemblies.Count == 0)
            {
                return;
            }

            builderContainer.SafeRegisterControllers(assemblies.ToArray());

            //cache the assemblies.
            _registerHistory[bundleId] = assemblies;
        }

        public void Stop(IBundleContext context)
        {
            context.BundleStateChanged -= context_BundleStateChanged;
        }
    }
}
