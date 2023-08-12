using System;
using System.Collections.Generic;
using System.Linq;

using SummerFramework.Base;
using SummerFramework.Core.Configuration.Scope;
using SummerFramework.Core.Task;

namespace SummerFramework.Core.Configuration;

public class ConfiguredObjectPool : IDictionaryContainer<object>
{
    protected Dictionary<string, object> objects = new();

    public TaskManager<object?> DeferredObjectConfigurationTaskManager { get; protected set; } = new();
    public ConfigurationScope Scope { get; internal set; }

    public ConfiguredObjectPool(ConfigurationScope? scope = null)
    {
        Scope = scope ?? Configuration.GlobalScope;
    }

    public void Add(string key, object value) => this.objects[key] = value;

    public object? Get(string key) => this.objects[key];
}