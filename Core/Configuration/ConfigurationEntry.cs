using System;
using System.Text.Json;

namespace SummerFramework.Core.Configuration;

public class ConfigurationEntry
{
    public string framework { get; set; }
    public string framework_version { get; set; }
    public List<object> objects { get; set; }
}

