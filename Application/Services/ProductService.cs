using Application.DTOs;
using AutoMapper;
using Core.Entities;
using Core.Interfaces;
using Core.Models;
using Core.Specifications;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Application.Services
{
    public class ProductService : IProductService
    {
        #region CTOR
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly UserManager<AppUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ProductService(IUnitOfWork unitOfWork, IMapper mapper, UserManager<AppUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        #endregion


        #region GET
        public async Task<ProductToReturnDto> GetProductByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid product ID");

            var product = await _unitOfWork.Repository<Product>().GetByIdAsync(id);

            if (product == null)
                throw new KeyNotFoundException("Product not found");

            return _mapper.Map<ProductToReturnDto>(product);
        }


        public async Task<IReadOnlyList<ProductToReturnDto>> GetProductsASync(string? sort)
        {
            var spec = new SortedProductsSpecification(sort);
            var models = await _unitOfWork.Repository<Product>().ListAsync(spec);
            return _mapper.Map<IReadOnlyList<ProductToReturnDto>>(models);
        }

        #endregion


        #region ADD

        public async Task<bool> AddProductAsync(AddProductDto model)
        {
            var product = _mapper.Map<Product>(model);

            var sellerId = _httpContextAccessor.HttpContext.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;



            if (string.IsNullOrEmpty(sellerId))
            {
                throw new InvalidOperationException("User not found");
            }

            product.Seller = await _unitOfWork.Repository<Seller>().FindAsync(s => s.Id == sellerId);
            product.SellerId = sellerId;

            await _unitOfWork.Repository<Product>().AddAsync(product);
            int result = await _unitOfWork.CompleteAsync();

            return result > 0;
        }
        #endregion


        #region UPDATE
        public async Task UpdateProductAsync(ProductUpdateDto model, int productId)
        {
            var product = await _unitOfWork.Repository<Product>().GetByIdAsync(productId);

            if (product == null)
                throw new KeyNotFoundException("Product not found");

            _mapper.Map(model, product);

            await _unitOfWork.Repository<Product>().UpdateAsync(product);
            await _unitOfWork.CompleteAsync();
        }

        #endregion


        #region DELETE
        public async Task DeleteProductAsync(int id)
        {
            var product = await _unitOfWork.Repository<Product>().GetByIdAsync(id);

            if (product == null)
            {
                throw new NullReferenceException("Product not found");
            }

            await _unitOfWork.Repository<Product>().DeleteAsync(product);
            await _unitOfWork.CompleteAsync();
        }

        #endregion
    }

}

