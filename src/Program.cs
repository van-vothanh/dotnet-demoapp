using DotnetDemoapp;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
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

// Add HttpClient factory for better management of HttpClient instances
builder.Services.AddHttpClient();

// Configure CORS policy
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("https://*.benco.io")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Configure request timeouts (new in .NET 8)
builder.Services.AddRequestTimeouts(options =>
{
    options.DefaultTimeout = TimeSpan.FromSeconds(10);
});

// ============================================================

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios.
    app.UseHsts();
}

// Make Azure AD auth an optional feature if the config is present
if (builder.Configuration.GetSection("AzureAd").Exists() && 
    !string.IsNullOrEmpty(builder.Configuration.GetSection("AzureAd").GetValue<string>("ClientId")))
{
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();    // Note. Only Needed for Microsoft.Identity.Web.UI
}

app.UseStaticFiles();
app.UseCors();
app.UseRequestTimeouts();
app.UseStatusCodePages("text/html", "<!doctype html><h1>&#128163;HTTP error! Status code: {0}</h1>");
app.MapRazorPages();

// API routes for monitoring data and weather 
app.MapGet("/api/monitor", async (IHttpClientFactory httpClientFactory) =>
{
    return new
    {
        cpuPercentage = Convert.ToInt32(await ApiHelper.GetCpuUsageForProcess()),
        workingSet = Environment.WorkingSet
    };
})
.WithName("GetMonitoringData")
.WithOpenApi();

app.MapGet("/api/weather/{posLat:double}/{posLong:double}", async (double posLat, double posLong, IConfiguration config, IHttpClientFactory httpClientFactory) =>
{
    var apiKey = config.GetValue<string>("Weather:ApiKey");
    if (string.IsNullOrEmpty(apiKey))
    {
        return Results.BadRequest("Weather API key not configured");
    }
    
    (var status, var data) = await ApiHelper.GetOpenWeather(apiKey, posLat, posLong, httpClientFactory);
    return status == 200 ? Results.Content(data, "application/json") : Results.StatusCode(status);
})
.WithName("GetWeatherData")
.WithOpenApi();

// Easy to miss this, starting the whole app and server!
app.Run();
