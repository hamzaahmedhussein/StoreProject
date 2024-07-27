using Application.DTOs;
using Core.Models;

namespace Application.Services
{
    public interface IProductService
    {
        Task<ProductToReturnDto> GetProductByIdAsync(int id);
        Task<IReadOnlyList<ProductToReturnDto>> GetProductsASync(string sort, int? brandId, int? typeId);
        Task AddProductAsync(Product product);
        Task UpdateProductAsync(Product product);
        Task DeleteProductAsync(int id);
        Task<IReadOnlyList<ProductBrand>> GetProductBrands();
        Task<IReadOnlyList<ProductType>> GetProductTypes();

    }
}
