using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using SummerFramework.Base;

namespace SummerFramework.Core.Configuration;

public class ConfiguredMethodPool : LazySingleton<ConfiguredMethodPool>, IDictionaryContainer<MethodInfo>
{
    protected Dictionary<string, MethodInfo> methods { get; set; } = new();

    public void Add(string key, MethodInfo value) => this.methods[key] = value;

    public MethodInfo Get(string key) => this.methods[key];
}
