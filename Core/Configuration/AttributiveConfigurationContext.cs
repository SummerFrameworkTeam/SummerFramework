﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using SummerFramework.Base;
using SummerFramework.Base.Data;
using SummerFramework.Core.Configuration.Attributes;

namespace SummerFramework.Core.Configuration;

public class AttributiveConfigurationContext : AbstractConfigurationContext
{
    public Type ConfigurationClass { get; set; }

    internal AttributiveConfigurationContext(Type config_class)
    {
        ConfigurationClass = config_class;
        this.Phase();
    }

    public static AttributiveConfigurationContext Create(Type config_class) => new(config_class);

    public static AttributiveConfigurationContext Create<T>() where T : class => Create(typeof(T));

    protected override void Phase()
    {
        foreach (var method in ConfigurationClass.GetMethods())
        {
            if (method.HasAttribute<ConfiguredMethodAttribute>(out var attr))
                ConfiguredMethodPool.Instance.Add(attr!.Identifier, new MethodObject(null, method));
        }

        foreach (var prop in ConfigurationClass.GetProperties())
        {
            object? invoked = prop.GetMethod!.IsStatic ? null : Activator.CreateInstance(ConfigurationClass);

            if (prop.GetCustomAttributes<ConfiguredParametersAttribute>().Any())
            {
                var param_attrs = prop.GetCustomAttributes<ConfiguredParametersAttribute>();

                var constructor_parameters = new Dictionary<Type, object?>();

                foreach (var param_attr in param_attrs)
                    constructor_parameters.Add(param_attr.ParameterType, 
                        ObjectFactory.CreateObject(
                            TypeExtractor.GetShortNameFormValueType(param_attr.ParameterType), 
                            param_attr.ParameterValue));

                var target_constructor = prop.PropertyType.GetConstructor(constructor_parameters.Keys.ToArray());

                prop.SetValue(invoked, target_constructor?.Invoke(constructor_parameters.Values.ToArray()));
            }

            if (prop.HasAttribute(out ConfiguredObjectAttribute? attr))
            {
                ConfiguredObjectPool.Instance.Add(attr!.Identifier, prop.GetValue(invoked)!);
            }
        }
    }    
}