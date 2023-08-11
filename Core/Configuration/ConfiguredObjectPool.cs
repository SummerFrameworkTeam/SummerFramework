using System;
using System.Collections.Generic;
using System.Linq;

using SummerFramework.Base;
using SummerFramework.Core.Task;

namespace SummerFramework.Core.Configuration;

public class ConfiguredObjectPool : LazySingleton<ConfiguredObjectPool>, IDictionaryContainer<object>
{
    protected Dictionary<string, object> Objects { get; set; } = new();
    public TaskManager<object?> DeferredObjectConfigurationTaskManager { get; protected set; } = new();

    public void Add(string key, object value) => this.Objects[key] = value;

    public object? Get(string key) => this.Objects[key];
}
