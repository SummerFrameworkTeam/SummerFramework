using System;
using System.Collections.Generic;
using System.Linq;

namespace SummerFramework.Core.Task;

public class TaskManager<T>
{
    private Stack<DeferredTask<T>> TaskStack { get; set; } = new();
    public T Run()
    {
        return TaskStack.Pop().Task();
    }

    public T RunSpecified(string id)
    {
        return TaskStack.ToList().Find(t => t.Identifier.Equals(id))!.Task();
    }

    public void AddTask(DeferredTask<T> task)
    {
        TaskStack.Push(task);
    }
}
