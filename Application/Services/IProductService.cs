using Application.DTOs;

namespace Application.Services
{
    public interface IProductService
    {
        Task<ProductToReturnDto> GetProductByIdAsync(int id);
        Task<IReadOnlyList<ProductToReturnDto>> GetProductsASync(string sort);
        Task<bool> AddProductAsync(AddProductDto model);
        Task UpdateProductAsync(ProductUpdateDto updateDto, int id);
        Task DeleteProductAsync(int id);


    }
}
