using Application.DTOs;
using Application.Services;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        #region CTOR
        private readonly IProductService _productService;
        private readonly IMapper _mapper;

        public ProductsController(IProductService productService, IMapper mapper)
        {
            _productService = productService;
            _mapper = mapper;
        }
        #endregion

        #region GET
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            try
            {
                var product = await _productService.GetProductByIdAsync(id);
                return Ok(product);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred", details = ex.Message });
            }
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetSortedProducts(string? sort)
        {
            try
            {
                var products = await _productService.GetProductsASync(sort);
                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }
        }

        #endregion

        #region ADD 
        [HttpPost("add")]
        public async Task<IActionResult> AddProduct(AddProductDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _productService.AddProductAsync(model);
                if (result)
                    return Ok(new { message = "Product added successfully" });
                else
                    return StatusCode(StatusCodes.Status500InternalServerError, "Failed to add product");
            }
            catch (InvalidOperationException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred: {ex.Message}");
            }
        }

        #endregion

        #region UPDATE
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateProduct(ProductUpdateDto model, int id)
        {
            try
            {
                await _productService.UpdateProductAsync(model, id);
                return Ok("Product Updated Successfully");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred", details = ex.Message });
            }
        }

        #endregion

        #region DELETE 

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                await _productService.DeleteProductAsync(id);
                return Ok(new { message = "Product deleted successfully" });
            }
            catch (NullReferenceException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred: {ex.Message}");
            }
        }
        #endregion


    }
}
