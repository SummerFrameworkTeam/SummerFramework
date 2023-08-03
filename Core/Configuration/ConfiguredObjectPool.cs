using System;
using System.Collections.Generic;
using System.Linq;

namespace SummerFramework.Core.Configuration;

public class ConfiguredObjectPool
{
    private static Lazy<ConfiguredObjectPool> instance = new(() => new ConfiguredObjectPool());

    private ConfiguredObjectPool() { }

    public static ConfiguredObjectPool Instance
    {
        get
        {
            return instance.Value;
        }
    }

    protected Dictionary<string, object> objects { get; set; } = new();

    public void AddObject(string key, object value)
    {
        this.objects[key] = value;
    }

    public object GetObject(string key)
    {
        return this.objects[key];
    }
}
