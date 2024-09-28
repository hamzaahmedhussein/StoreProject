using Application.DTOs;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BasketController : ControllerBase
    {
        private readonly IBasketService _basketService;

        public BasketController(IBasketService basketService)
        {
            _basketService = basketService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<CustomerBasket>>> GetBasket()
        {
            var basket = await _basketService.GetBasketAsync(HttpContext);
            if (basket == null)
            {
                return NotFound(new ApiResponse<CustomerBasket>
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = "Basket not found.",
                    Errors = new List<string> { "No basket associated with this user." }
                });
            }

            return Ok(new ApiResponse<CustomerBasket>
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Basket retrieved successfully.",
                Data = basket
            });
        }

        [HttpDelete("items/{productId}")]
        public async Task<ActionResult<ApiResponse<CustomerBasket>>> RemoveItemFromBasket(int productId)
        {
            try
            {
                var basket = await _basketService.RemoveItemFromBasketAsync(HttpContext, productId);
                return Ok(new ApiResponse<CustomerBasket>
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Item removed from basket successfully.",
                    Data = basket
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<CustomerBasket>
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "Error removing item from basket.",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpDelete]
        public async Task<ActionResult<ApiResponse<string>>> DeleteBasket()
        {
            var deleted = await _basketService.DeleteBasketAsync(HttpContext);
            if (!deleted)
            {
                return BadRequest(new ApiResponse<string>
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "Problem deleting basket.",
                    Errors = new List<string> { "Basket could not be deleted." }
                });
            }

            return NoContent();
        }


        [HttpPost("items")]
        public async Task<ActionResult<ApiResponse<CustomerBasket>>> AddItemToBasket([FromQuery] int productId)

        {
            try
            {
                var basket = await _basketService.AddItemToBasketAsync(HttpContext, productId);
                return Ok(new ApiResponse<CustomerBasket>
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Item added to basket successfully.",
                    Data = basket
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<CustomerBasket>
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "Error adding item to basket.",
                    Errors = new List<string> { ex.Message }
                });
            }
        }
    }
}
