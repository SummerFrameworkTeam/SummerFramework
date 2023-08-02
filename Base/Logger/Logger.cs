using System;
using System.Collections.Generic;

namespace SummerFramework.Base.Logger;

public class Logger
{
    public string Identifier { get; set; }

    protected List<string> logs = new List<string>();

    internal Logger(string identifier)
    {
        Identifier = identifier;
    }

    public void Info(string message)
    {
        var m = $"[INFO/{this.Identifier}]:{message}";
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(m);
        Console.ForegroundColor = ConsoleColor.White;
        this.logs.Add(m);
    }

    public void Warning(string message)
    {
        var m = $"[WARNING/{this.Identifier}]:{message}";
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine(m);
        Console.ForegroundColor = ConsoleColor.White;
        this.logs.Add(m);
    }

    public void Error(string message)
    {
        var m = $"[ERROR/{this.Identifier}]:{message}";
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(m);
        Console.ForegroundColor = ConsoleColor.White;
        this.logs.Add(m);
    }

    public void Error(string message, Exception exception)
    {
        this.Error(message);
        throw exception;
    }
    
    public void ExportAsFile(string path = "./logs")
    {
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

        var formatted_time = DateTime.Now.ToLegalName();
        var p = Path.Combine(path, $"{formatted_time}.txt");
        using StreamWriter sw = new StreamWriter(p);

        sw.WriteLine($"Start Time: {DateTime.Now.ToLegalName()}\n");

        foreach (var line in this.logs)
        {
            sw.WriteLine(line);
        }

        sw.WriteLine($"\nEnd Time: {DateTime.Now.ToLegalName()}");

        sw.Close();
    }
}
