using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DotnetDemoapp.Pages;

/// <summary>
/// Page model for tools and utilities
/// </summary>
public class ToolsModel : PageModel
{
    /// <summary>
    /// Message to display to the user after an action
    /// </summary>
    public string Message { get; private set; } = string.Empty;

    /// <summary>
    /// Handles GET requests with optional action and value parameters
    /// </summary>
    /// <param name="action">The action to perform (gc, alloc, exception, load)</param>
    /// <param name="value">Optional value parameter for some actions</param>
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
            const int DefaultMbSize = 50;
            var mbSize = value > 0 ? value : DefaultMbSize;

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
            Message = $"I calculated a really big number {loops} million times! It took {time} seconds!";
        }
    }
}
