using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Graph;

namespace DotnetDemoapp.Pages;

/// <summary>
/// Page model for user information display
/// </summary>
[Authorize]
public class UserInfoModel : PageModel
{
    private readonly ILogger<UserInfoModel> _logger;
    private readonly GraphServiceClient _graphServiceClient;

    /// <summary>
    /// Username from identity
    /// </summary>
    public string Username { get; private set; } = string.Empty;
    
    /// <summary>
    /// Object ID from claims
    /// </summary>
    public string Oid { get; private set; } = string.Empty;
    
    /// <summary>
    /// Name from claims
    /// </summary>
    public string Name { get; private set; } = string.Empty;
    
    /// <summary>
    /// Tenant ID from claims
    /// </summary>
    public string TenantId { get; private set; } = string.Empty;
    
    /// <summary>
    /// Preferred username from claims
    /// </summary>
    public string PreferredUsername { get; private set; } = string.Empty;
    
    /// <summary>
    /// Graph API data
    /// </summary>
    internal Dictionary<string, string> GraphData { get; } = new();
    
    /// <summary>
    /// User photo from Graph API
    /// </summary>
    internal byte[]? GraphPhoto { get; private set; }

    /// <summary>
    /// Constructor for UserInfoModel
    /// </summary>
    public UserInfoModel(ILogger<UserInfoModel> logger, GraphServiceClient graphServiceClient)
    {
        _logger = logger;
        _graphServiceClient = graphServiceClient;
    }

    /// <summary>
    /// Handler for GET requests
    /// </summary>
    public async Task<IActionResult> OnGet()
    {
        foreach (var claim in User.Claims)
        {
            if (claim.Type.Contains("objectidentifier", StringComparison.OrdinalIgnoreCase) || 
                claim.Type.Contains("oid", StringComparison.OrdinalIgnoreCase))
            {
                Oid = claim.Value;
            }
            if (claim.Type.Contains("tenant", StringComparison.OrdinalIgnoreCase) || 
                claim.Type.Contains("tid", StringComparison.OrdinalIgnoreCase))
            {
                TenantId = claim.Value;
            }
            if (claim.Type == "name")
            {
                Name = claim.Value;
            }
            if (claim.Type == "preferred_username")
            {
                PreferredUsername = claim.Value;
            }
        }

        Username = User.Identity?.Name ?? string.Empty;

        // Graph stuff
        try
        {
            // Fetch user details from Graph API
            var graphDetails = await _graphServiceClient.Me
                .Request()
                .GetAsync();

            GraphData.Add("UPN", graphDetails.UserPrincipalName ?? string.Empty);
            GraphData.Add("Given Name", graphDetails.GivenName ?? string.Empty);
            GraphData.Add("Display Name", graphDetails.DisplayName ?? string.Empty);
            GraphData.Add("Office", graphDetails.OfficeLocation ?? string.Empty);
            GraphData.Add("Mobile", graphDetails.MobilePhone ?? string.Empty);
            GraphData.Add("Other Phone", graphDetails.BusinessPhones.Any() ? graphDetails.BusinessPhones.First() : string.Empty);
            GraphData.Add("Job Title", graphDetails.JobTitle ?? string.Empty);

            // Fetch user photo, this used to fail with MSA accounts hence the extra try/catch
            try
            {
                var pictureStream = await _graphServiceClient
                    .Me
                    .Photos["432x432"]
                    .Content
                    .Request()
                    .GetAsync();

                // Convert to bytes
                GraphPhoto = await ToByteArrayAsync(pictureStream);
            }
            catch (Exception e)
            {
                _logger.LogWarning("Failed to get user photo: {Message}", e.Message);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching user data from Graph API");
            
            // Cookie seems to get out of sync with the token cache when hotreloading the page
            foreach (var cookie in Request.Cookies.Keys)
            {
                Response.Cookies.Delete(cookie);
            }
            return Redirect("/");
        }
        return Page();
    }

    private static async Task<byte[]> ToByteArrayAsync(Stream stream)
    {
        using var memoryStream = new MemoryStream();
        await stream.CopyToAsync(memoryStream);
        return memoryStream.ToArray();
    }
}
