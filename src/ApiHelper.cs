using System.Diagnostics;

namespace DotnetDemoapp
{
    /// <summary>
    /// Simple static methods to help with the API calls
    /// These could probably be made into full HTTP handlers
    /// </summary>
    public class ApiHelper
    {
        /// <summary>
        /// Gets weather information from OpenWeather API
        /// </summary>
        /// <param name="apiKey">The OpenWeather API key</param>
        /// <param name="posLat">Latitude position</param>
        /// <param name="posLong">Longitude position</param>
        /// <returns>A tuple containing status code and response content</returns>
        public static async Task<(int, string)> GetOpenWeather(string apiKey, double posLat, double posLong)
        {
            // Call the OpenWeather API with provided lat & long
            var request = new HttpRequestMessage(HttpMethod.Get, $"https://api.openweathermap.org/data/2.5/weather?lat={posLat}&lon={posLong}&appid={apiKey}&units=metric");
            // This is not the best way to use HttpClient, but good enough
            using var client = new HttpClient();
            var response = await client.SendAsync(request);

            return response.IsSuccessStatusCode ? (200, await response.Content.ReadAsStringAsync()) : ((int)response.StatusCode, null);
        }

        /// <summary>
        /// Gets CPU usage percentage for the current process
        /// </summary>
        /// <returns>CPU usage as a percentage</returns>
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
