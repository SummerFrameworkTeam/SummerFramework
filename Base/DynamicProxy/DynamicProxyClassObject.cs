using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SummerFramework.Base.DynamicProxy;

public class DynamicProxyClassObject<T> where T : IInterceptor, new()
{
    public Type PrincipalType { get; set; }
    public Type? ProxyType { get; set; }

    public DynamicProxyClassObject(Type principal)
    {
        this.PrincipalType = principal;
        this.ProxyType = DynamicProxyBuilder<T>.Build(principal);
    }
}
