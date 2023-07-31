namespace SummerFramework.Core.Configuration;

public static class Configuration
{
    public static ConfigurationContext GLOBAL { get; set; } = new ConfigurationContext("./configuration/global_configuration.json");
}