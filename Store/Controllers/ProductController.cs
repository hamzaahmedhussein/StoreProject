using API.DTOs;
using AutoMapper;
using Core.Interfaces;
using Core.Models;
using Core.Specifications;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
   
    public class ProductController : BaseApiController
    {
        private readonly IGenericRepository<Product> _productRepo;
        private readonly IGenericRepository<ProductBrand> _productBrandRepo;
        private readonly IGenericRepository<ProductType> _productTypeRepo;
        private readonly IMapper _mapper;

        public ProductController(IGenericRepository<Product> productRepo,
            IGenericRepository<ProductType> productTypeRepo,
            IGenericRepository<ProductBrand> productBrandRepo,
             IMapper mapper)
       
        {
            _productRepo = productRepo;
            _productBrandRepo = productBrandRepo;
            _productTypeRepo = productTypeRepo;
            _mapper = mapper;

        }

        [HttpGet("product/{id}")]
        public async Task<IActionResult> GetProduct(int id)
        {
            var spec = new ProductWithTypesAndBrandsSpecification(id);

            var model = await _productRepo.GetByIdWithSpecAsync(spec);

            if (model == null)
                return NotFound();

            return Ok(_mapper.Map<Product, ProductToReturnDto>(model));
        }

        [HttpGet("products")]
        public async Task<ActionResult<IReadOnlyList<ProductToReturnDto>>> GetProducts(string sort,int? brandId,int? typeId)
        {
            var spec = new ProductWithTypesAndBrandsSpecification(sort,brandId,typeId);
            var models = await _productRepo.GetAllWithSpecAsync(spec);
            return Ok(_mapper.Map<IReadOnlyList<Product>,IReadOnlyList<ProductToReturnDto>>(models));
        }
        [HttpGet("brands")]

        public async Task<ActionResult<List<ProductBrand>>> GetProductBrands()
        {
            var models = await _productBrandRepo.GetAllAsync();
            return Ok(models);
        }
        [HttpGet("types")]

        public async Task<ActionResult<List<ProductType>>> GetProductTypes()
        {
            var models = await _productTypeRepo.GetAllAsync();
            return Ok(models);
        } 
    }
}
