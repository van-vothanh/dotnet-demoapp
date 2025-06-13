# .NET 8 - Demo Web Application

This is a modern .NET 8 web application using the minimal hosting model and Razor pages. It was originally created from the `dotnet new webapp` template and has been enhanced with custom APIs, Bootstrap v5, Microsoft Identity, and other modern packages/features.

The app has been designed with cloud native demos & containers in mind, providing a real working application for deployment scenarios - something more substantial than "hello-world" but with minimal prerequisites. It is not intended as a complete example of a fully functioning architecture or complex software design.

## ✨ What's New in v2.0 (.NET 8 Upgrade)

- **Upgraded to .NET 8.0** - Latest LTS version with improved performance and features
- **Updated Dependencies** - All NuGet packages updated to latest secure versions
- **Modern C# Features** - Utilizes primary constructors, collection expressions, and improved string comparisons
- **Enhanced Code Quality** - Strict code analysis rules with comprehensive documentation
- **Security Improvements** - Resolved all known vulnerabilities in dependencies

## 🚀 Typical Use Cases

- Deployment to Kubernetes
- Docker containerization demos
- CI/CD pipeline demonstrations (build pipelines provided)
- Cloud deployment (Azure) showcases
- Application monitoring and auto-scaling demos

## 📱 Application Features

The app includes several pages accessible from the top navigation menu. Some features are only available when specific configuration variables are set (see 'Optional Features' below):

