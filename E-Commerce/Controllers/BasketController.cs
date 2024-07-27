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

        [HttpGet("{id}")]
        public async Task<ActionResult<CustomerBasket>> GetBasketById(string id)
        {
            var basket = await _basketService.GetBasketAsync(id);
            if (basket == null)
                return NotFound();

            return Ok(basket);
        }

        [HttpPost("update")]
        public async Task<ActionResult<CustomerBasket>> UpdateBasket(CustomerBasket basket)
        {
            var updatedBasket = await _basketService.UpdateBasketAsync(basket);
            if (updatedBasket == null)
                return BadRequest("Problem updating basket");

            return Ok(updatedBasket);
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
        public async Task<ActionResult<CustomerBasket>> AddItemToBasket(string basketId, BasketItemDto item)
        {
            var basket = await _basketService.AddItemToBasketAsync(basketId, item);
            return Ok(basket);
        }
    }
}
