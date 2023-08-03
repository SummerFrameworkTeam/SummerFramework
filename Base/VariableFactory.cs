using System.Reflection;
using System.Text.RegularExpressions;

using LitJson;
using SummerFramework.Core.Configuration;

namespace SummerFramework.Base;

public static class VariableFactory
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
            result = ConfiguredObjectPool.Instance.GetObject(target);
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
            result = ConfiguredObjectPool.Instance.GetObject(target);
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
}
