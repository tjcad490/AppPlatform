using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Integration.Mvc;
using UIShell.OSGi;
using UIShell.OSGi.Utility;

namespace UIShell.IoCPlugin
{
    public static class IocContainer
    {
        public static ContainerBuilder Initialize(this BundleRuntime runtime)
        {
            //provide the container builder so that each plugin can register the dependancy when starting.
            var containerBuilder = new ContainerBuilder();
            runtime.AddService(typeof(ContainerBuilder), containerBuilder);
            return containerBuilder;
        }

        public static void SafeRegisterControllers(this ContainerBuilder containerBuilder, Assembly[] assmblies)
        {
            if (assmblies == null || assmblies.Length == 0)
            {
                return;
            }

            foreach (var assembly in assmblies)
            {
                FileLogUtility.Debug(string.Format("Registering assembly:{0}({1}) to IoC container.", assembly.FullName, assembly.Location));
            }
            //ContainerBuilder is not thread safe。
            lock (containerBuilder)
            {
                var container = BundleRuntime.Instance.GetFirstOrDefaultService<IContainer>();
                if (container == null)
                {
                    //If container is newly created,it can accept assmblies right now.
                    containerBuilder.RegisterControllers(assmblies);
                }
                else
                {
                    ContainerBuilder anotherBuilder = new ContainerBuilder();
                    anotherBuilder.RegisterControllers(assmblies);

                    anotherBuilder.Update(container);
                }
            }
        }

        public static void UnRegisterControllers(this ContainerBuilder containerBuilder, Assembly[] assmblies)
        {
            if (assmblies == null || assmblies.Length == 0)
            {
                return;
            }

            //ContainerBuilder is not thread safe.。
            lock (containerBuilder)
            {
                var container = BundleRuntime.Instance.GetFirstOrDefaultService<IContainer>();
                if (container == null)
                {
                    foreach (var assembly in assmblies)
                    {
                        FileLogUtility.Error(string.Format("Can't unregister assembly:{0}({1}), because no IContainer service is avaliable.",assembly.FullName, assembly.Location));
                    }
                    return;
                }
                var builder = new ContainerBuilder();
                var components = container.ComponentRegistry.Registrations
                                          .Where(cr => !assmblies.Contains(cr.Activator.LimitType.Assembly))
                    ;
                foreach (var c in components)
                {
                    builder.RegisterComponent(c);
                }

                foreach (var source in container.ComponentRegistry.Sources)
                {
                    builder.RegisterSource(source);
                }

                //builder.Update(container);
                var newContainer = builder.Build();
                BundleRuntime.Instance.RemoveService < IContainer>(container);
                BundleRuntime.Instance.AddService<IContainer>(newContainer);
            }
        }

        public static void Complete(this BundleRuntime runtime)
        {
            var containerBuilder = runtime.GetFirstOrDefaultService<ContainerBuilder>();
            if (containerBuilder == null)
            {
                return;
            }

            if (runtime.GetFirstOrDefaultService<IContainer>() != null)
            {
                return;
            }

            lock (containerBuilder)
            {
                var container = containerBuilder.Build(); 
                runtime.AddService(typeof(IContainer), container);
            }
        }
    }
}
