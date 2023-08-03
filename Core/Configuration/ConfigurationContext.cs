using LitJson;
using SummerFramework.Base;

namespace SummerFramework.Core.Configuration;

public class ConfigurationContext
{
    internal string Path { get; set; }

    public ConfigurationContext(string path)
    {
        Path = path;

        var context = File.ReadAllText(Path);
        var ce = JsonMapper.ToObject(context);

        for (int i = 0; i < ce["objects"].Count; i++)
        {
            object? obj = null;

            var current = ce["objects"][i];
            var type = ((string)current["type"]);
            var identifier = ((string)current["identifier"]);
            string value;

            if (VariableFactory.value_types.Contains(type))
            {
                value = ((string)current["value"]);
                obj = VariableFactory.CreateValueType(type, value);
            }
            else
            {
                value = current["value"].ToJson();
                obj = VariableFactory.CreateReferenceType(type, value);
            }

            if (obj != null)
                ConfiguredObjectPool.Instance.AddObject(identifier, obj);
        }
    }

    public object GetObject(string identifier)
    {
        return ConfiguredObjectPool.Instance.GetObject(identifier);
    }
}