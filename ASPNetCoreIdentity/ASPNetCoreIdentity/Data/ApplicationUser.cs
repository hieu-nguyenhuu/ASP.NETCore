using Microsoft.AspNetCore.Identity;

namespace ASPNetCoreIdentity.Data
{
    public class ApplicationUser : IdentityUser
    {
        public string firstName { get; set; }
        public string lastName { get; set; }
    }
}
