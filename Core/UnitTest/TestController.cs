using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using SummerFramework.Base.Logger;

namespace SummerFramework.Core.UnitTest;

public static class TestController
{
    public static Logger Logger { get; private set; } = LoggerFactory.CreateLogger("main_test");

    public static void Run()
    {
        var assembly = Assembly.GetEntryAssembly()!;

        foreach (var type in assembly.DefinedTypes)
        {
            Type? target_type = null;

            if (type.GetCustomAttribute<TestClassAttribute>() != null)
            {
                target_type = type;

                foreach (var method in target_type!.GetMethods())
                {
                    if (method.GetCustomAttribute<TestAttribute>() != null && method.IsStatic)
                    {
                        Logger.Info($"Test Method<{method.Name}> Start:");
                        method.Invoke(null, null);
                        Logger.Info($"Test Method<{method.Name}> End!");
                    }
                }
            }  
        }
    }
}