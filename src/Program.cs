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

// Configure HTTP request pipeline
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
}

// API routes for monitoring data and weather 
app.MapGet("/api/monitor", async () =>
{
    return new
    {
        cpuPercentage = Convert.ToInt32(await ApiHelper.GetCpuUsageForProcess()),
        workingSet = Environment.WorkingSet
    };
});

app.MapGet("/api/weather/{posLat:double}/{posLong:double}", async (double posLat, double posLong) =>
{
    var apiKey = builder.Configuration.GetValue<string>("Weather:ApiKey");
    if (string.IsNullOrEmpty(apiKey))
    {
        return Results.BadRequest("Weather API key not configured");
    }
    
    (var status, var data) = await ApiHelper.GetOpenWeather(apiKey, posLat, posLong);
    return status == 200 ? Results.Content(data, "application/json") : Results.StatusCode(status);
});

// Start the app
app.Run();
