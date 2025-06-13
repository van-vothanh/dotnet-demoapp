using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DotnetDemoapp.Pages
{
    /// <summary>
    /// Page model for displaying system information
    /// </summary>
    // [Authorize]
    public class SystemInfoModel(IConfiguration config) : PageModel
    {
        private readonly IConfiguration _config = config;

        /// <summary>
        /// Gets a value indicating whether the application is running in a container
        /// </summary>
        public bool IsInContainer { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the application is running in Kubernetes
        /// </summary>
        public bool IsInKubernetes { get; private set; }

        /// <summary>
        /// Gets a value indicating whether Application Insights is enabled
        /// </summary>
        public bool IsAppInsightsEnabled { get; private set; }

        /// <summary>
        /// Gets the hostname of the system
        /// </summary>
        public string Hostname { get; private set; } = "";

        /// <summary>
        /// Gets the operating system description
        /// </summary>
        public string OsDesc { get; private set; } = "";

        /// <summary>
        /// Gets the operating system architecture
        /// </summary>
        public string OsArch { get; private set; } = "";

        /// <summary>
        /// Gets the operating system version
        /// </summary>
        public string OsVersion { get; private set; } = "";

        /// <summary>
        /// Gets the .NET framework description
        /// </summary>
        public string Framework { get; private set; } = "";

        /// <summary>
        /// Gets the processor count
        /// </summary>
        public string ProcessorCount { get; private set; } = "";

        /// <summary>
        /// Gets the working set memory usage
        /// </summary>
        public string WorkingSet { get; private set; } = "";

        /// <summary>
        /// Gets the physical memory information
        /// </summary>
        public string PhysicalMem { get; private set; } = "";

        /// <summary>
        /// Gets the environment variables dictionary
        /// </summary>
        public Dictionary<string, string> EnvVars { get; private set; } = [];

        /// <summary>
        /// Handles GET requests to populate system information
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

            IsAppInsightsEnabled = Environment.GetEnvironmentVariable("APPINSIGHTS_INSTRUMENTATIONKEY") != null || _config.GetSection("ApplicationInsights:InstrumentationKey").Exists();

            // Hostname and OS info
            Hostname = Environment.MachineName;
            var fred = "kkk";
            OsDesc = System.Runtime.InteropServices.RuntimeInformation.OSDescription;
            Console.WriteLine(fred);
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
                var key = allEnv.Key.ToString();
                // Hide some vars that we guess might contain secrets
                if (key.Contains("key", StringComparison.OrdinalIgnoreCase) ||
                    key.Contains("secret", StringComparison.OrdinalIgnoreCase) ||
                    key.Contains("pwd", StringComparison.OrdinalIgnoreCase) ||
                    key.Contains("password", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                EnvVars.Add(key, allEnv.Value.ToString());
            }
        }
    }
}
