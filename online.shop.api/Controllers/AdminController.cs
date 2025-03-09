using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using online.shop.api.Models;
using online.shop.api.Services;
using online.shop.api.ViewModels;

namespace online.shop.api.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ProductService _productService;
        private readonly UserService _userService;

        public AdminController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ProductService productService,
            UserService userService
        )
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _productService = productService;
            _userService = userService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
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

        [HttpGet]
        public async Task<IActionResult> EditUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return Json(new { success = false, message = "Failed action" });
            ;

            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault() ?? "User"; // Default to "User"

            var model = new EditUserViewModel
            {
                Id = user.Id,
                Username = user.UserName,
                Email = user.Email,
                Role = role
            };

            return PartialView("Shared/_EditUser", model);
        }

        [HttpPost]
        public async Task<IActionResult> EditUser(EditUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                return Json(new { success = false, errors });
            }

            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null)
                return Json(
                    new { success = false, errors = new List<string> { "User not found." } }
                );

            user.UserName = model.Username;
            user.Email = model.Email;

            // Validate role before updating
            if (!string.IsNullOrEmpty(model.Role) && await _roleManager.RoleExistsAsync(model.Role))
            {
                var currentRoles = await _userManager.GetRolesAsync(user);
                var removeRolesResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
                if (!removeRolesResult.Succeeded)
                {
                    return Json(
                        new
                        {
                            success = false,
                            errors = removeRolesResult.Errors.Select(e => e.Description).ToList()
                        }
                    );
                }

                var addRoleResult = await _userManager.AddToRoleAsync(user, model.Role);
                if (!addRoleResult.Succeeded)
                {
                    return Json(
                        new
                        {
                            success = false,
                            errors = addRoleResult.Errors.Select(e => e.Description).ToList()
                        }
                    );
                }
            }
            else
            {
                return Json(
                    new { success = false, errors = new List<string> { "Invalid role selected." } }
                );
            }

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                return Json(
                    new
                    {
                        success = false,
                        errors = updateResult.Errors.Select(e => e.Description).ToList()
                    }
                );
            }

            return Json(new { success = true, message = "User updated successfully." });
        }

        // GET: Delete User Modal
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
                return Json(new { success = false, message = "Failed action" });
            ;

            return PartialView("Shared/_DeleteUser", user);
        }

        // POST: Confirm Delete User
        [HttpPost]
        public async Task<IActionResult> ConfirmDeleteUser(string id)
        {
            var result = await _userService.DeleteUserAsync(id);
            if (result.Succeeded)
            {
                return Json(new { success = true, message = "User deleted successfully." });
            }

            return Json(new { success = false, errors = result.Errors.Select(e => e.Description) });
        }

        [HttpGet]
        // GET: Load Create User Modal
        public async Task<IActionResult> CreateUser()
        {
            var roles = _roleManager.Roles.Select(r => r.Name).ToList();
            var model = new CreateUserViewModel { Roles = roles };
            return PartialView("Shared/_CreateUser", model);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(CreateUserViewModel model)
        {
            if (!ModelState.IsValid)
                return Json(
                    new
                    {
                        success = false,
                        errors = ModelState.Values
                            .SelectMany(v => v.Errors)
                            .Select(e => e.ErrorMessage)
                    }
                );

            var user = new ApplicationUser { UserName = model.UserName, Email = model.Email };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                if (
                    !string.IsNullOrEmpty(model.SelectedRole)
                    && await _roleManager.RoleExistsAsync(model.SelectedRole)
                )
                {
                    await _userManager.AddToRoleAsync(user, model.SelectedRole);
                }
                return Json(new { success = true, message = "User created successfully." });
            }

            return Json(new { success = false, errors = result.Errors.Select(e => e.Description) });
        }

        // Manage Products
        public async Task<IActionResult> ManageProducts()
        {
            var products = await _productService.GetAllProductsAsync(); // Assuming a product service exists
            return View(products);
        }

        // Create Product
        [HttpGet]
        public IActionResult CreateProduct()
        {
            return PartialView("Shared/_CreateProduct", new Product());
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct(Product product)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Failed to create product." });
            }

            bool isCreated = await _productService.CreateProductAsync(product);
            if (isCreated)
            {
                return Json(new { success = true, message = "Product created successfully." });
            }

            return Json(new { success = false, message = "Failed to create product." });
        }

        // Edit Product
        [HttpGet]
        public async Task<IActionResult> EditProduct(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
                return NotFound();
            return PartialView("Shared/_EditProduct", product);
        }

        [HttpPost]
        public async Task<IActionResult> EditProduct(Product model)
        {
            if (ModelState.IsValid)
            {
                var updated = await _productService.UpdateProductAsync(model);
                if (updated)
                    return Json(new { success = true, message = "Product updated successfully." });
            }
            return Json(new { success = false, message = "Failed action" });
        }

        [HttpGet]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
                return NotFound();
            return PartialView("Shared/_DeleteProduct", product);
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmDelete(int id)
        {
            var deleted = await _productService.DeleteProductAsync(id);
            if (deleted)
                return Json(new { success = true, message = "Product deleted successfully." });

            return Json(new { success = false, message = "Failed action" });
            ;
        }
    }
}
