using System;
using System.Collections.Generic;
using System.Linq;

using SummerFramework.Core.Aop;

namespace SummerFramework.Base.DynamicProxy;

public class AspectInterceptor : IInterceptor
{
    public object? Intercept(Invocation invocation)
    {
        object? result = null;
        var attributes = invocation.Callee.Method.GetCustomAttributes(true);

        bool isExecuted = true, isSucceeded = false;

        foreach (var attr in attributes)
        {
            var before_attr = attr as BeforeAttribute;

            if (before_attr != null)
            {
                foreach (var callback_name in before_attr.Callbacks)
                {
                    var callback = AspectHandler.Befores[callback_name];
                    if (!callback())
                    {
                        isExecuted = false;
                        break;
                    }
                }
            }
        }

        try
        {
            if (isExecuted)
            {
                result = invocation.Invoke();
                isSucceeded = true;
            }
        }
        finally
        {
            foreach (var attr in attributes)
            {
                var target_attr = attr as AfterAttribute;

                if (target_attr != null)
                {
                    foreach (var callback_name in target_attr.Callbacks)
                    {
                        var callback = AspectHandler.Afters[callback_name];

                        if (!callback(isExecuted, isSucceeded))
                        {
                            break;
                        }
                    }
                }
            }
        }

        if (!isExecuted)
            result = null;

        return result;
    }
}
