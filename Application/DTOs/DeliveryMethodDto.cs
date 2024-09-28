using System.ComponentModel.DataAnnotations;

namespace Application.DTOs
{
    public class DeliveryMethodDto
    {
        [Required(ErrorMessage = "Short name is required.")]
        [StringLength(100, ErrorMessage = "Short name cannot exceed 100 characters.")]
        public string ShortName { get; set; }

        [Required(ErrorMessage = "Delivery time is required.")]
        public string DeliveryTime { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Price is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero.")]
        public decimal Price { get; set; }
    }
}
