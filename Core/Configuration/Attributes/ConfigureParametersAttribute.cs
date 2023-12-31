﻿using System;
using System.Collections.Generic;

namespace SummerFramework.Core.Configuration.Attributes;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
public class ConfigureParametersAttribute : Attribute
{
    public Type ParameterType { get; set; }

    public string ParameterValue { get; set; }

    public ConfigureParametersAttribute(Type param_type, string param_value)
    {
        ParameterType = param_type;
        ParameterValue = param_value;
    }
}
