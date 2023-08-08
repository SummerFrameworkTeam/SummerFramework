using System;
using System.Collections.Generic;
using System.Linq;

using SummerFramework.Base;

namespace SummerFramework.Core.Configuration;

public class ConfiguredObjectPool : LazySingleton<ConfiguredObjectPool>, IDictionaryContainer<object>
{
    protected Dictionary<string, object> objects { get; set; } = new();

    public void Add(string key, object value) => this.objects[key] = value;

    public object Get(string key) => this.objects[key];
}
