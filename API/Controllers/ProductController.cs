using Application.DTOs;
using Application.Services;
using AutoMapper;
using Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IMapper _mapper;

        public ProductsController(IProductService productService, IMapper mapper)
        {
            _productService = productService;
            _mapper = mapper;
        }

        #region GET
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<Product>>> GetProductById(int id)
        {
            try
            {
                var product = await _productService.GetProductByIdAsync(id);
                if (product == null)
                {
                    return NotFound(new ApiResponse<ProductToReturnDto>
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                        Message = $"Product with ID {id} not found.",
                        Errors = new List<string> { "Product not found." }
                    });
                }

                return Ok(new ApiResponse<ProductToReturnDto>
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Product retrieved successfully.",
                    Data = product
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ApiResponse<Product>
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = ex.Message,
                    Errors = new List<string> { ex.Message }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<Product>
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = "An error occurred.",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<Pagination<ProductToReturnDto>>>> GetSortedProducts(string? sort, int pageIndex = 1, int pageSize = 10)
        {
            try
            {
                var products = await _productService.GetProductsASync(sort, pageIndex, pageSize);
                return Ok(new ApiResponse<Pagination<ProductToReturnDto>>
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Products retrieved successfully.",
                    Data = products
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<Pagination<ProductToReturnDto>>
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = "Internal server error.",
                    Errors = new List<string> { ex.Message }
                });
            }
        }
        #endregion

        #region ADD 
        [HttpPost]
        public async Task<IActionResult> AddProduct([FromForm] AddProductDto model, IFormFile Picture)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<AddProductDto>
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "Invalid model state.",
                    Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList()
                });

            try
            {
                var result = await _productService.AddProductAsync(model, Picture);
                if (result != null)
                {
                    return Ok(new ApiResponse<ProductToReturnDto>
                    {
                        StatusCode = StatusCodes.Status200OK,
                        Message = "Product added successfully.",
                        Data = result

                    });
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<AddProductDto>
                    {
                        StatusCode = StatusCodes.Status500InternalServerError,
                        Message = "Failed to add product.",
                        Errors = new List<string> { "Unexpected error occurred." }
                    });
                }
            }
            catch (InvalidOperationException ex)
            {
                return Unauthorized(new ApiResponse<AddProductDto>
                {
                    StatusCode = StatusCodes.Status401Unauthorized,
                    Message = ex.Message,
                    Errors = new List<string> { ex.Message }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<AddProductDto>
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = "An error occurred.",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        #endregion

        #region UPDATE
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct([FromForm] ProductUpdateDto model, int id, IFormFile Picture)
        {
            try
            {
                var result = await _productService.UpdateProductAsync(model, id, Picture);
                return Ok(new ApiResponse<ProductToReturnDto>
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Product updated successfully.",
                    Data = result

                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResponse<ProductUpdateDto>
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = ex.Message,
                    Errors = new List<string> { "Product not found." }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<ProductUpdateDto>
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = "An error occurred.",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        #endregion

        #region DELETE 

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                var result = await _productService.DeleteProductAsync(id);
                return Ok(new ApiResponse<ProductToReturnDto>
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Product deleted successfully.",
                    Data = result
                });
            }
            catch (NullReferenceException ex)
            {
                return NotFound(new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = ex.Message,
                    Errors = new List<string> { "Product not found." }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = "An error occurred.",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        #endregion
    }
}
