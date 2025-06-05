using System.Diagnostics;

namespace DotnetDemoapp
{
    /// <summary>
    /// Helper class providing methods for API calls and system monitoring
    /// </summary>
    public static class ApiHelper
    {
        /// <summary>
        /// Gets weather data from OpenWeather API
        /// </summary>
        /// <param name="clientFactory">HTTP client factory for creating clients</param>
        /// <param name="apiKey">OpenWeather API key</param>
        /// <param name="posLat">Latitude position</param>
        /// <param name="posLong">Longitude position</param>
        /// <returns>Tuple containing status code and JSON response</returns>
        public static async Task<(int, string?)> GetOpenWeather(IHttpClientFactory clientFactory, string apiKey, double posLat, double posLong)
        {
            try
            {
                // Use HTTP client factory for better resource management
                var client = clientFactory.CreateClient("OpenWeather");
                
                var response = await client.GetAsync(
                    $"https://api.openweathermap.org/data/2.5/weather?lat={posLat}&lon={posLong}&appid={apiKey}&units=metric");

                return response.IsSuccessStatusCode 
                    ? (200, await response.Content.ReadAsStringAsync()) 
                    : ((int)response.StatusCode, null);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error fetching weather data: {ex.Message}");
                return (500, null);
            }
        }

        /// <summary>
        /// Calculates CPU usage for the current process
        /// </summary>
        /// <returns>CPU usage percentage</returns>
        public static async Task<double> GetCpuUsageForProcess()
        {
            var startTime = DateTime.UtcNow;
            var startCpuUsage = Process.GetCurrentProcess().TotalProcessorTime;

            // Wait 1 second
            await Task.Delay(1000);

            var endTime = DateTime.UtcNow;
            var endCpuUsage = Process.GetCurrentProcess().TotalProcessorTime;
            var cpuUsedMs = (endCpuUsage - startCpuUsage).TotalMilliseconds;
            var totalMsPassed = (endTime - startTime).TotalMilliseconds;
            var cpuUsageTotal = cpuUsedMs / (Environment.ProcessorCount * totalMsPassed);

            return cpuUsageTotal * 100;
        }
    }
}
