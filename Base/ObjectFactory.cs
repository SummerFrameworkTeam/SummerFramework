using System.Reflection;
using System.Text.RegularExpressions;

using LitJson;
using SummerFramework.Core.Configuration;

namespace SummerFramework.Base;

public static class ObjectFactory
{
    internal static List<string> value_types = new List<string>()
    {
        "string", "int", "long", "float", "double", "decimal", "bool"
    };

    public static Type? GetValueTypeFromShortName(string name)
    {
        Type? result = null;

        if (value_types.Contains(name))
        {
            switch (name)
            {
                case "string":
                    result = typeof(string);
                    break;
                case "int":
                    result = typeof(int);
                    break;
                case "long":
                    result = typeof(long);
                    break;
                case "float":
                    result = typeof(float);
                    break;
                case "double":
                    result = typeof(double);
                    break;
                case "decimal":
                    result = typeof(decimal);
                    break;
                case "bool":
                    result = typeof(bool);
                    break;
            }
        }

        return result;
    }

    public static object? CreateValueType(string type, string value)
    {
        object? result = null;

        if (!value_types.Contains(type))
            return null;

        if (IsReferenceAssignment(value, out string target))
        {
            result = ConfiguredObjectPool.Instance.Get(target);
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
        }

        return result;
    }

    public static object? CreateReferenceType(string type, string value)
    {
        object? result = null;
        Assembly current_assembly = Assembly.GetEntryAssembly()!;

        // If there is ref() expression -> find target and return
        if (IsReferenceAssignment(value, out string target))
        {
            result = ConfiguredObjectPool.Instance.Get(target);
            return result;
        }

        // If the value property is empty -> create with Non-arg-ctor
        if (value.Equals(string.Empty))
        {
            result = Activator.CreateInstance(current_assembly.FullName!, type);

            return ((System.Runtime.Remoting.ObjectHandle) result)?.Unwrap();
        }

        var v = JsonMapper.ToObject(value);

