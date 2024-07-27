using Application.DTOs;
using AutoMapper;
using Core.Interfaces;
using Core.Models;
using Core.Specifications;

namespace Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ProductService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ProductToReturnDto> GetProductByIdAsync(int id)
        {
            var spec = new ProductWithTypesAndBrandsSpecification(id);

            var model = await _unitOfWork.Repository<Product>().GetEntityWithSpec(spec);
            return _mapper.Map<ProductToReturnDto>(model);
        }

        public async Task<IReadOnlyList<ProductToReturnDto>> GetProductsASync(string sort, int? brandId, int? typeId)
        {
            var spec = new ProductWithTypesAndBrandsSpecification(sort, brandId, typeId);
            var models = await _unitOfWork.Repository<Product>().ListAsync(spec);
            return _mapper.Map<IReadOnlyList<ProductToReturnDto>>(models);
        }

        public async Task AddProductAsync(Product product)
        {
            _unitOfWork.Repository<Product>().Add(product);
            await _unitOfWork.CompleteAsync();
        }

        public async Task UpdateProductAsync(Product product)
        {
            _unitOfWork.Repository<Product>().Update(product);
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteProductAsync(int id)
        {
            var product = await _unitOfWork.Repository<Product>().GetByIdAsync(id);
            _unitOfWork.Repository<Product>().Delete(product);
            await _unitOfWork.CompleteAsync();
        }
        public async Task<IReadOnlyList<ProductBrand>> GetProductBrands()
        {
            var models = await _unitOfWork.Repository<ProductBrand>().ListAllAsync();
            return models;
        }
        public async Task<IReadOnlyList<ProductType>> GetProductTypes()
        {
            var models = await _unitOfWork.Repository<ProductType>().ListAllAsync();
            return models;
        }


    }
}
