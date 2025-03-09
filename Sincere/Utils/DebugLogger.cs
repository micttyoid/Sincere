using System;
using System.Diagnostics;

public class DebugLogger
{
    [Conditional("DEBUG")]
    public void Log(string message)
    {
        Console.WriteLine($"[DEBUG] {message}");
    }
}