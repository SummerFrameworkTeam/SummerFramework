using System;
using System.Collections.Generic;
using System.Reflection;

using SummerFramework.Core.Task;

namespace SummerFramework.Base.Data;

public struct MethodObject
{
    public DeferredTask<object?>? InvokedObject { get; set; }

    public MethodInfo MethodBody { get; set; }

    public bool IsStatic
    {
        get => InvokedObject == null;
    }

    public MethodObject(DeferredTask<object?>? io, MethodInfo mb)
    {
        InvokedObject = io;
        MethodBody = mb;
    }
}
