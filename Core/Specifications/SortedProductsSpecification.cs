using Core.Models;

namespace Core.Specifications
{
    public class SortedProductsSpecification : Specification<Product>
    {
        public SortedProductsSpecification(string sort)
        {
            // Default ordering if no sort parameter is provided
            AddOrderBy(n => n.Name);

            if (!string.IsNullOrEmpty(sort))
            {
                switch (sort)
                {
                    case "priceAsc":
                        AddOrderBy(p => p.Price);
                        break;
                    case "priceDesc":
                        AddOrderByDescending(p => p.Price);
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
