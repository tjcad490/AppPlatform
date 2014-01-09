using System;
using System.Linq;
using System.Web.Mvc;
using UIShell.OSGi.Loader;
using System.Web.SessionState;
using UIShell.OSGi.Collection;
using UIShell.OSGi.Utility;

namespace UIShell.OSGi.MvcCore
{
    public static class ControllerTypeCache
    {
        private static ThreadSafeDictionary<string, Type> _controllerTypes = new ThreadSafeDictionary<string, Type>();

        private static string GetKey(string plugin, string controllerName)
        {
            return string.Format("{0}$:$:${1}", plugin, controllerName);
        }

        public static void AddControllerType(string plugin, string controllerName, Type controllerType)
        {
            using (var locker = _controllerTypes.Lock())
            {
                locker[GetKey(plugin, controllerName)] = controllerType;
            }
        }
        public static Type GetControllerType(string plugin, string controllerName)
        {
            using (var locker = _controllerTypes.Lock())
            {
                var key = GetKey(plugin, controllerName);
                if (locker.ContainsKey(key))
                {
                    return locker[key];
                }
            }
            return null;
        }
    }

    public class BundleRuntimeControllerFactory : IControllerFactory
    {
        //if the controller is defined in a normal assembly other than plugin, the DefaultControllerFactory should be able to resolve it.
        IControllerFactory _defaultFactory = new DefaultControllerFactory();

        public IController CreateController(System.Web.Routing.RequestContext requestContext, string controllerName)
        {
            //http://localhost:1616/Blog/Index?plugin=BlogPlugin

            var pluginSymbolicName = requestContext.GetPluginSymbolicName();

            //when trying to access page provided by the Shell.
            if (string.IsNullOrWhiteSpace(pluginSymbolicName) && requestContext.RouteData.Values["controller"] != null)
            {
                try
                {
                    //if the controller is defined in a normal assembly other than plugin, the DefaultControllerFactory should be able to resolve it.
                    var controller = _defaultFactory.CreateController(requestContext, requestContext.RouteData.Values["controller"].ToString());
                    if (controller != null)
                    {
                        return controller;
                    }
                }
// ReSharper disable EmptyGeneralCatchClause
                catch
// ReSharper restore EmptyGeneralCatchClause
                {
                    //suppress any error.
                }
            }
        

        var type = GetControllerType(pluginSymbolicName, controllerName);
            if (type != null)
            {
                var resolver = BundleRuntime.Instance.GetFirstOrDefaultService<IControllerResolver>();
                if (resolver != null)
                {
                    var result = resolver.Resolve(type) as IController;
                    if (result != null)
                    {
                        return result;
                    }
                }

                return Activator.CreateInstance(type) as IController;
            }
            return null;
        }

        private Type GetControllerType(string pluginSymbolicName, string controllerName)
        {
            var symbolicName = pluginSymbolicName;
            if (symbolicName != null)
            {
                var controllerType = ControllerTypeCache.GetControllerType(symbolicName, controllerName);
                if (controllerType != null)
                {
                    FileLogUtility.Inform(string.Format("Loaded controller '{0}' from bundle '{1}' by using cache.", controllerName, symbolicName));
                    return controllerType;
                }

                var controllerTypeName = controllerName + "Controller";
                var runtimeService = BundleRuntime.Instance.GetFirstOrDefaultService<IRuntimeService>();
                var assemblies = runtimeService.LoadBundleAssembly((string)symbolicName);

                foreach (var assembly in assemblies)
                {
                    foreach (var type in assembly.GetTypes())
                    {
                        if (type.Name.Contains(controllerTypeName) && typeof(IController).IsAssignableFrom(type))
                        {
                            controllerType = type;
                            ControllerTypeCache.AddControllerType(symbolicName.ToString(), controllerName, controllerType);
                            FileLogUtility.Inform(string.Format("Loaded controller '{0}' from bundle '{1}' and then added to cache.", controllerName, symbolicName));
                            return controllerType;
                        }
                    }
                }

                FileLogUtility.Error(string.Format("Failed to load controller '{0}' from bundle '{1}'.", controllerName, symbolicName));
            }

            FileLogUtility.Error(string.Format("Failed to load controller '{0}' since the plugin name is not specified.", controllerName));
            
            return null;
        }

       

        public SessionStateBehavior GetControllerSessionBehavior(System.Web.Routing.RequestContext requestContext, string controllerName)
        {
            var controllerType = GetControllerType(requestContext.GetPluginSymbolicName(), controllerName);
            if (controllerType == null) return SessionStateBehavior.Default;

            var attribute = controllerType.GetCustomAttributes(typeof(SessionStateAttribute), true).OfType<SessionStateAttribute>().FirstOrDefault<SessionStateAttribute>();
            if (attribute == null) return SessionStateBehavior.Default;
            return attribute.Behavior;
        }

        public void ReleaseController(IController controller)
        {
            IDisposable disposable = controller as IDisposable;
            if (disposable != null) disposable.Dispose();
        }
    }
}