using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using SummerFramework.Base;
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
        foreach (var prop in ConfigurationClass.GetProperties())
        {
            object? invoked = prop.GetMethod!.IsStatic ? null : Activator.CreateInstance(ConfigurationClass);

            if (prop.GetCustomAttributes<ConfiguredParametersAttribute>().Any())
            {
                var param_attrs = prop.GetCustomAttributes<ConfiguredParametersAttribute>();

                var constructor_parameters = new Dictionary<Type, object?>();

                foreach (var param_attr in param_attrs)
                {
                    if (SyntaxPhaser.PhaseRefExpression(param_attr.ParameterValue, out var ref_target_id))
                    {
                        constructor_parameters.Add(param_attr.ParameterType, ConfiguredObjectPool.Instance.Get(ref_target_id));
                    }
                    else if (SyntaxPhaser.InvokeMethod(param_attr.ParameterValue, out var final_result))
                    {
                        constructor_parameters.Add(param_attr.ParameterType, final_result);
                    }
                    else
                    {
                        object? value;
                        if (TypeExtractor.vt_mappings.ContainsKey(TypeExtractor.GetShortNameFormValueType(param_attr.ParameterType)))
                            value = ObjectFactory.CreateValueType(TypeExtractor.GetShortNameFormValueType(param_attr.ParameterType), param_attr.ParameterValue);
                        else
                            value = ObjectFactory.CreateReferenceType(param_attr.ParameterType.Name, param_attr.ParameterValue);

                        constructor_parameters.Add(param_attr.ParameterType, value);
                    }
                }

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