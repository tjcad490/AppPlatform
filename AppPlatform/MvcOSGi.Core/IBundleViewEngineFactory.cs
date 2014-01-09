namespace UIShell.OSGi.MvcCore
{
    public interface IBundleViewEngineFactory
    {
        IBundleViewEngine CreateViewEngine(IBundle bundle);
    }

    public class BundleRazorViewEngineFactory:IBundleViewEngineFactory
    {
        public IBundleViewEngine CreateViewEngine(IBundle bundle)
        {
            return new BundleRazorViewEngine(bundle);
        }
    }

    public class BundleWebFormViewEngineFactory : IBundleViewEngineFactory
    {
        public IBundleViewEngine CreateViewEngine(IBundle bundle)
        {
            return new BundleWebFormViewEngine(bundle);
        }
    }
}
