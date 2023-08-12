using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using SummerFramework.Base;
using SummerFramework.Base.Data;
using SummerFramework.Core.Configuration.Scope;

namespace SummerFramework.Core.Configuration;

public class ConfiguredMethodPool : IDictionaryContainer<MethodObject>
{
    protected Dictionary<string, MethodObject> methods = new();

    public ConfigurationScope Scope { get; internal set; }

    public ConfiguredMethodPool(ConfigurationScope? scope = null)
    {
        Scope = scope ?? Configuration.GlobalScope;
    }

    public void Add(string key, MethodObject value) => this.methods[key] = value;

    public MethodObject Get(string key) => this.methods[key];
}
