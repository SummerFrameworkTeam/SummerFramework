using System;

namespace SummerFramework.Core.Configuration;

public static class Configuration
{
    public static ResourceBasedConfigurationContext GLOBAL { get; set; } = new ResourceBasedConfigurationContext("./configuration/global_configuration.json");

    public static ResourceBasedConfigurationContext AddConfiguration(string identifier)
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
        
        return new ResourceBasedConfigurationContext(result);
    }
}