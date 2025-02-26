using System.ComponentModel.DataAnnotations.Schema;

namespace online.shop.api.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        [Column(TypeName = "DECIMAL(10,2)")]
        public decimal Price { get; set; }
        public string ImageUrl { get; set; }
    }
}
