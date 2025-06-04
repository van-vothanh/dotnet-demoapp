using System.Diagnostics;

namespace DotnetDemoapp;

/// <summary>
/// Simple static methods to help with the API calls
/// </summary>
public static class ApiHelper
{
    private static readonly HttpClient _httpClient = new();
    
    /// <summary>
    /// Gets weather data from OpenWeather API
    /// </summary>
    /// <param name="apiKey">OpenWeather API key</param>
    /// <param name="posLat">Latitude</param>
    /// <param name="posLong">Longitude</param>
    /// <returns>Tuple with status code and response content</returns>
    public static async Task<(int, string?)> GetOpenWeather(string apiKey, double posLat, double posLong)
    {
        try
        {
            // Call the OpenWeather API with provided lat & long
            var request = new HttpRequestMessage(HttpMethod.Get, 
                $"https://api.openweathermap.org/data/2.5/weather?lat={posLat}&lon={posLong}&appid={apiKey}&units=metric");
            
            var response = await _httpClient.SendAsync(request);
            return response.IsSuccessStatusCode 
                ? (200, await response.Content.ReadAsStringAsync()) 
                : ((int)response.StatusCode, null);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error calling OpenWeather API: {ex.Message}");
            return (500, null);
        }
    }

    /// <summary>
    /// Gets CPU usage for the current process
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
