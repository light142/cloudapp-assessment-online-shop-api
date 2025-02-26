using System.ComponentModel.DataAnnotations;

namespace online.shop.api.ViewModels
{
    public class EditUserViewModel
    {
        public string Id { get; set; }

        [Required]
        public string Username { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        public string Role { get; set; } // ✅ Add this property

    }
}
