using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Graph;

#pragma warning disable CA1848 
#pragma warning disable CA2254 

namespace DotnetDemoapp.Pages;

[Authorize]
public class UserInfoModel : PageModel
{
    private readonly ILogger<UserInfoModel> _logger;
    private readonly GraphServiceClient _graphServiceClient;

    public string Username { get; private set; } = string.Empty;
    public string Oid { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string TenantId { get; private set; } = string.Empty;
    public string PreferredUsername { get; private set; } = string.Empty;
    internal Dictionary<string, string> GraphData { get; } = new();
    internal byte[]? GraphPhoto { get; private set; }

    public UserInfoModel(ILogger<UserInfoModel> logger, GraphServiceClient graphServiceClient)
    {
        _logger = logger;
        _graphServiceClient = graphServiceClient;
    }

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
                _logger.LogWarning("Failed to get user photo: {Error}", e.Message);
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

    private static async Task<byte[]> ToByteArrayAsync(Stream stream)
    {
        using var memoryStream = new MemoryStream();
        await stream.CopyToAsync(memoryStream);
        return memoryStream.ToArray();
    }
}
