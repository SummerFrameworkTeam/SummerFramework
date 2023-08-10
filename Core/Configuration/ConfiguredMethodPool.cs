using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using SummerFramework.Base;
using SummerFramework.Base.Data;

namespace SummerFramework.Core.Configuration;

public class ConfiguredMethodPool : LazySingleton<ConfiguredMethodPool>, IDictionaryContainer<MethodObject>
{
    protected Dictionary<string, MethodObject> methods { get; set; } = new();

    public void Add(string key, MethodObject value) => this.methods[key] = value;

    public MethodObject Get(string key) => this.methods[key];
}
