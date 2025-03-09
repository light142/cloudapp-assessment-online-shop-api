using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

public class WishlistController : Controller
{
    private readonly WishlistService _wishlistService;

    public WishlistController(WishlistService wishlistService)
    {
        _wishlistService = wishlistService;
    }

    // Action to add a product to the wishlist
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> AddToWishlist(int productId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get the logged-in user

        try
        {
            await _wishlistService.AddToWishlistAsync(userId, productId);
            return Json(new { success = true, message = "Product added to wishlist!" });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> RemoveFromWishlist(int productId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get the logged-in user
        await _wishlistService.RemoveFromWishlistAsync(userId, productId);
        return Json(new { success = true });
    }
}
