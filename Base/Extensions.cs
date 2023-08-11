using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using SummerFramework.Core.UnitTest;

namespace SummerFramework.Base;

public static class Extensions
{
    public static string ToLegalName(this DateTime self)
    {
        return self.ToString().Replace(' ', '-').Replace('/', '-').Replace(':', '-');
    }

    public static void Log(this object self)
    {
        TestController.logger.Info(self.ToString()!);
    }

    public static void ForeachPrint<T>(this T[] self)
    {
        self.ToList().ForEach(i => TestController.logger.Info(i?.ToString()!));
    }

    public static ConstructorInfo GetParameterlessConstructor(this Type self)
    {
        return self.GetConstructors(BindingFlags.Public | BindingFlags.Instance).First(c => c.GetParameters().Length == 0);
    }

    public static bool HasAttribute<T>(this Type self, out T? result) where T : Attribute
    {
        result = self.GetCustomAttribute<T>();
        return self != null;
    }

    public static bool HasAttribute<T>(this MemberInfo self, out T? result) where T : Attribute
    {
        result = self.GetCustomAttribute<T>();
        return self != null;
    }
}
