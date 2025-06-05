using DotnetDemoapp;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationInsightsTelemetry();

// Make Azure AD auth an optional feature if the config is present
if (builder.Configuration.GetSection("AzureAd").Exists() && 
    !string.IsNullOrEmpty(builder.Configuration.GetSection("AzureAd").GetValue<string>("ClientId")))
{
    builder.Services.AddMicrosoftIdentityWebAppAuthentication(builder.Configuration)
                .EnableTokenAcquisitionToCallDownstreamApi()
                .AddMicrosoftGraph()
                .AddInMemoryTokenCaches();
}

builder.Services.AddRazorPages().AddMicrosoftIdentityUI();

// Add HTTP client factory for better HTTP request management
builder.Services.AddHttpClient();

// ============================================================

var app = builder.Build();

// Make Azure AD auth an optional feature if the config is present
if (builder.Configuration.GetSection("AzureAd").Exists() && 
    !string.IsNullOrEmpty(builder.Configuration.GetSection("AzureAd").GetValue<string>("ClientId")))
{
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();    // Note. Only Needed for Microsoft.Identity.Web.UI
}

app.UseStaticFiles();
app.MapRazorPages();
app.UseStatusCodePages("text/html", "<!doctype html><h1>&#128163;HTTP error! Status code: {0}</h1>");

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

// API routes for monitoring data and weather 
app.MapGet("/api/monitor", async (IHttpClientFactory clientFactory) =>
{
    return new
    {
        cpuPercentage = Convert.ToInt32(await ApiHelper.GetCpuUsageForProcess()),
        workingSet = Environment.WorkingSet
    };
});

app.MapGet("/api/weather/{posLat:double}/{posLong:double}", async (double posLat, double posLong, IHttpClientFactory clientFactory, IConfiguration config) =>
{
    var apiKey = config.GetValue<string>("Weather:ApiKey");
    if (string.IsNullOrEmpty(apiKey))
    {
        return Results.BadRequest("Weather API key not configured");
    }
    
    (var status, var data) = await ApiHelper.GetOpenWeather(clientFactory, apiKey, posLat, posLong);
    return status == 200 ? Results.Content(data, "application/json") : Results.StatusCode(status);
});

app.Run();
