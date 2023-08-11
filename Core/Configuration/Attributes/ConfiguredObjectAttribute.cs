using System;

namespace SummerFramework.Core.Configuration.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class ConfiguredObjectAttribute : Attribute
{
    public string Identifier { get; set; }

    public ConfiguredObjectAttribute(string id)
    {
        Identifier = id;
    }
}