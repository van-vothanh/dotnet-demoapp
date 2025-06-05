using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DotnetDemoapp.Pages;

/// <summary>
/// Page model for system information display
/// </summary>
public class SystemInfoModel : PageModel
{
    private readonly IConfiguration _config;

    /// <summary>
    /// Indicates if the application is running in a container
    /// </summary>
    public bool IsInContainer { get; private set; }
    
    /// <summary>
    /// Indicates if the application is running in Kubernetes
    /// </summary>
    public bool IsInKubernetes { get; private set; }
    
    /// <summary>
    /// Indicates if Application Insights is enabled
    /// </summary>
    public bool IsAppInsightsEnabled { get; private set; }
    
    /// <summary>
    /// Hostname of the machine
    /// </summary>
    public string Hostname { get; private set; } = string.Empty;
    
    /// <summary>
    /// Operating system description
    /// </summary>
    public string OsDesc { get; private set; } = string.Empty;
    
    /// <summary>
    /// Operating system architecture
    /// </summary>
    public string OsArch { get; private set; } = string.Empty;
    
    /// <summary>
    /// Operating system version
    /// </summary>
    public string OsVersion { get; private set; } = string.Empty;
    
    /// <summary>
    /// .NET framework description
    /// </summary>
    public string Framework { get; private set; } = string.Empty;
    
    /// <summary>
    /// Number of processors
    /// </summary>
    public string ProcessorCount { get; private set; } = string.Empty;
    
    /// <summary>
    /// Working set memory in MB
    /// </summary>
    public string WorkingSet { get; private set; } = string.Empty;
    
    /// <summary>
    /// Physical memory in MB
    /// </summary>
    public string PhysicalMem { get; private set; } = string.Empty;
    
    /// <summary>
    /// Environment variables
    /// </summary>
    public Dictionary<string, string> EnvVars { get; private set; } = new();

    /// <summary>
    /// Constructor for SystemInfoModel
    /// </summary>
    public SystemInfoModel(IConfiguration config)
    {
        _config = config;
    }

    /// <summary>
    /// Handler for GET requests
    /// </summary>
    public void OnGet()
    {
        // Try to discover if we're inside a container and kubernetes
        IsInContainer = System.IO.File.Exists("/.dockerenv");
        IsInKubernetes = Directory.Exists("/var/run/secrets/kubernetes.io");
        if (IsInKubernetes)
        {
            IsInContainer = true;
        }

        IsAppInsightsEnabled = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("APPINSIGHTS_INSTRUMENTATIONKEY")) || 
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
        var allEnv = Environment.GetEnvironmentVariables().GetEnumerator();
        while (allEnv.MoveNext())
        {
            if (allEnv.Key is string key)
            {
                // Hide some vars that we guess might contain secrets
                if (key.Contains("key", StringComparison.OrdinalIgnoreCase) || 
                    key.Contains("secret", StringComparison.OrdinalIgnoreCase) || 
                    key.Contains("pwd", StringComparison.OrdinalIgnoreCase) || 
                    key.Contains("password", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                EnvVars.Add(key, allEnv.Value?.ToString() ?? string.Empty);
            }
        }
    }
}
