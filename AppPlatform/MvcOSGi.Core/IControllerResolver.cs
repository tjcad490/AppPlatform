using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UIShell.OSGi.MvcCore
{
    public interface IControllerResolver
    {
        object Resolve(Type type);
    }
}
