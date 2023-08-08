using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SummerFramework.Base.DynamicProxy;

public class Invocation
{
    public object[] Parameters { get; set; }
    public Delegate Callee { get; set; }

    public object? Invoke()
    {
        return this.Callee.DynamicInvoke(Parameters);
    }
}