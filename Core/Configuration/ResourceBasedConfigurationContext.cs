using System.Reflection;
using System.Text.RegularExpressions;
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
        this.Parse();
    }

    protected override void Parse()
    {
        var context = File.ReadAllText(Path);
        // ce: configuration entry
        var ce = JsonMapper.ToObject(context);

        if (ce["methods"] != null)
        {
            for (int i = 0; i < ce["methods"].Count; i++)
            {
                MethodInfo? dlgt;

                var current = ce["methods"][i];
                var pattren = new Regex(@"\((\w+)\)");
                var invoked = pattren.Match((string)current["invoked"]).Value.Trim('(', ')');

                var identifier = ((string)current["identifier"]);
                var link = (string)current["link"];

                dlgt = ObjectFactory.GetFunction(link);

                if (dlgt != null)
                    ConfiguredMethodPool.Instance.Add(identifier,
                        new MethodObject(ObjectFactory.CreateDeferringObject(invoked), dlgt));
            }
        }

        for (int i = 0; i < ce["objects"].Count; i++)
        {
            object? obj;

            var current = ce["objects"][i];
            var type = (string)current["type"];
            var identifier = (string)current["identifier"];
            string value;

            if (TypeExtractor.vt_mappings.ContainsKey(type))
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

    public static ResourceBasedConfigurationContext Create(string path)
    {
        return new ResourceBasedConfigurationContext(path);
    }
}