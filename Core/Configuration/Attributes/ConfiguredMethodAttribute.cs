using System;

namespace SummerFramework.Core.Configuration.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class ConfiguredMethodAttribute : Attribute
{
    public string Identifier { get; set; }

    public ConfiguredMethodAttribute(string id)
    {
        Identifier = id;
    }
}