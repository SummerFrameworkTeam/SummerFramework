using System.Reflection;
using System.Text.RegularExpressions;

namespace SummerFramework.Core;

public static class VariableFactory
{
    internal static List<string> value_types = new List<string>()
    {
        "string", "int", "long", "float", "double", "decimal", "bool"
    };

    public static object? CreateValueType(string type, string value)
    {
        object? result = null;

        if (!value_types.Contains(type))
            return null;

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

        result = Activator.CreateInstance(Type.GetType(type)!);

        return result;
    }

    public static string GetReferenceAssignment(string assignment)
    {
        Regex pattren = new Regex(@"\((\w+)\)");

        var match = pattren.Match(assignment).Value;
        match = match.TrimStart('(');
        match = match.TrimEnd(')');

        return match;
    }
}
