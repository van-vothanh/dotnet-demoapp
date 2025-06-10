using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Graph;

#pragma warning disable CA1848 
#pragma warning disable CA2254 

namespace DotnetDemoapp.Pages;

/// <summary>
/// Page model for displaying user information from Azure AD and Microsoft Graph
/// </summary>
[Authorize]
public class UserInfoModel(ILogger<UserInfoModel> logger, GraphServiceClient graphServiceClient) : PageModel
{
    private readonly ILogger<UserInfoModel> _logger = logger;
    private readonly GraphServiceClient _graphServiceClient = graphServiceClient;

    /// <summary>
    /// Username from the identity
    /// </summary>
    public string Username { get; private set; } = string.Empty;
    
    /// <summary>
    /// Object ID from Azure AD
    /// </summary>
    public string Oid { get; private set; } = string.Empty;
    
    /// <summary>
    /// User's display name
    /// </summary>
    public string Name { get; private set; } = string.Empty;
    
    /// <summary>
    /// Azure AD tenant ID
    /// </summary>
    public string TenantId { get; private set; } = string.Empty;
    
    /// <summary>
    /// User's preferred username
    /// </summary>
    public string PreferredUsername { get; private set; } = string.Empty;
    
    /// <summary>
    /// Data retrieved from Microsoft Graph API
    /// </summary>
    internal Dictionary<string, string> GraphData { get; } = [];
    
    /// <summary>
    /// User's profile photo from Microsoft Graph
    /// </summary>
    internal byte[]? GraphPhoto { get; private set; }

    /// <summary>
    /// Handles GET requests for the page
    /// </summary>
    public async Task<IActionResult> OnGet()
    {
        foreach (var claim in User.Claims)
        {
            if (claim.Type.Contains("objectidentifier") || claim.Type.Contains("oid"))
            {
                Oid = claim.Value;
            }
            if (claim.Type.Contains("tenant") || claim.Type.Contains("tid"))
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
        // Acquire the access token
        try
        {
            // Fetch user details from Graph API
            var graphDetails = await _graphServiceClient.Me
                .Request()
                .GetAsync();

            GraphData.Add("UPN", graphDetails.UserPrincipalName);
            GraphData.Add("Given Name", graphDetails.GivenName);
            GraphData.Add("Display Name", graphDetails.DisplayName);
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
                GraphPhoto = ToByteArray(pictureStream);
            }
            catch (Exception e)
            {
                _logger.LogWarning(e.ToString());
            }
        }
        catch (Exception)
        {
            // Cookie seems to get out of sync with the token cache when hotreloading the page
            // This is a horrible hack
            foreach (var cookie in Request.Cookies.Keys)
            {
                Response.Cookies.Delete(cookie);
            }
            return Redirect("/");
        }
        return Page();
    }

    /// <summary>
    /// Converts a stream to a byte array
    /// </summary>
    private static byte[] ToByteArray(Stream stream)
    {
        var length = stream.Length > int.MaxValue ? int.MaxValue : Convert.ToInt32(stream.Length);
        var buffer = new byte[length];
        _ = stream.Read(buffer, 0, length);
        return buffer;
    }
}
