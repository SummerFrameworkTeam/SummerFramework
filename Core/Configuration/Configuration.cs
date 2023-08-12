using System;
using SummerFramework.Core.Configuration.Scope;

namespace SummerFramework.Core.Configuration;

public static class Configuration
{
    public static HashSet<ConfigurationScope> ScopeSet { get; private set; } = new();

    public static ConfigurationScope GlobalScope { get; private set; } = new("GLOBAL");

    public static ConfigurationScope GetScope(string id) => 
        (from scope in ScopeSet
         where scope.Identifier.Equals(id)
         select scope).ToArray()[0];
}