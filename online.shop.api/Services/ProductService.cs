using online.shop.api.Models;
using Microsoft.EntityFrameworkCore;
using online.shop.api.Data;

namespace online.shop.api.Services
{
    public class ProductService
    {
        private readonly ApplicationDbContext _context;

        public ProductService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Create a new product
        public async Task<bool> CreateProductAsync(Product product)
        {
            _context.Products.Add(product);
            int rowsAffected = await _context.SaveChangesAsync();
            return rowsAffected > 0;
        }

        // Get all products
        public async Task<List<Product>> GetAllProductsAsync()
        {
            return await _context.Products.ToListAsync();
        }

        // Get product by ID
        public async Task<Product> GetProductByIdAsync(int id)
        {
            return await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
        }

        // Update a product
        public async Task<bool> UpdateProductAsync(Product product)
        {
            _context.Products.Update(product);
            int rowsAffected = await _context.SaveChangesAsync();
            return rowsAffected > 0;
        }

        // Delete a product
        public async Task<bool> DeleteProductAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
                return false;

            _context.Products.Remove(product);
            int rowsAffected = await _context.SaveChangesAsync();
            return rowsAffected > 0;
        }
    }
}
