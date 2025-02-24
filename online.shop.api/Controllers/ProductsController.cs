using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using online.shop.api.Models;

namespace online.shop.api.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IConfiguration _configuration;

        public ProductsController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            List<Product> products = new List<Product>();

            using (
                MySqlConnection conn = new MySqlConnection(
                    _configuration.GetConnectionString("DefaultConnection")
                )
            )
            {
                try
                {
                    conn.Open();
                }
                catch (Exception)
                {
                    // If there's an error connecting to the DB, return the error view
                    return View("Error");
                }

                MySqlCommand cmd = new MySqlCommand("SELECT * FROM products", conn);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        products.Add(
                            new Product
                            {
                                Id = reader.GetInt32("id"),
                                Name = reader.GetString("name"),
                                Description = reader.GetString("description"),
                                Price = reader.GetDecimal("price"),
                                ImageUrl = reader.IsDBNull(reader.GetOrdinal("image_url"))
                                    ? ""
                                    : reader.GetString("image_url")
                            }
                        );
                    }
                }
            }

            return View(products);
        }
    }
}
