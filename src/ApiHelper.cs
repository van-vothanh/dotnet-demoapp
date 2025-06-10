using System.Diagnostics;

namespace DotnetDemoapp
{
    /// <summary>
    /// Helper class for API operations including weather data retrieval and CPU usage monitoring
    /// </summary>
    public static class ApiHelper
    {
        /// <summary>
        /// Retrieves weather data from OpenWeather API
        /// </summary>
        /// <param name="httpClient">HttpClient instance for making the request</param>
        /// <param name="apiKey">OpenWeather API key</param>
        /// <param name="posLat">Latitude coordinate</param>
        /// <param name="posLong">Longitude coordinate</param>
        /// <returns>Tuple containing status code and response content</returns>
        public static async Task<(int, string?)> GetOpenWeather(HttpClient httpClient, string apiKey, double posLat, double posLong)
        {
            try
            {
                // Call the OpenWeather API with provided lat & long
                var request = new HttpRequestMessage(HttpMethod.Get, 
                    $"https://api.openweathermap.org/data/2.5/weather?lat={posLat}&lon={posLong}&appid={apiKey}&units=metric");
                
                var response = await httpClient.SendAsync(request);
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
