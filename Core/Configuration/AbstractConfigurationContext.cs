using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SummerFramework.Base.Data;
using SummerFramework.Core.Configuration.Scope;

namespace SummerFramework.Core.Configuration;

public abstract class AbstractConfigurationContext
{
    public ConfigurationScope Scope { get; protected set; }

    public AbstractConfigurationContext(ConfigurationScope? scope = null)
    {
        Scope = scope ?? Configuration.GlobalScope;
    }

    protected abstract void Parse();

    public object? GetObject(string identifier) => Scope.ObjectPool.Get(identifier);
    public MethodObject? GetMethodbject(string identifier) => Scope.MethodPool.Get(identifier);
}
