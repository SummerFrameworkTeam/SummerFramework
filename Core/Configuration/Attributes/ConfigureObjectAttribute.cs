using System;

namespace SummerFramework.Core.Configuration.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class ConfigureObjectAttribute : Attribute
{
    public string Identifier { get; set; }

    public ConfigureObjectAttribute(string id)
    {
        Identifier = id;
    }
}