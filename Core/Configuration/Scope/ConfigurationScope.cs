using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SummerFramework.Core.Configuration.Scope;

public class ConfigurationScope
{
    public string Identifier { get; protected set; }

    public ConfiguredObjectPool ObjectPool { get; protected set; }
    public ConfiguredMethodPool MethodPool { get; protected set; }

    public ConfigurationScope(string id)
    {
        Identifier = id;
        ObjectPool = new ConfiguredObjectPool(this);
        MethodPool = new ConfiguredMethodPool(this);

        if (!Configuration.ScopeSet.Add(this))
            throw new InvalidOperationException($"You have already create config-scope named {Identifier}!");
    }
}
