using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using SummerFramework.Base.Logger;

namespace SummerFramework.Core.UnitTest;

public static class TestController
{
    public static Logger logger { get; private set; } = LoggerFactory.CreateLogger("main_test");

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
                        logger.Info($"Test Method<{method.Name}> Start:");
                        method.Invoke(null, null);
                        logger.Info($"Test Method<{method.Name}> End!");
                    }

                    if (method.GetCustomAttribute<TimerAttribute>() != null && method.IsStatic)
                    {
                        var t1 = DateTime.Now;
                        method.Invoke(null, null);
                        var t2 = DateTime.Now;

                        logger.Custom("TIMER", $"{t2 - t1}", ConsoleColor.White);
                    }
                }
            }  
        }
    }
}