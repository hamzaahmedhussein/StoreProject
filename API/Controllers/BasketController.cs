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

        [HttpGet("{id}")]
        public async Task<ActionResult<CustomerBasket>> GetBasketById(string id)
        {
            var basket = await _basketService.GetBasketAsync(id);
            if (basket == null)
                return NotFound();

            return Ok(basket);
        }

        [HttpPost("delete-item")]
        public async Task<IActionResult> RemoveItemFromBasket(string basketId, int productId)
        {
            try
            {
                var basket = await _basketService.RemoveItemFromBasketAsync(basketId, productId);

                return Ok(basket);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }


        [HttpDelete("delete/{id}")]
        public async Task<ActionResult> DeleteBasket(string id)
        {
            var deleted = await _basketService.DeleteBasketAsync(id);
            if (!deleted)
                return BadRequest("Problem deleting basket");

            return NoContent();
        }

        [HttpPost("add-item")]
        public async Task<IActionResult> AddItemToBasket(string basketId, int productId)
        {

            try
            {
                var basket = await _basketService.AddItemToBasketAsync(basketId, productId);
                return Ok(basket);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
