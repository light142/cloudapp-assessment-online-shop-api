using Microsoft.AspNetCore.Identity;

namespace online.shop.api.Models
{
    // You can extend the IdentityUser class if you need more custom properties for the user.
    public class ApplicationUser : IdentityUser
    {
        // Add any custom properties here, for example:
        // public IList<string> Roles { get; set; }
    }

    public class UserWithRoles
    {
        public ApplicationUser User { get; set; }
        public IList<string> Roles { get; set; }
    }
}
