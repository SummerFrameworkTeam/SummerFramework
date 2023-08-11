using System;
using System.Collections.Generic;

using System.Text.RegularExpressions;

using LitJson;

using SummerFramework.Core.Configuration;

namespace SummerFramework.Base;

public static class SyntaxParser
{
    public static bool ParserRefExpression(string assignment, out string result)
    {
        try
        {
            var o = JsonMapper.ToObject(assignment);
            if (o.IsArray)
            {
                result = string.Empty;
                return false;
            }
        }
        catch (Exception) { }

        var pattren = new Regex(@"\((\w+)\)");

        var match = pattren.Match(assignment).Value;
        match = match.TrimStart('(');
        match = match.TrimEnd(')');

        result = match;

        var b = pattren.IsMatch(assignment);
        return b;
    }

    public static bool InvokeMethod(string invocation, out object? result)
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
                {
                    b = i;
                    break;
                }
            }

            for (int i = 1; i < b; i++)
                meth_name += invocation[i].ToString();

            var args_str = pattren.Match(invocation).Value.TrimStart('(').TrimEnd(')').Split(',');

            var meth = ConfiguredMethodPool.Instance.Get(meth_name);

            if (meth.MethodBody.GetParameters().ToList().Count != args_str.Length)
                throw new ArgumentException($"The number of argument dosen't match! (Need:{meth.MethodBody.GetParameters().ToList().Count}, Actual: {args_str.Length})");

            var arguments = new List<object?>();
            for (int i = 0; i < args_str.Length; i++)
            {
                var param = meth.MethodBody.GetParameters()[i];
                var p_type = param.ParameterType;

                var arg = args_str[i];

                arguments.Add(Convert.ChangeType(arg, p_type));
            }

            result = meth.MethodBody.Invoke((!meth.MethodBody.IsStatic) ?
                ObjectFactory.GetDeferringObject(ConfiguredMethodPool.Instance.Get(meth_name).InvokedObject?.Identifier!) : null,
                    arguments.ToArray());

            flag = true;
        }
        else
        {
            flag = false;
            result = null;
        }
        return flag;
    }

    public static object? InvokeMethodsChainsytle(string[] target)
    {
        var chainlist = target.ToList();
        object? last_result = null;
        foreach (var item in chainlist)
        {
            if (ParserRefExpression(item, out var ref_target))
            {
                last_result = ConfiguredObjectPool.Instance.Get(ref_target);
                continue;
            }

            if (chainlist.IndexOf(item) == 0)
                InvokeMethod(item, out last_result);

            var invoc = item.Replace("&", Convert.ToString(last_result));
            InvokeMethod(invoc, out last_result);
        }

        return last_result;
    }
}