- **Info** - Displays system & runtime information, detects Docker container and Kubernetes environments
- **Tools** - Utilities for demos including CPU load generation (for autoscale demos) and error/exception pages for monitoring tools
- **Monitoring** - Real-time CPU load and memory working set charts via REST API using Chart.js
- **Weather** - (Optional) Uses HTML5 Geolocation to fetch weather forecasts from [OpenWeather API](https://openweathermap.org/)
- **User Account** - (Optional) Azure AD integration with user login and Microsoft Graph API integration

![Application Screenshots](https://user-images.githubusercontent.com/14982936/71717446-0bc47400-2e10-11ea-8db2-1db5b991d566.png)

## 📊 Status

![](https://img.shields.io/github/last-commit/benc-uk/dotnet-demoapp) ![](https://img.shields.io/github/release-date/benc-uk/dotnet-demoapp) ![](https://img.shields.io/github/v/release/benc-uk/dotnet-demoapp) ![](https://img.shields.io/github/commit-activity/y/benc-uk/dotnet-demoapp)

Live instances:
[![](https://img.shields.io/website?label=Hosted%3A%20Kubernetes&up_message=online&url=https%3A%2F%2Fdotnet-demoapp.kube.benco.io%2F)](https://dotnet-demoapp.kube.benco.io/)

## 🛠️ Running and Testing Locally

### Prerequisites

- Linux, WSL, or macOS with bash, make, etc.
- [.NET 8 SDK](https://docs.microsoft.com/en-us/dotnet/core/install/linux) - for local development, linting, and testing
- [Docker](https://docs.docker.com/get-docker/) - for containerization and image operations
- [Azure CLI](https://docs.microsoft.com/en-us/cli/azure/install-azure-cli-linux) - for Azure deployment

### Getting Started

Clone the project to your development directory:

```bash
git clone https://github.com/benc-uk/dotnet-demoapp.git
cd dotnet-demoapp
```

### 📋 Makefile Commands

A comprehensive GNU Makefile is provided for common operations:

```txt
$ make

help                 💬 This help message
lint                 🔎 Lint & format, will not fix but sets exit code on error
image                🔨 Build container image from Dockerfile
push                 📤 Push container image to registry
run                  🏃‍ Run locally using Dotnet CLI
deploy               🚀 Deploy to Azure Container App
undeploy             💀 Remove from Azure
test                 🎯 Unit tests with xUnit
test-report          🤡 Unit tests with xUnit & output report
clean                🧹 Clean up project
```

### Configuration Variables

| Makefile Variable | Default                |
| ----------------- | ---------------------- |
| IMAGE_REG         | ghcr<span>.</span>io   |
| IMAGE_REPO        | benc-uk/dotnet-demoapp |
| IMAGE_TAG         | latest                 |
| AZURE_RES_GROUP   | demoapps               |
| AZURE_REGION      | northeurope            |
| AZURE_APP_NAME    | dotnet-demoapp         |

The web application listens on Kestrel's default port 5000. This can be customized using the `ASPNETCORE_URLS` environment variable or the `--urls` parameter ([documentation](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/servers/kestrel?view=aspnetcore-8.0)).

## 🐳 Container Support

Public container images are [available on GitHub Container Registry](https://github.com/users/benc-uk/packages/container/package/dotnet-demoapp).

Run the application in a container:

```bash
docker run --rm -it -p 5000:5000 ghcr.io/benc-uk/dotnet-demoapp:latest
```

To build your own container image:
```bash
make image IMAGE_REPO=your-repo/dotnet-demoapp IMAGE_TAG=your-tag
```

### Kubernetes Deployment

The application can be easily deployed to Kubernetes using Helm. See [deploy/kubernetes/readme.md](deploy/kubernetes/readme.md) for detailed instructions.

## 🔄 CI/CD with GitHub Actions

Comprehensive GitHub Actions workflows are included:

- **CI Builds** - Automated builds for PRs with code validation (linting and tests) and dev image creation
- **CD Deployment** - Automated deployment to AKS using Helm when code is merged to master

[![](https://img.shields.io/github/workflow/status/benc-uk/dotnet-demoapp/CI%20Build%20App)](https://github.com/benc-uk/dotnet-demoapp/actions?query=workflow%3A%22CI+Build+App%22) [![](https://img.shields.io/github/workflow/status/benc-uk/dotnet-demoapp/CD%20Release%20-%20AKS?label=release-kubernetes)](https://github.com/benc-uk/dotnet-demoapp/actions?query=workflow%3A%22CD+Release+-+AKS%22)

## ⚙️ Optional Features

The application starts with zero configuration, but only basic features (*Info*, *Tools* & *Monitoring*) will be available. Enable additional features with these configurations:

### 📊 Application Insights

**Configuration:** Set `ApplicationInsights__InstrumentationKey`

The app includes Application Insights SDK instrumentation for:
- Request tracking
- Dependency call monitoring  
- Exception and error logging

For local development using `appsettings.Development.json`:

```json
{
  "ApplicationInsights": {
    "InstrumentationKey": "<your-key-here>"
  }
}
```

[Learn more about .NET monitoring with Application Insights](https://docs.microsoft.com/en-us/azure/azure-monitor/app/asp-net-core)

### 🌤️ Weather Integration

**Configuration:** Set `Weather__ApiKey`

Requires an API key from [OpenWeather](https://openweathermap.org/) (free signup available). The feature uses HTML5 Geolocation API to fetch the user's location for weather data.

**Note:** Geolocation only works over HTTPS or localhost. London, UK weather is shown as fallback.

Local configuration example:

```json
{
  "Weather": {
    "ApiKey": "<your-openweather-api-key>"
  }
}
```

### 🔐 Azure AD Authentication & Microsoft Graph

**Configuration:** Set multiple `AzureAd` environment variables

Enables user authentication with Microsoft Identity Platform and Microsoft Graph API integration for user profile data and photos.

Uses [Microsoft.Identity.Web](https://github.com/AzureAD/microsoft-identity-web) library for seamless Azure AD integration.

#### Azure AD App Registration Setup

1. Register an app in your Azure AD tenant ([registration guide](https://docs.microsoft.com/en-us/azure/active-directory/develop/quickstart-register-app))
2. Configure these settings:
   - Enable *"Accounts in any organizational directory and personal Microsoft accounts"*
   - Add *Web platform* for authentication
   - Set Redirect URI ending with `/signin-oidc`
   - Enable *"Access Tokens"* and *"ID Tokens"*
   - Create a client secret

#### Environment Variables

- `AzureAd__ClientId`: Your app's client ID
- `AzureAd__ClientSecret`: Your app's client secret  
- `AzureAd__Instance`: Set to `https://login.microsoftonline.com/`
- `AzureAd__TenantId`: Set to `common`

Local configuration example:

```json
{
  "AzureAd": {
    "Instance": "https://login.microsoftonline.com/",
    "ClientId": "<your-client-id>",
    "ClientSecret": "<your-client-secret>",
    "TenantId": "common"
  }
}
```

## ☁️ Azure Container App Deployment

Deploy to Azure Container App using the provided Bicep template in the [deploy](deploy/) directory.

Quick deployment:
```bash
make deploy
```

This creates:
- Resource group
- Azure Container App instance with supporting resources
- Deployment of the latest container image

**Note:** Azure Container App currently doesn't support HTTP header forwarding, so Azure AD sign-in may not work correctly due to redirect URL issues.

## 📝 Version History

- **Dec 2024** - Major upgrade to .NET 8 with modern C# features and security updates
- **Nov 2021** - Large scale rewrite to .NET 6
- **Mar 2021** - Updated deployment, added unit tests and makefile
- **Nov 2020** - Updated to .NET 5
- **Nov 2020** - New GitHub pipelines & container registry
- **Jun 2020** - Moved to NuGet for Microsoft.Identity.Web
- **Jan 2020** - Complete rewrite from scratch

## 🏗️ Architecture & Technologies

- **.NET 8** - Latest LTS framework
- **ASP.NET Core** - Web framework with minimal APIs
- **Razor Pages** - Server-side rendering
- **Bootstrap 5** - Modern responsive UI
- **Chart.js** - Real-time monitoring charts
- **Microsoft Identity Platform** - Enterprise authentication
- **Microsoft Graph API** - User profile integration
- **Application Insights** - Telemetry and monitoring
- **OpenWeather API** - Weather data integration
- **Docker** - Containerization
- **Kubernetes/Helm** - Orchestration
- **GitHub Actions** - CI/CD pipelines
