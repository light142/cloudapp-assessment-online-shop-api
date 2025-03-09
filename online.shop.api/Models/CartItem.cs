namespace online.shop.api.Models
{
    public class CartItem
    {
        public int ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
    }

    public class CartItemRequest
    {
        public int ProductId { get; set; }
        public int? Quantity { get; set; }
    }
}
