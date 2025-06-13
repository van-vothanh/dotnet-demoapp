using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Graph;

#pragma warning disable CA1848 
#pragma warning disable CA2254 

namespace DotnetDemoapp.Pages
{
    /// <summary>
    /// Page model for displaying user information from Microsoft Graph API
    /// </summary>
    [Authorize]
    public class UserInfoModel(ILogger<UserInfoModel> logger, GraphServiceClient graphServiceClient) : PageModel
    {
        private readonly ILogger<UserInfoModel> _logger = logger;
        private readonly GraphServiceClient _graphServiceClient = graphServiceClient;

        /// <summary>
        /// Gets the username from identity
        /// </summary>
        public string Username { get; private set; } = "";

        /// <summary>
        /// Gets the object identifier from claims
        /// </summary>
        public string Oid { get; private set; } = "";

        /// <summary>
        /// Gets the name from claims
        /// </summary>
        public string Name { get; private set; } = "";

        /// <summary>
        /// Gets the tenant ID from claims
        /// </summary>
        public string TenantId { get; private set; } = "";

        /// <summary>
        /// Gets the preferred username from claims
        /// </summary>
        public string PreferredUsername { get; private set; } = "";

        /// <summary>
        /// Gets the Graph API data dictionary
        /// </summary>
        internal Dictionary<string, string> GraphData = [];

        /// <summary>
        /// Gets the user photo from Graph API
        /// </summary>
        internal byte[] GraphPhoto;

        /// <summary>
        /// Handles GET requests and retrieves user information from Microsoft Graph
        /// </summary>
        /// <returns>The page result or redirect to home</returns>
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

            Username = User.Identity.Name;

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
                GraphData.Add("Office", graphDetails.OfficeLocation);
                GraphData.Add("Mobile", graphDetails.MobilePhone);
                GraphData.Add("Other Phone", graphDetails.BusinessPhones.Any() ? graphDetails.BusinessPhones.First() : "");
                GraphData.Add("Job Title", graphDetails.JobTitle);

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
        /// Converts a stream to byte array
        /// </summary>
        /// <param name="stream">The input stream</param>
        /// <returns>Byte array representation of the stream</returns>
        private static byte[] ToByteArray(Stream stream)
        {
            var length = stream.Length > int.MaxValue ? int.MaxValue : Convert.ToInt32(stream.Length);
            var buffer = new byte[length];
            _ = stream.Read(buffer, 0, length);
            return buffer;
        }
    }
}
