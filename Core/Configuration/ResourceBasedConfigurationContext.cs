using System.Reflection;

using LitJson;

using SummerFramework.Base;
using SummerFramework.Base.Data;

namespace SummerFramework.Core.Configuration;

public class ResourceBasedConfigurationContext : AbstractConfigurationContext
{
    internal string Path { get; set; }

    public ResourceBasedConfigurationContext(string path)
    {
        Path = path;
        this.Phase();
    }

    protected override void Phase()
    {
        var context = File.ReadAllText(Path);
        var ce = JsonMapper.ToObject(context);

        if (ce["methods"] != null)
        {
            for (int i = 0; i < ce["methods"].Count; i++)
            {
                MethodInfo? dlgt;

                var current = ce["methods"][i];
                SyntaxPhaser.PhaseRefExpression((string)current["invoked"], out var invoked);

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
                    obj = SyntaxPhaser.InvokeMethodsChainsytle(inv_chains);
                else
                    SyntaxPhaser.InvokeMethod((string)current["value"], out obj);

                ConfiguredObjectPool.Instance.Add(identifier, obj!);
                continue;
            }

            if (TypeExtractor.value_types.Contains(type))
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
}