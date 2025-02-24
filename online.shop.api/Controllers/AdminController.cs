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
        public IActionResult CreateProduct()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateProduct(Product model)
        {
            if (ModelState.IsValid)
            {
                _productService.CreateProduct(model);
                return RedirectToAction("ManageProducts");
            }
            return View(model);
        }

        // Edit Product
        public IActionResult EditProduct(int id)
        {
            var product = _productService.GetProductById(id);
            return View(product);
        }

        [HttpPost]
        public IActionResult EditProduct(Product model)
        {
            if (ModelState.IsValid)
            {
                _productService.UpdateProduct(model);
                return RedirectToAction("ManageProducts");
            }
            return View(model);
        }

        // Delete Product
        public IActionResult DeleteProduct(int id)
        {
            _productService.DeleteProduct(id);
            return RedirectToAction("ManageProducts");
        }
    }
}
