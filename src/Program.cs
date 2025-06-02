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

// Build the app
var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

// Make Azure AD auth an optional feature if the config is present
if (builder.Configuration.GetSection("AzureAd").Exists() && 
    !string.IsNullOrEmpty(builder.Configuration.GetSection("AzureAd").GetValue<string>("ClientId")))
{
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers(); // Required for Microsoft.Identity.Web.UI
}

app.UseStaticFiles();
app.UseStatusCodePages("text/html", "<!doctype html><h1>&#128163;HTTP error! Status code: {0}</h1>");
app.MapRazorPages();

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
    var (status, data) = await ApiHelper.GetOpenWeather(apiKey, posLat, posLong);
    return status == 200 ? Results.Content(data!, "application/json") : Results.StatusCode(status);
});

app.Run();
