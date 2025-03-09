using Microsoft.AspNetCore.Mvc;
using online.shop.api.Services;

namespace online.shop.api.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ProductService _productService;

        public ProductsController(ProductService productService)
        {
            _productService = productService;
        }

        public IActionResult Index()
        {
            try
            {
                var products = _productService.GetAllProducts();
                return View(products);
            }
            catch (Exception)
            {
                // If there's an error connecting to the DB, return the error view
                return View("Error");
            }
        }

        public IActionResult Details(int id)
        {
            try
            {
                var product = _productService.GetProductById(id);
                if (product == null)
                {
                    return NotFound();
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
