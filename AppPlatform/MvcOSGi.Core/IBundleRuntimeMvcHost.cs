using System.Collections.Generic;
using System.Reflection;
using System.IO;

namespace UIShell.OSGi.MvcCore
{
    public interface IBundleRuntimeMvcHost
    {
        
        BundleRuntime BundleRuntime
        {
            get;
        }
       
        IList<Assembly> TopLevelReferencedAssemblies
        { 
            get;
        }
        
        IList<Assembly> AddReferencedAssemblies(string bundleSymbolicName);
       
        void AddReferencedAssembly(Assembly assembly);
      
        void RemoveReferencedAssemlby(Assembly assembly);

        void RemoveReferencedAssemblies(IList<Assembly> assemblies);
        /// <summary>
        /// Restart current web application.
        /// </summary>
        void RestartAppDomain();
        
        void RestartAppDomain(WriteHtmlContentAfterReboot writeHtmlContent);

    }
    
    public delegate void WriteHtmlContentAfterReboot(StreamWriter sw);
}
