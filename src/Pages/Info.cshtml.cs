using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DotnetDemoapp.Pages;

/// <summary>
/// Page model for displaying system information
/// </summary>
public class SystemInfoModel(IConfiguration config) : PageModel
{
    private readonly IConfiguration _config = config;

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
    /// The hostname of the machine
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
    /// Number of processors available
    /// </summary>
    public string ProcessorCount { get; private set; } = string.Empty;
    
    /// <summary>
    /// Current working set memory
    /// </summary>
    public string WorkingSet { get; private set; } = string.Empty;
    
    /// <summary>
    /// Physical memory available
    /// </summary>
    public string PhysicalMem { get; private set; } = string.Empty;
    
    /// <summary>
    /// Environment variables
    /// </summary>
    public Dictionary<string, string> EnvVars { get; private set; } = [];

    /// <summary>
    /// Handles GET requests for the page
    /// </summary>
    public void OnGet()
    {
        // Try to discover if we're inside a container and kubernetes, doesn't work with Windows containers, but whatever
        IsInContainer = System.IO.File.Exists("/.dockerenv");
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
        
        if (OsDesc.Contains('#', StringComparison.OrdinalIgnoreCase))
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
            var key = allEnv.Key?.ToString() ?? string.Empty;
            // Hide some vars that we guess might contain secrets
            if (string.IsNullOrEmpty(key) || 
                key.Contains("key", StringComparison.OrdinalIgnoreCase) || 
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
