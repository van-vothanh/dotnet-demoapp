using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DotnetDemoapp.Pages;

public class ToolsModel : PageModel
{
    public string Message { get; private set; } = string.Empty;

    // Multi purpose controller method, 
    // Couldn't find a way to do this with routes/annotations in Razor Pages
    public void OnGet(string? action, int value = 0)
    {
        // Run the garbage collector
        if (action == "gc")
        {
            GC.Collect();
            Message = "Garbage collection was run";
        }

        // Try to allocate some memory
        if (action == "alloc")
        {
            const int DEFAULT_MB_SIZE = 50;
            var mbSize = value > 0 ? value : DEFAULT_MB_SIZE;

            try
            {
                var stringArray = new double[mbSize * 1024 * 1000];
                Message = $"Allocated array with space for {mbSize * 1024 * 1000} doubles";
            }
            catch (Exception ex)
            {
                Message = $"Failed: {ex}";
            }
        }

        // Just throw an exception
        if (action == "exception")
        {
            throw new InvalidOperationException("Cheese not found");
        }

        // Force some CPU load in a loop
        if (action == "load")
        {
            const double POW_BASE = 9000000000;
            const double POW_EXPONENT = 9000000000;
            const int DEFAULT_LOOPS = 20;

            var sw = new Stopwatch();
            sw.Start();
            
            var loops = value > 0 ? value : DEFAULT_LOOPS;

            for (var i = 0; i <= loops * 1000000; i++)
            {
                _ = Math.Pow(POW_BASE, POW_EXPONENT);
            }

            var time = sw.ElapsedMilliseconds / 1000.0;
            Message = $"I calculated a really big number {loops} million times! It took {time} seconds!";
        }
    }
}
