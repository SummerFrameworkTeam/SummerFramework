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

    public static ConstructorInfo GetParameterlessConstructor(this Type self)
    {
        return self.GetConstructors(BindingFlags.Public | BindingFlags.Instance).First(c => c.GetParameters().Length == 0);
    }
}
