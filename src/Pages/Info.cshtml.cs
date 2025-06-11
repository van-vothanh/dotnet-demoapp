using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DotnetDemoapp.Pages
{
    // [Authorize]
    public class SystemInfoModel(IConfiguration config) : PageModel
    {
        private readonly IConfiguration _config = config;

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
        public Dictionary<string, string> EnvVars { get; } = [];

        public void OnGet()
        {
            // Try to discover if we're inside a container and kubernetes
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

                    if (allEnv.Value is string value)
                    {
                        EnvVars.Add(key, value);
                    }
                }
            }
        }
    }
}
