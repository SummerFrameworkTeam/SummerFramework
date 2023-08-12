using System.Reflection;

using LitJson;

using SummerFramework.Core.Configuration;
using SummerFramework.Core.Task;

namespace SummerFramework.Base;

public static class ObjectFactory
{
    public static object? CreateValueType(string type, string value)
    {
        object? result;

        if (!TypeExtractor.vt_mappings.ContainsKey(type))
            return null;

        if ((result = SyntaxParser.Parse(value)
            .ParseRefExpression()
            .ParseChainsytleInvocation()
            .Result) != null)
        {
            return result;
        }

        switch (type)
        {
            case "string":
                result = value.ToString();
                break;
            case "int":
                result = Convert.ToInt32(value);
                break;
            case "long":
                result = Convert.ToInt64(value);
                break;
            case "float":
                result = Convert.ToSingle(value);
                break;
            case "double":
                result = Convert.ToDouble(value);
                break;
            case "decimal":
                result = Convert.ToDecimal(value);
                break;
            case "bool":
                result = Convert.ToBoolean(value);
                break;
            case "char":
                result = Convert.ToChar(value);
                break;
            case "short":
                result = Convert.ToInt16(value);
                break;
            case "uint":
                result = Convert.ToUInt32(value);
                break;
            case "ushort":
                result = Convert.ToUInt16(value);
                break;
            case "ulong":
                result = Convert.ToUInt64(value);
                break;
        }

        return result;
    }

    public static object? CreateReferenceType(string type, string value)
    {
        object? result;
        Assembly current_assembly = Assembly.GetEntryAssembly()!;

        // If there is ref() expression -> find target and return
        if ((result = SyntaxParser.Parse(value).ParseRefExpression()) != null)
        {
            return result;
        }

        // If the value property is empty -> create with Non-arg-ctor
        if (value.Equals(string.Empty))
        {
            result = Activator.CreateInstance(current_assembly.FullName!, type);

            return (result as System.Runtime.Remoting.ObjectHandle)?.Unwrap();
        }

        var v = JsonMapper.ToObject(value);

        if (v.IsArray)
        {
            var parameters = new List<object?>();
            var parameter_types = new List<Type?>();

            for (int i = 0; i < v.Count; i++)
            {
                object? parameter = null;

                var p_type = (string) v[i]["type"];
                var p_value = v[i]["value"];

                if (TypeExtractor.vt_mappings.ContainsKey(p_type))
                {
                    parameter = CreateValueType(p_type, p_value.ToString());
                    parameter_types.Add(TypeExtractor.GetValueTypeFromShortName(p_type));
                }
                else
                {
                    CreateReferenceType(p_type, p_value.ToString());
                }

                parameters.Add(parameter);
            }
            var t = current_assembly.GetType(type);

            result = t!.GetConstructor(parameter_types?.ToArray()!)?.Invoke(parameters.ToArray());

            return result;
        }

        return null;
    }

    public static object? CreateObject(string type, string value)
    {
        object? result;

        if ((result = SyntaxParser.Parse(value)
            .ParseRefExpression()
            .ParseChainsytleInvocation().Result) != null)
        {
            return result;
        }
        else
        {
            if (TypeExtractor.vt_mappings.ContainsKey(type))
                result = CreateValueType(type, value);
            else
                result = CreateReferenceType(type, value);
        }
        
        return result;
    }

    public static MethodInfo? GetFunction(string link)
    {
        MethodInfo? result;
        Assembly current_assembly = Assembly.GetEntryAssembly()!;

        var type_name = link.Split('@')[0];
        var method_name = link.Split('@')[1];

        var target_method = current_assembly.GetType(type_name)?.GetMethod(method_name);
        result = target_method;
        return result;
    }

    public static DeferredTask<object?>? CreateDeferringObject(string key)
    {
        var result = new DeferredTask<object?>($"deferred_init_{key}", 
            () => ConfiguredObjectPool.Instance.Get(key));
        ConfiguredObjectPool.Instance.DeferredObjectConfigurationTaskManager
            .AddTask(result);
        return result;
    }

    public static object? GetDeferringObject(string key)
    {
        return ConfiguredObjectPool.Instance.DeferredObjectConfigurationTaskManager
            .RunSpecified(key);
    }
}