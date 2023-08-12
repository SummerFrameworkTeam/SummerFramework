using System;

namespace SummerFramework.Core.Configuration.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class ConfigureMethodAttribute : Attribute
{
    public string Identifier { get; set; }

    public ConfigureMethodAttribute(string id)
    {
        Identifier = id;
    }
}