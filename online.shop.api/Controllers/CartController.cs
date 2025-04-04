using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using online.shop.api.Models;
using online.shop.api.Services;

namespace online.shop.api.Controllers
{
    public class CartController : Controller
    {
        private const string CartSessionKey = "Cart";
        private readonly ProductService _productService;
        private readonly KeyVaultService _keyVaultService;

        public CartController(ProductService productService, KeyVaultService keyVaultService)
        {
            _productService = productService;
            _keyVaultService = keyVaultService;
        }

        public IActionResult Index()
        {
            var cart = GetCart();
            return View(cart);
        }

        [HttpGet]
        public IActionResult GetCartCount()
        {
            var cart = GetCart();
            int totalItems = cart.Sum(c => c.Quantity); // Count total items, not just products
            return Json(new { count = totalItems });
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart([FromBody] EncryptedRequest encryptedRequest)
        {
            string decryptedJson = _keyVaultService.Decrypt(encryptedRequest.EncryptedData);

            var request = JsonConvert.DeserializeObject<CartItemRequest>(decryptedJson);

            var cart = GetCart();
            var product = await _productService.GetProductByIdAsync(request.ProductId);

            var quantity = request.Quantity ?? 1;

            if (product == null)
            {
                return Json(new { success = false, message = "Product not found" });
            }
            var existingItem = cart.FirstOrDefault(c => c.ProductId == request.ProductId);

            if (existingItem != null)
            {
                existingItem.Quantity += quantity; // Increment quantity if product already exists
            }
            else
            {
                cart.Add(
                    new CartItem
                    {
                        ProductId = product.Id,
                        Name = product.Name,
                        ImageUrl = product.ImageUrl,
                        Price = product.Price,
                        Quantity = quantity
                    }
                );
            }
            SaveCart(cart);
            return Json(new { success = true });
        }

        [HttpPost]
        public IActionResult UpdateCart(int productId, int quantity)
        {
            var cart = GetCart();
            var cartItem = cart.FirstOrDefault(c => c.ProductId == productId);

            if (cartItem != null)
            {
                cartItem.Quantity = quantity;
            }

            SaveCart(cart);

            // Calculate new totals
            decimal itemSubtotal = cartItem != null ? cartItem.Quantity * cartItem.Price : 0;
            decimal cartTotal = cart.Sum(c => c.Quantity * c.Price);
            int cartCount = cart.Sum(c => c.Quantity);

            return Json(
                new
                {
                    success = true,
                    itemSubtotal,
                    cartTotal,
                    cartCount
                }
            );
        }

        [HttpPost]
        public IActionResult RemoveFromCart(int productId)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(c => c.ProductId == productId);

            if (item != null)
            {
                cart.Remove(item);
            }

            SaveCart(cart);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult ClearCart()
        {
            HttpContext.Session.Remove(CartSessionKey);
            return Json(new { success = true });
        }

        private List<CartItem> GetCart()
        {
            var cartJson = HttpContext.Session.GetString(CartSessionKey);
            return cartJson != null
                ? JsonConvert.DeserializeObject<List<CartItem>>(cartJson) ?? new List<CartItem>()
                : new List<CartItem>();
        }

        private void SaveCart(List<CartItem> cart)
        {
            HttpContext.Session.SetString(CartSessionKey, JsonConvert.SerializeObject(cart));
        }
    }
}
