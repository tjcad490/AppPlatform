using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using UIShell.OSGi.MvcCore;
using UIShell.OSGi.Utility;
using System.Threading;
using System.Reflection;
using UIShell.OSGi.Configuration.BundleManifest;
using System.Web.Compilation;
using System.IO;
using UIShell.OSGi.Core.Service;
using UIShell.OSGi.Loader;
using System.Web.Mvc;
using System.Web.Routing;


namespace UIShell.OSGi.MvcCore
{
    public class Bootstrapper
    {

        private static Dictionary<BundleData, IList<Assembly>> _registeredBunldeCache = new Dictionary<BundleData, IList<Assembly>>();


        public BundleRuntime BundleRuntime { get; private set; }

        protected virtual BundleRuntime CreateBundleRuntime()
        {
            return new BundleRuntime();
        }

        public void StartBundleRuntime()
        {
            FileLogUtility.Debug("WebSite is starting.");
            AddPreDefinedRefAssemblies();
            // Set SQLCE compact before BundleRuntime starting.
            AppDomain.CurrentDomain.SetData("SQLServerCompactEditionUnderWebHosting", true);

            BundleRuntime = CreateBundleRuntime();
            FileLogUtility.Debug("Framework is starting.");

            BundleRuntime.Start();

            //force to start all plugins
            LoadBundleResources();

            FileLogUtility.Debug("Framework is started.");

            ControllerBuilder.Current.SetControllerFactory(new BundleRuntimeControllerFactory());

            RegisterGlobalFilters(GlobalFilters.Filters);
        }

        private void LoadBundleResources()
        {
            foreach (var bundle in BundleRuntime.Framework.Bundles)
            {
                var bundleData =
                    BundleRuntime.Instance.GetFirstOrDefaultService<IBundleInstallerService>()
                                 .GetBundleDataByName(bundle.SymbolicName);
                if (bundleData == null)
                {
                    continue;
                }

                //register bundle assemblies to BuildManager.
                var assemblies = this.AddReferencedAssemblies(bundleData.SymbolicName);
                FileLogUtility.Debug(string.Format("Loaded assembiles from bundle {0}", bundle.SymbolicName));
                if (assemblies != null && assemblies.Count > 0)
                {
                    _registeredBunldeCache[bundleData] = assemblies;
                }
            }
        }

        public virtual void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        protected virtual void AddPreDefinedRefAssemblies()
        {
            AddReferencedAssembly(typeof(BundleRuntime).Assembly);

            AddReferencedAssembly(GetType().Assembly);
        }

        protected virtual void Application_End(object sender, EventArgs e)
        {
            FileLogUtility.Debug("Framework is stopping.");

            BundleRuntime.Stop();
            FileLogUtility.Debug("Framework is stopped.");
            FileLogUtility.Debug("WebSite is stopped.");
        }

        #region IBundleRuntimeMvcHost Members

        public virtual IList<Assembly> AddReferencedAssemblies(string bundleSymbolicName)
        {
            //Check if this bundle still exist or not.
            var bundleData = BundleRuntime.Instance.GetFirstOrDefaultService<IBundleInstallerService>().GetBundleDataByName(bundleSymbolicName);
            if (bundleData == null)
            {
                return new List<Assembly>();
            }

            //already registered its assembiles
            IList<Assembly> registeredItems;
            if (_registeredBunldeCache.TryGetValue(bundleData, out registeredItems))
            {
                return registeredItems;
            }

            var serviceContainer = BundleRuntime.Framework.ServiceContainer;
            var service = serviceContainer.GetFirstOrDefaultService<IRuntimeService>();
            var assemlbies = service.LoadBundleAssembly(bundleSymbolicName);
            assemlbies.ForEach(AddReferencedAssembly);
            //cache the assemblies
            _registeredBunldeCache[bundleData] = assemlbies;

            return assemlbies;
        }

        public virtual void RemoveReferencedAssemblies(IList<Assembly> assemblies)
        {
            if (assemblies != null)
            {
                foreach (Assembly assembly in assemblies)
                {
                    RemoveReferencedAssemlby(assembly);
                }
            }
        }

        public void AddReferencedAssembly(Assembly assembly)
        {
            //todo:use reflection to add assembly to Build Manager if the app_start is finished.
            BuildManager.AddReferencedAssembly(assembly);

            FileLogUtility.Debug(
                string.Format("Add assembly '{0} to top level referenced assemblies.'",
                assembly.FullName));
        }

        public void RemoveReferencedAssemlby(Assembly assembly)
        {
            //todo:use reflection to remove assembly to Build Manager
            FileLogUtility.Debug(
                string.Format("Remove assembly '{0} from top level referenced assemblies.'",
                assembly.FullName));
        }

        public void RestartAppDomain()
        {
            //todo:RestartAppDomain
        }


        #endregion
    }
}
