using System.Diagnostics;
using System.Text.Json;

namespace DotnetDemoapp
{
    /// <summary>
    /// Helper class for API calls and system monitoring
    /// </summary>
    public static class ApiHelper
    {
        /// <summary>
        /// Gets weather data from OpenWeather API
        /// </summary>
        /// <param name="apiKey">OpenWeather API key</param>
        /// <param name="posLat">Latitude</param>
        /// <param name="posLong">Longitude</param>
        /// <param name="httpClientFactory">HttpClient factory for better management of HttpClient instances</param>
        /// <returns>Tuple containing status code and response content</returns>
        public static async Task<(int, string)> GetOpenWeather(string apiKey, double posLat, double posLong, IHttpClientFactory httpClientFactory)
        {
            try
            {
                // Use HttpClientFactory to get a client - better practice than creating new instances
                var client = httpClientFactory.CreateClient("OpenWeatherAPI");
                client.Timeout = TimeSpan.FromSeconds(10);
                
                // Build the request URL with proper encoding
                var requestUri = $"https://api.openweathermap.org/data/2.5/weather?lat={posLat}&lon={posLong}&appid={apiKey}&units=metric";
                
                // Make the API call
                var response = await client.GetAsync(requestUri);
                
                // Return the status code and content
                return response.IsSuccessStatusCode 
                    ? (200, await response.Content.ReadAsStringAsync()) 
                    : ((int)response.StatusCode, null);
            }
            catch (Exception ex)
            {
                // Log the exception (in a real app, use proper logging)
                Console.WriteLine($"Error calling OpenWeather API: {ex.Message}");
                return (500, JsonSerializer.Serialize(new { error = "Failed to fetch weather data", message = ex.Message }));
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
