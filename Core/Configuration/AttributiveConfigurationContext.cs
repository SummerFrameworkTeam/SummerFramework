using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using SummerFramework.Base;
using SummerFramework.Base.Data;
using SummerFramework.Core.Configuration.Attributes;
using SummerFramework.Core.Configuration.Scope;

namespace SummerFramework.Core.Configuration;

public class AttributiveConfigurationContext : AbstractConfigurationContext
{
    public Type ConfigurationClass { get; set; }

    internal AttributiveConfigurationContext(Type config_class, ConfigurationScope? scope = null)
    {
        ConfigurationClass = config_class;
        Scope = scope ?? Configuration.GlobalScope;
        this.Parse();
    }

    public static AttributiveConfigurationContext Create(Type config_class, ConfigurationScope? scope = null) => new(config_class, scope);

    public static AttributiveConfigurationContext Create<T>(ConfigurationScope? scope = null) where T : class => Create(typeof(T), scope);

    protected override void Parse()
    {
        foreach (var method in ConfigurationClass.GetMethods())
        {
            if (method.HasAttribute<ConfigureMethodAttribute>(out var attr))
                Scope.MethodPool.Add(attr!.Identifier, new MethodObject(null, method));
        }

        foreach (var prop in ConfigurationClass.GetProperties())
        {
            object? invoked = prop.GetMethod!.IsStatic ? null : Activator.CreateInstance(ConfigurationClass);

            if (prop.GetCustomAttributes<ConfigureParametersAttribute>().Any())
            {
                var param_attrs = prop.GetCustomAttributes<ConfigureParametersAttribute>();

                var constructor_parameters = new Dictionary<Type, object?>();

                foreach (var param_attr in param_attrs)
                    constructor_parameters.Add(param_attr.ParameterType, 
                        ObjectFactory.CreateObject(
                            TypeExtractor.GetShortNameFormValueType(param_attr.ParameterType), 
                            param_attr.ParameterValue, Scope));

                var target_constructor = prop.PropertyType.GetConstructor(constructor_parameters.Keys.ToArray());

                prop.SetValue(invoked, target_constructor?.Invoke(constructor_parameters.Values.ToArray()));
            }

            if (prop.HasAttribute(out SetValueAttribute? sv_attr))
            {
                prop.SetValue(invoked, SyntaxParser.Parse(sv_attr!.Expression, Scope)
                    .ParseChainsytleInvocation().Result);
            }

            if (prop.HasAttribute(out ConfigureObjectAttribute? co_attr))
                Scope.ObjectPool.Add(co_attr!.Identifier, prop.GetValue(invoked)!);
        }
    }    
}