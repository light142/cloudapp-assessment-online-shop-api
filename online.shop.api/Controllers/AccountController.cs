using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using online.shop.api.Models;

namespace online.shop.api.Controllers
{
    [Authorize]
    public class AccountController(UserManager<ApplicationUser> userManager) : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            return View(
                new UserProfileModel
                {
                    Username = user.UserName ?? string.Empty,
                    Email = user.Email ?? string.Empty,
                    Role = User.IsInRole("Admin") ? "Admin" : "User"
                }
            );
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
            return RedirectToAction("Index", "Products");
        }
    }
}
