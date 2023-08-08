using System;
using System.Collections.Generic;
using System.Linq;

namespace SummerFramework.Base.DynamicProxy;
public interface IInterceptor
{
    object? Intercept(Invocation invocation);
}