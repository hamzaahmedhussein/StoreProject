using System.ComponentModel.DataAnnotations;

namespace Application.DTOs
{
    public class ProductUpdateDto
    {

        public string Name { get; set; }

        [StringLength(500, ErrorMessage = "Description can't be longer than 500 characters.")]
        public string Description { get; set; }

        [Range(0.0, double.MaxValue, ErrorMessage = "Price must be greater than zero.")]
        public decimal Price { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Quantity can't be negative.")]
        public int Quantity { get; set; }

        [StringLength(100, ErrorMessage = "Category can't be longer than 100 characters.")]

        public string Category { get; set; }

        [StringLength(100, ErrorMessage = "Brand can't be longer than 100 characters.")]

        public string Brand { get; set; }
    }
}
