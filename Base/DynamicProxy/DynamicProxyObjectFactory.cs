using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SummerFramework.Base.DynamicProxy;

public static class DynamicProxyObjectFactory
{
    public static volatile Dictionary<string, Type> DynamicProxyClasses = new Dictionary<string, Type>();

    public static object? Create(Type target)
    {
        var key = target.FullName!;
        var dpco = new DynamicProxyClassObject<AspectInterceptor>(target);

        if (DynamicProxyClasses.ContainsKey(key))
            return Activator.CreateInstance(DynamicProxyClasses[key]);
        
        DynamicProxyClasses.Add(key, dpco.ProxyType!);

        return Activator.CreateInstance(dpco.ProxyType!);
    }

    public static T? Create<T>() where T : class
    {
        return Create(typeof(T)) as T;
    }
}
