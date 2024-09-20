using Core.Entities;

namespace Core.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string Picture { get; set; }
        public string Category { get; set; }
        public string Brand { get; set; }
        public string SellerId { get; set; }
        public Seller Seller { get; set; }

    }
}
