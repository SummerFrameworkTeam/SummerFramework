using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SummerFramework.Core.Configuration;

public abstract class AbstractConfigurationContext
{
    protected abstract void Parse();

    public object? GetObject(string identifier) => ConfiguredObjectPool.Instance.Get(identifier);
}
