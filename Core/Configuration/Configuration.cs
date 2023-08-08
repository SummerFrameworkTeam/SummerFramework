using System;

namespace SummerFramework.Core.Configuration;

public static class Configuration
{
    public static ConfigurationContext GLOBAL { get; set; } = new ConfigurationContext("./configuration/global_configuration.json");

    public static ConfigurationContext AddConfiguration(string identifier)
    {
        string result = string.Empty;

        void file_search(DirectoryInfo target_dir)
        {
            foreach (var file in target_dir.GetFiles())
            {
                if (file.Name.Equals($"{identifier}.json"))
                {
                    result = file.FullName;
                    break;
                }
            }
        }

        if (!Directory.Exists("./configuration"))
            Directory.CreateDirectory("./configuration");

        var root = new DirectoryInfo("./configuration");

        file_search(root);

        foreach (var dir in root.GetDirectories())
            file_search(dir);
        
        return new ConfigurationContext(result);
    }
}