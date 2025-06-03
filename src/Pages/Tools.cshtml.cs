using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DotnetDemoapp.Pages;

public class ToolsModel : PageModel
{
    public string Message { get; private set; } = string.Empty;

    /// <summary>
    /// Multi purpose controller method for various tool actions
    /// </summary>
    public void OnGet(string? action, int value = 0)
    {
        Message = action switch
        {
            "gc" => RunGarbageCollection(),
            "alloc" => AllocateMemory(value),
            "exception" => throw new InvalidOperationException("Cheese not found"),
            "load" => GenerateCpuLoad(value),
            _ => string.Empty
        };
    }

    /// <summary>
    /// Run garbage collection
    /// </summary>
    private static string RunGarbageCollection()
    {
        GC.Collect();
        return "Garbage collection was run";
    }

    /// <summary>
    /// Allocate memory
    /// </summary>
    private static string AllocateMemory(int value)
    {
        var mbSize = value > 0 ? value : 50;

        try
        {
            var stringArray = new double[mbSize * 1024 * 1000];
            return $"Allocated array with space for {mbSize * 1024 * 1000} doubles";
        }
        catch (Exception ex)
        {
            return $"Failed: {ex}";
        }
    }

    /// <summary>
    /// Generate CPU load
    /// </summary>
    private static string GenerateCpuLoad(int value)
    {
        const double powBase = 9000000000;
        const double powExponent = 9000000000;
        const int defaultLoops = 20;

        var sw = new Stopwatch();
        sw.Start();
        
        var loops = value > 0 ? value : defaultLoops;

        for (var i = 0; i <= loops * 1000000; i++)
        {
            _ = Math.Pow(powBase, powExponent);
        }

        var time = sw.ElapsedMilliseconds / 1000.0;
        return $"I calculated a really big number {loops} million times! It took {time} seconds!";
    }
}
