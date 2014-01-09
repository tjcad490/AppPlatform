using System.Linq;
using System.Web.Mvc;
using UIShell.OSGi.Collection;
using UIShell.OSGi.Collection.Locker;

namespace UIShell.OSGi.MvcCore
{
    public class BundleRuntimeViewEngine : IViewEngine
    {
        private readonly ThreadSafeDictionary<string, IBundleViewEngine> _viewEngines =
            new ThreadSafeDictionary<string, IBundleViewEngine>();

        public BundleRuntimeViewEngine(IBundleViewEngineFactory viewEngineFactory)
        {
            this.BundleViewEngineFactory = viewEngineFactory;
            BundleRuntime.Instance.Framework.EventManager.AddBundleEventListener(BundleEventListener, true);
            BundleRuntime.Instance.Framework.EventManager.AddFrameworkEventListener(FrameworkEventListener);

            BundleRuntime.Instance.Framework.Bundles.ForEach(AddViewEngine);
        }

        public ViewEngineResult FindPartialView(ControllerContext controllerContext, string partialViewName,
                                                bool useCache)
        {
            var viewEngine = GetViewEngine(controllerContext);
            if (viewEngine != null)
            {
                return viewEngine.FindPartialView(controllerContext, partialViewName, useCache);
            }
            return new ViewEngineResult(new string[0]);
        }

        public ViewEngineResult FindView(ControllerContext controllerContext, string viewName, string masterName,
                                         bool useCache)
        {
            var viewEngine = GetViewEngine(controllerContext);
            if (viewEngine != null)
            {
                return viewEngine.FindView(controllerContext, viewName, masterName, useCache);
            }
            return new ViewEngineResult(new string[0]);
        }

        public void ReleaseView(ControllerContext controllerContext, IView view)
        {
            var viewEngine = GetViewEngine(controllerContext);
            if (viewEngine != null)
            {
                viewEngine.ReleaseView(controllerContext, view);
            }
        }

        private void FrameworkEventListener(object sender, FrameworkEventArgs args)
        {
            if (args.EventType == FrameworkEventType.Stopped)
            {
                BundleRuntime.Instance.Framework.EventManager.RemoveBundleEventListener(BundleEventListener, true);
                BundleRuntime.Instance.Framework.EventManager.RemoveFrameworkEventListener(FrameworkEventListener);
            }
        }

        private void BundleEventListener(object sender, BundleStateChangedEventArgs args)
        {
            if (args.CurrentState == BundleState.Active)
            {
                AddViewEngine(args.Bundle);
            }
            else if (args.CurrentState == BundleState.Stopping)
            {
                RemoveViewEngine(args.Bundle);
            }
        }

        public IBundleViewEngineFactory BundleViewEngineFactory { get; private set; }

        private void AddViewEngine(IBundle bundle)
        {
            using (DictionaryLocker<string, IBundleViewEngine> locker = _viewEngines.Lock())
            {
                locker[bundle.SymbolicName] = BundleViewEngineFactory.CreateViewEngine(bundle);
            }
        }

        private void RemoveViewEngine(IBundle bundle)
        {
            _viewEngines.Remove(bundle.SymbolicName);
        }

        private IViewEngine GetViewEngine(ControllerContext controllerContext)
        {
            object symbolicName = controllerContext.GetPluginSymbolicName();
            if (symbolicName != null)
            {
                using (DictionaryLocker<string, IBundleViewEngine> locker = _viewEngines.Lock())
                {
                    string key = symbolicName.ToString();
                    if (locker.ContainsKey(key))
                    {
                        return locker[key];
                    }
                }
            }
            return null;
        }
    }
}