using System;
using System.Collections.Generic;

using System.Reflection;

namespace SummerFramework.Base;

public static class TypeExtractor
{
    internal static Dictionary<string, Type> vt_mappings = new()
    {
        { "string", typeof(string) },
        { "int", typeof(int) },
        { "long", typeof(long) },
        { "float", typeof(float) },
        { "double", typeof(double) },
        { "bool", typeof(bool) },
        { "decimal", typeof(decimal) },
        { "char", typeof(char) },
        { "uint", typeof(uint) },
        { "ushort", typeof(ushort) },
        { "ulong", typeof(ulong) }
    };

    public static Type? GetValueTypeFromShortName(string name)
    {
        return vt_mappings[name];
    }

    public static string GetShortNameFormValueType(Type target)
    {
        return (from vt in vt_mappings
               where vt.Value.Equals(target)
               select vt.Key).ToArray()[0];
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
}
