using Microsoft.AspNetCore.Identity;
namespace CognitoMetric.Models
{
    // Use IdentityUser to leverage ASP.NET Identity
    public class ApplicationUser : IdentityUser
    {
        public string DisplayName { get; set; }
        // Add extra fields as needed
    }
}