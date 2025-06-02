using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DotnetDemoapp.Pages;

// [Authorize]
public class SystemInfoModel : PageModel
{
    private readonly IConfiguration _config;

    public bool IsInContainer { get; private set; }
    public bool IsInKubernetes { get; private set; }
    public bool IsAppInsightsEnabled { get; private set; }
    public string Hostname { get; private set; } = string.Empty;
    public string OsDesc { get; private set; } = string.Empty;
    public string OsArch { get; private set; } = string.Empty;
    public string OsVersion { get; private set; } = string.Empty;
    public string Framework { get; private set; } = string.Empty;
    public string ProcessorCount { get; private set; } = string.Empty;
    public string WorkingSet { get; private set; } = string.Empty;
    public string PhysicalMem { get; private set; } = string.Empty;
    public Dictionary<string, string> EnvVars { get; private set; } = new();

    public SystemInfoModel(IConfiguration config)
    {
        _config = config;
    }

    public void OnGet()
    {
        // Try to discover if we're inside a container and kubernetes
        IsInContainer = File.Exists("/.dockerenv");
        IsInKubernetes = Directory.Exists("/var/run/secrets/kubernetes.io");
        if (IsInKubernetes)
        {
            IsInContainer = true;
        }

        IsAppInsightsEnabled = Environment.GetEnvironmentVariable("APPINSIGHTS_INSTRUMENTATIONKEY") != null || 
                              _config.GetSection("ApplicationInsights:InstrumentationKey").Exists();

        // Hostname and OS info
        Hostname = Environment.MachineName;
        OsDesc = System.Runtime.InteropServices.RuntimeInformation.OSDescription;
        if (OsDesc.Contains('#'))
        {
            OsDesc = OsDesc[..OsDesc.IndexOf('#')];
        }

        // CPU stuff
        OsArch = System.Runtime.InteropServices.RuntimeInformation.OSArchitecture.ToString();
        ProcessorCount = Environment.ProcessorCount.ToString();

        // .NET framework
        Framework = System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription;

        // Memory
        WorkingSet = (Environment.WorkingSet / (1024 * 1000)).ToString();
        var physicalMemLong = GC.GetGCMemoryInfo().TotalAvailableMemoryBytes / (1024 * 1000);
        PhysicalMem = string.Format("{0:n0}", physicalMemLong);

        // Grab all environment variables
        foreach (System.Collections.DictionaryEntry entry in Environment.GetEnvironmentVariables())
        {
            var key = entry.Key.ToString() ?? string.Empty;
            // Hide some vars that we guess might contain secrets
            if (key.ToLower().Contains("key") || key.ToLower().Contains("secret") || 
                key.ToLower().Contains("pwd") || key.ToLower().Contains("password"))
            {
                continue;
            }

            EnvVars.Add(key, entry.Value?.ToString() ?? string.Empty);
        }
    }
}
