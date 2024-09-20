using Application.DTOs;
using AutoMapper;
using Core.Entities;
using Core.Entities.OrderAggregate;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IMapper _mapper;

        public OrdersController(IOrderService orderService, IMapper mapper)
        {
            _orderService = orderService;
            _mapper = mapper;
        }

        [HttpPost("create-order")]
        public async Task<ActionResult<Order>> CreateOrder(OrderDto orderDto)
        {
            var address = _mapper.Map<Address>(orderDto.ShippingToAddress);

            var order = await _orderService.CreateOrderAsync(orderDto.BuyerEmail, orderDto.DeliveryMethodId, orderDto.BasketId, address);

            if (order == null) return BadRequest("Problem creating order");

            return Ok(order);
        }


        [HttpPost("add-delivery-method")]
        public async Task<IActionResult> AddDeliveryMethod([FromBody] DeliveryMethodDto dto)
        {
            var newMethod = await _orderService.AddDeliveryMethodAsync(dto.ShortName, dto.DeliveryTime, dto.Description, dto.Price);

            if (newMethod == null)
                return BadRequest("Failed to add delivery method");

            return Ok(newMethod);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(int id, [FromQuery] string buyerEmail)
        {
            var order = await _orderService.GetOrderByIdAsync(id, buyerEmail);

            if (order == null)
                return NotFound($"Order with ID {id} and email {buyerEmail} not found");

            return Ok(order);
        }


        [HttpGet("user-orders")]
        public async Task<IActionResult> GetOrdersForUser([FromQuery] string buyerEmail)
        {
            var orders = await _orderService.GetOrdersForUserAsync(buyerEmail);

            if (orders == null || orders.Count == 0)
                return NotFound($"No orders found for user {buyerEmail}");

            return Ok(orders);
        }


        [HttpGet("deliveryMethods")]
        public async Task<ActionResult<IReadOnlyList<DeliveryMethod>>> GetDeliveryMethods()
        {
            var methods = await _orderService.GetDeliveryMethodsAsync();
            return Ok(methods);
        }
    }
}
