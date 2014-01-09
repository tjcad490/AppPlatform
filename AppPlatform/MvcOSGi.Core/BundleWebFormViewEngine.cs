using System.Linq;
using System.Web.Mvc;

namespace UIShell.OSGi.MvcCore
{
    public class BundleWebFormViewEngine : WebFormViewEngine, IBundleViewEngine
    {
        

        public BundleWebFormViewEngine(IBundle bundle)
        {
            Bundle = bundle;
            string bundleRelativePath =Utility.MapPathReverse(bundle.Location);

            MasterLocationFormats = Utility.RedirectToBundlePath(MasterLocationFormats, bundleRelativePath).ToArray();
            AreaMasterLocationFormats = Utility.RedirectToBundlePath(AreaMasterLocationFormats, bundleRelativePath).ToArray();
            ViewLocationFormats = Utility.RedirectToBundlePath(ViewLocationFormats, bundleRelativePath).ToArray();
            AreaViewLocationFormats = Utility.RedirectToBundlePath(AreaViewLocationFormats, bundleRelativePath).ToArray();
        }

        public IBundle Bundle { get; private set; }

        public string SymbolicName
        {
            get { return Bundle.SymbolicName; }
        }



        public override ViewEngineResult FindView(ControllerContext controllerContext, string viewName,
                                                  string masterName, bool useCache)
        {
            object symbolicName = controllerContext.GetPluginSymbolicName();
            if (symbolicName != null && Bundle.SymbolicName.Equals(symbolicName))
            {
                return base.FindView(controllerContext, viewName, masterName, useCache);
            }
            return new ViewEngineResult(new string[0]);
        }

        public override ViewEngineResult FindPartialView(ControllerContext controllerContext, string partialViewName,
                                                         bool useCache)
        {
            object symbolicName = controllerContext.GetPluginSymbolicName();
            if (symbolicName != null && Bundle.SymbolicName.Equals(symbolicName))
            {
                return base.FindPartialView(controllerContext, partialViewName, useCache);
            }
            return new ViewEngineResult(new string[0]);
        }
    }
}