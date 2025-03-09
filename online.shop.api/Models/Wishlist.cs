using online.shop.api.Models;

public class Wishlist
{
    public int Id { get; set; }
    public string UserId { get; set; } // Reference to the user
    public ApplicationUser User { get; set; } // Related user
    public ICollection<Product> Products { get; set; } = new List<Product>(); // Related products
}
