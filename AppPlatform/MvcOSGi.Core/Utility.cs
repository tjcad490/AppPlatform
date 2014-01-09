using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Routing;

namespace UIShell.OSGi.MvcCore
{
    public static class Utility
    {
        public static string RedirectToBundlePath(string locationFormat, string bundleRelativePath)
        {
            return locationFormat.Replace("~", bundleRelativePath);
        }

        public static IEnumerable<string> RedirectToBundlePath(IEnumerable<string> locationFormats, string bundleRelativePath)
        {
            return locationFormats.Select(item => RedirectToBundlePath(item, bundleRelativePath));
        }

        public static string MapPathReverse(string fullServerPath)
        {
            return @"~\" + fullServerPath.Replace(HostingEnvironment.ApplicationPhysicalPath, String.Empty);
        }

        public static string GetPluginSymbolicName(this System.Web.Routing.RequestContext requestContext)
        {
            return requestContext.HttpContext.GetPluginSymbolicName();
        }

        public static string GetPluginSymbolicName(this ControllerContext requestContext)
        {
            return requestContext.HttpContext.GetPluginSymbolicName();
        }

        public static string GetPluginSymbolicName(this HttpContextBase context)
        {
            return context.Request.QueryString["plugin"];
        }
    }
}
