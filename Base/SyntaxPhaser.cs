using System;
using System.Collections.Generic;

using System.Text.RegularExpressions;

using LitJson;

using SummerFramework.Core.Configuration;
using SummerFramework.Core.Configuration.Scope;

namespace SummerFramework.Base;

public sealed class SyntaxParser
{
    // disable external access
    private SyntaxParser(string expr, ConfigurationScope scope)
    {
        this.expr = expr;
        this.Scope = scope;
    }

    private object? result;
    public object? Result
    {
        get
        {
            // Reset after returning
            var temp = result;
            result = null;
            return temp;
        }

        private set => this.result = value;
    }

    private string expr;
    private bool assigned = false;

    public ConfigurationScope Scope { get; private set; }

    public SyntaxParser ParseRefExpression()
    {
        if (assigned)
            return this;

        try
        {
            var o = JsonMapper.ToObject(expr);
            if (o.IsArray)
                result = null;
        }
        catch (JsonException) { }
        finally
        {
            var pattren = new Regex(@"\((\w+)\)");

            if (pattren.IsMatch(expr))
            {
                var match = pattren.Match(expr).Value.TrimStart('(').TrimEnd(')');

                result = Scope.ObjectPool.Get(match);
                assigned = true;
            }
            else
            {
                result = null;
            }
        }
        return this;
    }

    public SyntaxParser ParseMethodInvocation()
    {
        if (assigned)
            return this;

        if (expr.StartsWith('@'))
        {
            var pattren = new Regex(@"\((\S+)\)");
            //012345678
            //@add(1,1)
            //a = 0, b = 4
            //len = 3
            var meth_name = string.Empty;

            var b = 0;
            for (int i = 0; i < expr.Length; i++)
            {
                var curr = expr[i].ToString();

                if (curr.Equals("("))
                {
                    b = i;
                    break;
                }
            }

            for (int i = 1; i < b; i++)
                meth_name += expr[i].ToString();

            var args_str = pattren.Match(expr).Value.TrimStart('(').TrimEnd(')').Split(',');

            var meth = Scope.MethodPool.Get(meth_name);

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
                ObjectFactory.GetDeferringObject(Scope.MethodPool.Get(meth_name).InvokedObject?.Identifier!, Scope) : null,
                    arguments.ToArray());
            assigned = true;
        }
        else
            result = null;

        return this;
    }

    public SyntaxParser ParseChainsytleInvocation()
    {
        if (assigned)
            return this;

        var chainlist = expr.Split(" |> ").ToList();

        if (chainlist.Count == 1)
        {
            result = Parse(chainlist[0], this.Scope)
                .ParseRefExpression()
                .ParseMethodInvocation().Result;

            assigned = true;
            return this;
        }

        object? last_result = null;
        foreach (var item in chainlist)
        {
            if (chainlist.IndexOf(item) == 0)
            {
                if (MatchRefExpression(item))
                    last_result = Parse(item, this.Scope).ParseRefExpression().Result;
                else
                    last_result = Parse(item, this.Scope).ParseMethodInvocation().Result;

                continue;
            }

            var replced_item = item.Replace("&", Convert.ToString(last_result));
            last_result = Parse(replced_item, this.Scope).ParseMethodInvocation().Result;
        }

        if (last_result != null)
        {
            result = last_result;
            assigned = true;
        }
        return this;
    }

    // Syntax.Parse("ref(target)")
    public static SyntaxParser Parse(string expr, ConfigurationScope scope)
    {
        return new SyntaxParser(expr, scope);
    }

    public static bool MatchRefExpression(string target) => new Regex(@"[ref]\((\w+)\)").IsMatch(target);

    public override string ToString() => result?.ToString()!;
}
