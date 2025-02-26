using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using online.shop.api.Models;
using online.shop.api.Services;

namespace online.shop.api.Controllers
{
    [Authorize(Roles = "Admin")] // Only allow access to admin users
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ProductService _productService; // Service for handling products

        public AdminController(
            UserManager<ApplicationUser> userManager,
            ProductService productService
        )
        {
            _userManager = userManager;
            _productService = productService;
        }

        // Admin Dashboard
        public IActionResult Index()
        {
            return View();
        }

        // Manage Users
        public async Task<IActionResult> ManageUsers()
        {
            var users = await _userManager.Users.ToListAsync();

            var userWithRoles = new List<UserWithRoles>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);

                userWithRoles.Add(new UserWithRoles { User = user, Roles = roles.ToList() });
            }
            return View(userWithRoles);
        }

        // Promote or Demote User to Admin
        [HttpPost]
        public async Task<IActionResult> UpdateUserRole(string userId, bool isAdmin)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                if (isAdmin)
                {
                    await _userManager.AddToRoleAsync(user, "Admin");
                }
                else
                {
                    await _userManager.RemoveFromRoleAsync(user, "Admin");
                }
            }
            return RedirectToAction("ManageUsers");
        }

        // Manage Products
        public IActionResult ManageProducts()
        {
            var products = _productService.GetAllProducts(); // Assuming a product service exists
            return View(products);
        }

        // Create Product
        [HttpGet]
        public IActionResult CreateProduct()
        {
            return PartialView("Shared/_CreateProduct", new Product());
        }

        [HttpPost]
        public IActionResult CreateProduct(Product product)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("Shared/_CreateProduct", product);
            }

            bool isCreated = _productService.CreateProduct(product);
            if (isCreated)
            {
                return Json(new { success = true });
            }

            return Json(new { success = false, message = "Failed to create product." });
        }

        // Edit Product
        [HttpGet]
        public IActionResult EditProduct(int id)
        {
            var product = _productService.GetProductById(id);
            if (product == null)
                return NotFound();
            return PartialView("Shared/_EditProduct", product);
        }

        [HttpPost]
        public IActionResult EditProduct(Product model)
        {
            if (ModelState.IsValid)
            {
                var updated = _productService.UpdateProduct(model);
                if (updated)
                    return Json(new { success = true });
            }
            return PartialView("Shared/_EditProduct", model);
        }

        [HttpGet]
        public IActionResult DeleteProduct(int id)
        {
            var product = _productService.GetProductById(id);
            if (product == null)
                return NotFound();
            return PartialView("Shared/_DeleteProduct", product);
        }

        [HttpPost]
        public IActionResult ConfirmDelete(int id)
        {
            var deleted = _productService.DeleteProduct(id);
            if (deleted)
                return Json(new { success = true });

            return NotFound();
        }
    }
}
