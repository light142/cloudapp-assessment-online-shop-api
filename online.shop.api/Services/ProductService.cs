using MySql.Data.MySqlClient;
using online.shop.api.Models;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace online.shop.api.Services
{
    public class ProductService
    {
        private readonly string _connectionString;

        public ProductService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public List<Product> GetAllProducts()
        {
            var products = new List<Product>();

            using (var conn = new MySqlConnection(_connectionString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT id, name, image_url, price FROM products", conn);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        products.Add(new Product
                        {
                            Id = reader.GetInt32("id"),
                            Name = reader.GetString("name"),
                            ImageUrl = reader.GetString("image_url"),
                            Price = reader.GetDecimal("price")
                        });
                    }
                }
            }

            return products;
        }

        public Product GetProductById(int id)
        {
            Product product = null;

            using (var conn = new MySqlConnection(_connectionString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT id, name, image_url, price FROM products WHERE id = @id", conn);
                cmd.Parameters.AddWithValue("@id", id);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        product = new Product
                        {
                            Id = reader.GetInt32("id"),
                            Name = reader.GetString("name"),
                            ImageUrl = reader.GetString("image_url"),
                            Price = reader.GetDecimal("price")
                        };
                    }
                }
            }

            return product;
        }
    }
}
