using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using online.shop.api.Models;

namespace online.shop.api.Controllers
{
    [Authorize]
    public class AccountController(
        UserManager<ApplicationUser> userManager,
        WishlistService wishlistService
    ) : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly WishlistService _wishlistService = wishlistService;

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

        [HttpGet]
        public async Task<IActionResult> Wishlist()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var products = await _wishlistService.GetWishlistAsync(userId);

            return View(products);
        }
    }
}
