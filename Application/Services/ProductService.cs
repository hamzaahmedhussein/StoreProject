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
        private IUserHelpers _userHelpers;

        public ProductService(IUnitOfWork unitOfWork, IMapper mapper, UserManager<AppUser> userManager, IHttpContextAccessor httpContextAccessor, IUserHelpers userHelpers)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _userHelpers = userHelpers;
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


        public async Task<Pagination<ProductToReturnDto>> GetProductsASync(string? sort, int pageIndex, int pageSize)
        {
            var spec = new SortedProductsSpecification(sort, pageIndex, pageSize);

            var products = await _unitOfWork.Repository<Product>().ListAsync(spec);

            var totalCount = await _unitOfWork.Repository<Product>().CountAsync(new SortedProductsSpecification(sort, 0, int.MaxValue));

            var data = _mapper.Map<IReadOnlyList<ProductToReturnDto>>(products);

            return new Pagination<ProductToReturnDto>(pageIndex, pageSize, totalCount, data);
        }


        #endregion


        #region ADD

        public async Task<ProductToReturnDto> AddProductAsync(AddProductDto model, IFormFile Picture)
        {
            var product = _mapper.Map<Product>(model);
            product.Picture = await _userHelpers.AddImage(Picture, "Products");


            var sellerId = _httpContextAccessor.HttpContext.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;



            if (string.IsNullOrEmpty(sellerId))
            {
                throw new InvalidOperationException("User not found");
            }

            product.Seller = await _unitOfWork.Repository<Seller>().FindAsync(s => s.Id == sellerId);
            product.SellerId = sellerId;

            await _unitOfWork.Repository<Product>().AddAsync(product);
            int result = await _unitOfWork.CompleteAsync();

            return _mapper.Map<ProductToReturnDto>(product);
        }
        #endregion


        #region UPDATE
        public async Task<ProductToReturnDto> UpdateProductAsync(ProductUpdateDto model, int productId, IFormFile picture)
        {
            var product = await _unitOfWork.Repository<Product>().GetByIdAsync(productId);

            if (product == null)
            {
                throw new KeyNotFoundException("Product not found");
            }

            if (picture != null && picture.Length > 0)
            {
                product.Picture = await _userHelpers.AddImage(picture, "Products");
            }
            else
            {
                product.Picture = product.Picture;
            }

            if (!string.IsNullOrEmpty(model.Name))
            {
                product.Name = model.Name;
            }

            if (!string.IsNullOrEmpty(model.Description))
            {
                product.Description = model.Description;
            }

            if (model.Price > 0)
            {
                product.Price = model.Price;
            }

            if (model.Quantity >= 0)
            {
                product.Quantity = model.Quantity;
            }

            if (!string.IsNullOrEmpty(model.Category))
            {
                product.Category = model.Category;
            }

            if (!string.IsNullOrEmpty(model.Brand))
            {
                product.Brand = model.Brand;
            }

            await _unitOfWork.Repository<Product>().UpdateAsync(product);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<ProductToReturnDto>(product);
        }


        #endregion


        #region DELETE
        public async Task<ProductToReturnDto> DeleteProductAsync(int id)
        {
            var product = await _unitOfWork.Repository<Product>().GetByIdAsync(id);

            if (product == null)
            {
                throw new NullReferenceException("Product not found");
            }

            await _unitOfWork.Repository<Product>().DeleteAsync(product);
            await _unitOfWork.CompleteAsync();
            return _mapper.Map<ProductToReturnDto>(product);

        }

        #endregion
    }

}

