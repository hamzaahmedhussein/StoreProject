using Core.Entities;
using System.ComponentModel.DataAnnotations;

namespace Application.DTOs
{
    public class CustomerBasketDto
    {
        [Required]
        public string Id { get; set; }
        public List<BasketItem> Items { get; set; }
    }
}
