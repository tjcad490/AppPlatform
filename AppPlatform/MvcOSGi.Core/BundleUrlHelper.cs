using System;
using System.Web.Hosting;
using UIShell.OSGi.Utility;
using System.IO;

namespace UIShell.OSGi.MvcCore
{
    public static class BundleUrlHelper
    {
        private static string _webRootPath;

        static BundleUrlHelper()
        {
            _webRootPath = HostingEnvironment.MapPath("~");
        }

        public static string Content(IBundle bundle, string url)
        {
            AssertUtility.ArgumentNotNull(bundle, "bundle");
            AssertUtility.ArgumentHasText(url, "url");

            while (url.StartsWith("~") || url.StartsWith("/") || url.StartsWith("\\"))
            {
                url = url.Remove(0, 1);
            }

            return Path.Combine(bundle.Location.Replace(_webRootPath, @"~\"), url);
        }

        public static string Content(string symbolicName, string url)
        {
            IBundle bundle = BundleRuntime.Instance.Framework.Bundles.GetBundleBySymbolicName(symbolicName);
            if (bundle == null)
            {
                throw new Exception(string.Format("Bundle {0} doesn't exists.", symbolicName));
            }
            return Content(bundle, url);
        }
    }
}
