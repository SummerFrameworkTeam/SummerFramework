using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SummerFramework.Core.Configuration;

namespace SummerFramework.Base;

public class LazySingleton<T> where T : LazySingleton<T>, new()
{
    protected static Lazy<T> instance = new(() => new T());

    protected LazySingleton() { }

    public static T Instance
    {
        get => instance.Value;
    }
}
