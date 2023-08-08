using System;
using System.Collections.Generic;

namespace SummerFramework.Core.Aop;

public static class AspectHandler
{
    public static readonly Dictionary<string, BeforeAction> Befores = new();
    public static readonly Dictionary<string, AfterAction> Afters = new();

    public static void AddBefore(string key, BeforeAction value) => Befores.Add(key, value);
    public static void AddAfter(string key, AfterAction value) => Afters.Add(key, value);
}