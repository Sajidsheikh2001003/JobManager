using Microsoft.AspNetCore.Identity;

namespace JobManager.Data
{
    public class ApplicationUser : IdentityUser
    {
        public string? DisplayName { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Address { get; set; }
    }
}
