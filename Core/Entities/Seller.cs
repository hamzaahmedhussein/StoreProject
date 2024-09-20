using Core.Models;

namespace Core.Entities
{
    public class Seller : AppUser
    {

        public ICollection<Product> Products { get; set; }

    }
}
