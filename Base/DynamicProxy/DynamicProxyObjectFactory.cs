using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SummerFramework.Base.DynamicProxy;

public static class DynamicProxyObjectFactory
{
    public static Dictionary<string, Type> DynamicProxyClasses = new Dictionary<string, Type>();

    public static object? Create(Type target)
    {
        var key = target.FullName!;
        var dpco = new DynamicProxyClassObject<AspectInterceptor>(target);

        if (DynamicProxyClasses.ContainsKey(key))
            return Activator.CreateInstance(DynamicProxyClasses[key]);
        
        DynamicProxyClasses.Add(key, dpco.ProxyType!);

        return Activator.CreateInstance(dpco.ProxyType!);
    }
}
