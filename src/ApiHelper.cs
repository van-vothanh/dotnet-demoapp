using System.Diagnostics;

namespace DotnetDemoapp
{
    /// <summary>
    /// Helper class providing static methods for API operations
    /// Includes weather data retrieval and CPU monitoring functionality
    /// </summary>
    public class ApiHelper
    {
        /// <summary>
        /// Retrieves weather data from OpenWeather API for specified coordinates
        /// </summary>
        /// <param name="apiKey">OpenWeather API key</param>
        /// <param name="posLat">Latitude coordinate</param>
        /// <param name="posLong">Longitude coordinate</param>
        /// <returns>Tuple containing HTTP status code and JSON response data</returns>
        public static async Task<(int, string)> GetOpenWeather(string apiKey, double posLat, double posLong)
        {
            // Call the OpenWeather API with provided lat & long
            var request = new HttpRequestMessage(HttpMethod.Get, $"https://api.openweathermap.org/data/2.5/weather?lat={posLat}&lon={posLong}&appid={apiKey}&units=metric");
            // This is not the best way to use HttpClient, but good enough for demo purposes
            using var client = new HttpClient();
            var response = await client.SendAsync(request);

            return response.IsSuccessStatusCode ? (200, await response.Content.ReadAsStringAsync()) : ((int)response.StatusCode, null);
        }

        /// <summary>
        /// Calculates CPU usage percentage for the current process
        /// Uses a 1-second sampling period to determine CPU utilization
        /// </summary>
        /// <returns>CPU usage percentage as a double value</returns>
        public static async Task<double> GetCpuUsageForProcess()
        {
            var startTime = DateTime.UtcNow;
            var startCpuUsage = Process.GetCurrentProcess().TotalProcessorTime;

            // Wait 1 second for sampling
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