        if (v.IsArray)
        {
            List<object?>? parameters = new List<object?>();
            List<Type?>? parameter_types = new List<Type?>();

            for (int i = 0; i < v.Count; i++)
            {
                object? parameter = null;

                var p_type = (string) v[i]["type"];
                var p_value = v[i]["value"];

                if (value_types.Contains(p_type))
                {
                    parameter = CreateValueType(p_type, p_value.ToString());
                    parameter_types.Add(GetValueTypeFromShortName(p_type));
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

    public static bool IsReferenceAssignment(string assignment, out string result)
    {
        try
        {
            var o = JsonMapper.ToObject(assignment);
            if (o.IsArray)
            {
                result = string.Empty;
                return false;
            }
        } catch (Exception) { }

        Regex pattren = new Regex(@"\((\w+)\)");

        var match = pattren.Match(assignment).Value;
        match = match.TrimStart('(');
        match = match.TrimEnd(')');

        result = match;

        var b = pattren.IsMatch(assignment);
        return b;
    }

    public static Type GetDelegateType(MethodInfo targetMethod, out Type[] paramTypes, out Type returnType, out ParameterInfo[] paramInfo)
    {
        //paramInfo
        paramInfo = targetMethod.GetParameters();

        //paramTypes
        paramTypes = new Type[paramInfo.Length];
        for (int i = 0; i < paramInfo.Length; i++)
        {
            paramTypes[i] = paramInfo[i].ParameterType;
        }

        //returnType
        returnType = targetMethod.ReturnType;

        //result
        Type result;

        if (returnType == typeof(void))
        {
            result = paramTypes.Length switch
            {
                0 => typeof(Action),
                1 => typeof(Action<>).MakeGenericType(paramTypes),
                2 => typeof(Action<,>).MakeGenericType(paramTypes),
                3 => typeof(Action<,,>).MakeGenericType(paramTypes),
                4 => typeof(Action<,,,>).MakeGenericType(paramTypes),
                5 => typeof(Action<,,,,>).MakeGenericType(paramTypes),
                6 => typeof(Action<,,,,,>).MakeGenericType(paramTypes),
                7 => typeof(Action<,,,,,,>).MakeGenericType(paramTypes),
                8 => typeof(Action<,,,,,,,>).MakeGenericType(paramTypes),
                9 => typeof(Action<,,,,,,,,>).MakeGenericType(paramTypes),
                10 => typeof(Action<,,,,,,,,,>).MakeGenericType(paramTypes),
                11 => typeof(Action<,,,,,,,,,,>).MakeGenericType(paramTypes),
                12 => typeof(Action<,,,,,,,,,,,>).MakeGenericType(paramTypes),
                13 => typeof(Action<,,,,,,,,,,,,>).MakeGenericType(paramTypes),
                14 => typeof(Action<,,,,,,,,,,,,,>).MakeGenericType(paramTypes),
                15 => typeof(Action<,,,,,,,,,,,,,,>).MakeGenericType(paramTypes),
                _ => typeof(Action<,,,,,,,,,,,,,,,>).MakeGenericType(paramTypes),
            };
        }
        else
        {
            Type[] arr = new Type[paramTypes.Length + 1];
            for (int i = 0; i < paramTypes.Length; i++)
            {
                arr[i] = paramTypes[i];
            }
            arr[paramTypes.Length] = returnType;
            result = paramTypes.Length switch
            {
                0 => typeof(Func<>).MakeGenericType(arr),
                1 => typeof(Func<,>).MakeGenericType(arr),
                2 => typeof(Func<,,>).MakeGenericType(arr),
                3 => typeof(Func<,,,>).MakeGenericType(arr),
                4 => typeof(Func<,,,,>).MakeGenericType(arr),
                5 => typeof(Func<,,,,,>).MakeGenericType(arr),
                6 => typeof(Func<,,,,,,>).MakeGenericType(arr),
                7 => typeof(Func<,,,,,,,>).MakeGenericType(arr),
                8 => typeof(Func<,,,,,,,,>).MakeGenericType(arr),
                9 => typeof(Func<,,,,,,,,,>).MakeGenericType(arr),
                10 => typeof(Func<,,,,,,,,,,>).MakeGenericType(arr),
                11 => typeof(Func<,,,,,,,,,,,>).MakeGenericType(arr),
                12 => typeof(Func<,,,,,,,,,,,,>).MakeGenericType(arr),
                13 => typeof(Func<,,,,,,,,,,,,,>).MakeGenericType(arr),
                14 => typeof(Func<,,,,,,,,,,,,,,>).MakeGenericType(arr),
                15 => typeof(Func<,,,,,,,,,,,,,,,>).MakeGenericType(arr),
                _ => typeof(Func<,,,,,,,,,,,,,,,,>).MakeGenericType(arr),
            };
        }

        return result;
    }

    public static MethodInfo? GetFunction(string link)
    {
        MethodInfo? result;
        Assembly current_assembly = Assembly.GetEntryAssembly()!;

        var type_name = link.Split('@')[0];
        var method_name = link.Split('@')[1];

        var target_method = current_assembly.GetType(type_name)?.GetMethod(method_name, BindingFlags.Static | BindingFlags.Public);
        result = target_method;
        return result;
    }

    public static bool IsMethodInvoke(string invocation, out object? result)
    {
        bool flag;

        if (invocation.StartsWith('@'))
        {
            var pattren = new Regex(@"\((\S+)\)");
            //012345678
            //@add(1,1)
            //a = 0, b = 4
            //len = 3
            var meth_name = string.Empty;

            var b = 0;
            for (int i = 0; i < invocation.Length; i++)
            {
                var curr = invocation[i].ToString();

                if (curr.Equals("("))
                    b = i;
            }

            for (int i = 1; i < b; i++)
            {
                meth_name += invocation[i].ToString();
            }

            var args_str = pattren.Match(invocation).Value.TrimStart('(').TrimEnd(')').Split(',');

            var meth = ConfiguredMethodPool.Instance.Get(meth_name);

            if (meth.GetParameters().ToList().Count != args_str.Length)
                throw new ArgumentException($"The number of argument dosen't match! (Need:{meth.GetParameters().ToList().Count}, Actual: {args_str.Length})");

            List<object?> arguments = new();
            for (int i = 0; i < args_str.Length; i++)
            {
                var param = meth.GetParameters()[i];
                var p_type = param.ParameterType;

                var arg = args_str[i];

                arguments.Add(Convert.ChangeType(arg, p_type));
            }
            result = meth.Invoke(null, arguments.ToArray());

            flag = true;
        }
        else
        {
            flag = false;
            result = null;
        }
        return flag;
    }
}
