using MySql.Data.MySqlClient;
using online.shop.api.Models;

namespace online.shop.api.Services
{
    public class ProductService
    {
        private readonly string _connectionString;

        public ProductService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        // Create a new product
        public bool CreateProduct(Product product)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();

                var query =
                    @"
            INSERT INTO Products (Name, Description, Price, ImageUrl) 
            VALUES (@Name, @Description, @Price, @ImageUrl)";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@Name", product.Name);
                    cmd.Parameters.AddWithValue("@Description", product.Description);
                    cmd.Parameters.AddWithValue("@Price", product.Price);
                    cmd.Parameters.AddWithValue("@ImageUrl", product.ImageUrl);

                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }

        // Get all products
        public List<Product> GetAllProducts()
        {
            var products = new List<Product>();

            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();

                var query = "SELECT * FROM Products";
                using (var cmd = new MySqlCommand(query, connection))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var product = new Product
                        {
                            Id = reader.GetInt32("id"),
                            Name = reader.GetString("name"),
                            Description = reader.GetString("description"),
                            Price = reader.GetDecimal("price"),
                            ImageUrl = reader.GetString("ImageUrl")
                        };

                        products.Add(product);
                    }
                }
            }

            return products;
        }

        // Get product by ID
        public Product GetProductById(int id)
        {
            Product product = null;

            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();

                var query = "SELECT * FROM Products WHERE Id = @Id";
                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@Id", id);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            product = new Product
                            {
                                Id = reader.GetInt32("Id"),
                                Name = reader.GetString("Name"),
                                Description = reader.GetString("Description"),
                                Price = reader.GetDecimal("Price"),
                                ImageUrl = reader.GetString("ImageUrl")
                            };
                        }
                    }
                }
            }

            return product;
        }

        // Update a product
        public bool UpdateProduct(Product product)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();

                var query =
                    @"UPDATE Products SET 
                              Name = @Name, 
                              Description = @Description, 
                              Price = @Price, 
                              ImageUrl = @ImageUrl 
                              WHERE Id = @Id";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@Name", product.Name);
                    cmd.Parameters.AddWithValue("@Description", product.Description);
                    cmd.Parameters.AddWithValue("@Price", product.Price);
                    cmd.Parameters.AddWithValue("@ImageUrl", product.ImageUrl);
                    cmd.Parameters.AddWithValue("@Id", product.Id);

                    cmd.ExecuteNonQuery();

                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }

        // Delete a product
        public bool DeleteProduct(int id)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();

                var query = "DELETE FROM Products WHERE Id = @Id";
                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@Id", id);

                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }
    }
}
