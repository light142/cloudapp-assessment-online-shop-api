using Microsoft.EntityFrameworkCore;
using online.shop.api.Data;
using online.shop.api.Models;
using online.shop.api.Services;

public class WishlistService
{
    private readonly ApplicationDbContext _context;
    private readonly ProductService _productService;

    public WishlistService(ApplicationDbContext context, ProductService productService)
    {
        _context = context;
        _productService = productService;
    }

    public async Task AddToWishlistAsync(string userId, int productId)
    {
        var product = await _context.Products.FindAsync(productId);
        if (product == null)
        {
            throw new Exception("Product not found.");
        }

        var wishlist = await _context.Wishlists
            .Include(w => w.Products)
            .FirstOrDefaultAsync(w => w.UserId == userId);

        if (wishlist == null)
        {
            wishlist = new Wishlist { UserId = userId };
            _context.Wishlists.Add(wishlist);
        }

        if (!wishlist.Products.Any(p => p.Id == productId))
        {
            _context.Products.Attach(product); // ✅ Attach to avoid re-insertion
            wishlist.Products.Add(product);
        }

        await _context.SaveChangesAsync();
    }

    public async Task RemoveFromWishlistAsync(string userId, int productId)
    {
        var wishlist = await _context.Wishlists
            .Include(w => w.Products)
            .FirstOrDefaultAsync(w => w.UserId == userId);

        if (wishlist == null)
            return; // ✅ Avoid errors if wishlist not found

        var product = wishlist.Products.FirstOrDefault(p => p.Id == productId);
        if (product != null)
        {
            wishlist.Products.Remove(product);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<List<Product>> GetWishlistAsync(string userId)
    {
        var wishlist = await _context.Wishlists
            .Include(w => w.Products)
            .FirstOrDefaultAsync(w => w.UserId == userId);

        return wishlist?.Products?.ToList() ?? new List<Product>();
    }
}
