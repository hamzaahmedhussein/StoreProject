using Core.Models;

namespace Core.Specifications
{
    public class SortedProductsSpecification : Specification<Product>
    {
        public SortedProductsSpecification(string? sort, int pageIndex, int pageSize)
        {
            // Default ordering if no sort parameter is provided
            AddOrderBy(p => p.Name);

            // Handle sorting logic
            if (!string.IsNullOrEmpty(sort))
            {
                switch (sort.ToLower())
                {
                    case "priceasc":
                        AddOrderBy(p => p.Price);
                        break;
                    case "pricedesc":
                        AddOrderByDescending(p => p.Price);
                        break;
                    default:
                        break;
                }
            }

            ApplyPaging((pageIndex - 1) * pageSize, pageSize);
        }
    }
}
