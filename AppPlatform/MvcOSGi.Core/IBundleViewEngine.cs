using System.Web.Mvc;

namespace UIShell.OSGi.MvcCore
{
    public interface IBundleViewEngine : IViewEngine
    {
        IBundle Bundle { get; }
        string SymbolicName { get; }
    }
}