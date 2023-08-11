using System;
using SummerFramework.Core.Task;

namespace SummerFramework.Core.Configuration.Attributes;

public class ConfiguredMethodAttribute : Attribute
{
    public string Identifier { get; set; }
    public object? Invoked { get; set; }

    public ConfiguredMethodAttribute(string id, string invoked)
    {
        Identifier = id;
        Invoked = ConfiguredObjectPool.Instance.CreateDeferringObject(invoked);
    }
}
