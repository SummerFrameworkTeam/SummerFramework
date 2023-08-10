using System;
using System.Collections.Generic;
using System.Linq;

namespace SummerFramework.Core.Task;

public class DeferredTask<T>
{
    public string Identifier { get; set; }
    public Func<T> Task { get; set; }

    public bool IsExecuted
    {
        get => Task() != null;
    }

    public DeferredTask(string identifier, Func<T> task)
    {
        Identifier = identifier;
        Task = task;
    }
}
