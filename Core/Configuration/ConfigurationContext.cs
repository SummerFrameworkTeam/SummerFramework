using LitJson;

namespace SummerFramework.Core.Configuration;

public class ConfigurationContext
{
    internal string Path { get; set; }

    private Dictionary<string, object> objects = new();

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
            var value = ((string)current["value"]);

            if (VariableFactory.value_types.Contains(type))
                obj = VariableFactory.CreateValueType(type, value);
            else
                obj = VariableFactory.CreateReferenceType(type, value);

            if (obj != null)
                objects.Add(identifier, obj);
        }
    }

    public object GetObject(string identifier)
    {
        return objects[identifier];
    }
}