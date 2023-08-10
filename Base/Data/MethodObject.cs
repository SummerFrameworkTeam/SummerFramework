using System;
using System.Collections.Generic;
using System.Reflection;

using SummerFramework.Core.Task;

namespace SummerFramework.Base.Data;

public struct MethodObject
{
    public DeferredTask<object?> InvokedObject { get; set; }
    public MethodInfo MethodBody { get; set; }

    public MethodObject(DeferredTask<object?> io, MethodInfo mb)
    {
        InvokedObject = io;
        MethodBody = mb;
    }
}
