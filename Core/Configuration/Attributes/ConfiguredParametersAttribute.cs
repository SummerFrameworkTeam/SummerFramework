using System;
using System.Collections.Generic;

namespace SummerFramework.Core.Configuration.Attributes;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
public class ConfiguredParametersAttribute : Attribute
{
    public Type RefTargetType { get; set; }

    public string RefTargetIdentifier { get; set; }

    public ConfiguredParametersAttribute(Type ref_target_type, string ref_target_id)
    {
        RefTargetType = ref_target_type;
        RefTargetIdentifier = ref_target_id;
    }
}
