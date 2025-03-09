using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using online.shop.api.Services;

namespace online.shop.api.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ProductService _productService;
        private readonly WishlistService _wishlistService;

        public ProductsController(ProductService productService, WishlistService wishlistService)
        {
            _productService = productService;
            _wishlistService = wishlistService;
        }

        public async Task<IActionResult> IndexAsync()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var wishlist = await _wishlistService.GetWishlistAsync(userId);
                var wishlistProductIds = wishlist.Select(p => p.Id).ToList(); // List of product IDs in the wishlist

                var products = await _productService.GetAllProductsAsync();

                // Pass the wishlist product IDs to the view
                var viewModel = new ProductViewModel
                {
                    Products = products,
                    WishlistProductIds = wishlistProductIds
                };

                return View(viewModel);
            }
            catch (Exception)
            {
                // If there's an error connecting to the DB, return the error view
                return View("Error");
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var product = await _productService.GetProductByIdAsync(id);
                if (product == null)
                {
                    return View("Error");
                }
                return View(product);
            }
            catch (Exception)
            {
                // If there's an error connecting to the DB, return the error view
                return View("Error");
            }
        }
    }
}
