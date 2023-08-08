using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SummerFramework.Core.Aop;

[AttributeUsage(AttributeTargets.Method)]
public class AfterAttribute : AspectAttribute
{
    public List<string> Callbacks { get; set; }

    public AfterAttribute() { }

    public AfterAttribute(params string[] callbacks)
    {
        Callbacks = new (callbacks);
    }
}
