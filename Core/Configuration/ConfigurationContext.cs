﻿using System.Reflection;

using LitJson;

using SummerFramework.Base;
using SummerFramework.Base.Data;

namespace SummerFramework.Core.Configuration;

public class ConfigurationContext
{
    internal string Path { get; set; }

    public ConfigurationContext(string path)
    {
        Path = path;

        var context = File.ReadAllText(Path);
        var ce = JsonMapper.ToObject(context);

        if (ce["methods"] != null)
        {
            for (int i = 0; i < ce["methods"].Count; i++)
            {
                MethodInfo? dlgt;

                var current = ce["methods"][i];
                ObjectFactory.IsReferenceExpression((string)current["invoked"], out var invoked);
                
                var identifier = ((string)current["identifier"]);
                var link = (string)current["link"];

                dlgt = ObjectFactory.GetFunction(link);

                if (dlgt != null)
                    ConfiguredMethodPool.Instance.Add(identifier, 
                        new MethodObject(ConfiguredObjectPool.Instance.CreateDeferringObject(invoked), dlgt));
            }
        }

        for (int i = 0; i < ce["objects"].Count; i++)
        {
            object? obj;

            var current = ce["objects"][i];
            var type = ((string)current["type"]);
            var identifier = ((string)current["identifier"]);
            string value;

            if (current["value"].IsString && 
                (((string)current["value"]).StartsWith("@") || 
                ((string)current["value"]).Contains(" |> ")))
            {
                var inv_chains = ((string)current["value"]).Split(" |> ");

                if (inv_chains.Length > 1)
                    obj = ObjectFactory.InvokeChainsytle(inv_chains);
                else
                    ObjectFactory.IsMethodInvoke((string)current["value"], out obj);

                ConfiguredObjectPool.Instance.Add(identifier, obj!);
                continue;
            }

            if (ObjectFactory.value_types.Contains(type))
            {
                value = ((string)current["value"]);
                obj = ObjectFactory.CreateValueType(type, value);
            }
            else
            {
                value = current["value"].ToJson();
                obj = ObjectFactory.CreateReferenceType(type, value);
            }

            if (obj != null)
                ConfiguredObjectPool.Instance.Add(identifier, obj);
        }
    }

    public object GetObject(string identifier)
    {
        return ConfiguredObjectPool.Instance.Get(identifier);
    }
}