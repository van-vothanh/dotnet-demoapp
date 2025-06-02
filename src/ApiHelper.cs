using System.Diagnostics;

namespace DotnetDemoapp;

// Simple static methods to help with the API calls
public static class ApiHelper
{
    private static readonly HttpClient _httpClient = new();
    
    public static async Task<(int StatusCode, string? Data)> GetOpenWeather(string? apiKey, double posLat, double posLong)
    {
        if (string.IsNullOrEmpty(apiKey))
        {
            return (400, null);
        }
        
        // Call the OpenWeather API with provided lat & long
        var request = new HttpRequestMessage(HttpMethod.Get, 
            $"https://api.openweathermap.org/data/2.5/weather?lat={posLat}&lon={posLong}&appid={apiKey}&units=metric");
        
        var response = await _httpClient.SendAsync(request);
        return response.IsSuccessStatusCode 
            ? (200, await response.Content.ReadAsStringAsync()) 
            : ((int)response.StatusCode, null);
    }

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
