using Application.DTOs;
using Microsoft.AspNetCore.Http;

namespace Application.Services
{
    public interface IProductService
    {
        Task<ProductToReturnDto> GetProductByIdAsync(int id);
        Task<Pagination<ProductToReturnDto>> GetProductsASync(string? sort, int pageIndex, int pageSize);
        Task<ProductToReturnDto> AddProductAsync(AddProductDto model, IFormFile Picture);
        Task<ProductToReturnDto> UpdateProductAsync(ProductUpdateDto updateDto, int id, IFormFile Picture);
        Task<ProductToReturnDto> DeleteProductAsync(int id);


    }
}
