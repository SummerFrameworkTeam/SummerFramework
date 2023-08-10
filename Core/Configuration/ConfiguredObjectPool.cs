using System;
using System.Collections.Generic;
using System.Linq;

using SummerFramework.Base;
using SummerFramework.Core.Task;

namespace SummerFramework.Core.Configuration;

public class ConfiguredObjectPool : LazySingleton<ConfiguredObjectPool>, IDictionaryContainer<object>
{
    protected Dictionary<string, object> Objects { get; set; } = new();
    protected TaskManager<object?> DeferredObjectConfigurationTaskManager = new();

    public void Add(string key, object value) => this.Objects[key] = value;

    public object Get(string key) => this.Objects[key];

    public DeferredTask<object?> CreateDeferringObject(string key)
    {
        var result = new DeferredTask<object?>($"deferred_init_{key}", () => Get(key));
        DeferredObjectConfigurationTaskManager.AddTask(result);
        return result;
    }

    public object? GetDeferringObject(string key)
    {
        return this.DeferredObjectConfigurationTaskManager.RunSpecified(key);
    }
}
