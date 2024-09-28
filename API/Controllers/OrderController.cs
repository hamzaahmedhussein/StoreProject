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

        [HttpPost()]
        public async Task<ActionResult<ApiResponse<Order>>> CreateOrder(OrderDto orderDto)
        {
            var address = _mapper.Map<Address>(orderDto.ShippingToAddress);

            var order = await _orderService.CreateOrderAsync(orderDto.BuyerEmail, orderDto.DeliveryMethodId, orderDto.BasketId, address);

            if (order == null)
            {
                return BadRequest(new ApiResponse<Order>
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "Problem creating order.",
                    Errors = new List<string> { "Failed to create order." }
                });
            }

            return Ok(new ApiResponse<Order>
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Order created successfully.",
                Data = order
            });
        }

        [HttpPost("delivery-methods")]
        public async Task<ActionResult<ApiResponse<DeliveryMethod>>> AddDeliveryMethod([FromBody] DeliveryMethodDto dto)
        {
            var newMethod = await _orderService.AddDeliveryMethodAsync(dto.ShortName, dto.DeliveryTime, dto.Description, dto.Price);

            if (newMethod == null)
            {
                return BadRequest(new ApiResponse<DeliveryMethod>
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "Failed to add delivery method.",
                    Errors = new List<string> { "Delivery method could not be added." }
                });
            }

            return Ok(new ApiResponse<DeliveryMethod>
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Delivery method added successfully.",
                Data = newMethod
            });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<Order>>> GetOrderById(int id, [FromQuery] string buyerEmail)
        {
            var order = await _orderService.GetOrderByIdAsync(id, buyerEmail);

            if (order == null)
            {
                return NotFound(new ApiResponse<Order>
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = $"Order with ID {id} and email {buyerEmail} not found.",
                    Errors = new List<string> { "Order not found." }
                });
            }

            return Ok(new ApiResponse<Order>
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Order retrieved successfully.",
                Data = order
            });
        }

        [HttpGet("buyer")]
        public async Task<ActionResult<ApiResponse<IReadOnlyList<Order>>>> GetOrdersForUser([FromQuery] string buyerEmail)
        {
            var orders = await _orderService.GetOrdersForUserAsync(buyerEmail);

            if (orders == null || orders.Count == 0)
            {
                return NotFound(new ApiResponse<IReadOnlyList<Order>>
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = $"No orders found for user {buyerEmail}.",
                    Errors = new List<string> { "No orders found." }
                });
            }

            return Ok(new ApiResponse<IReadOnlyList<Order>>
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Orders retrieved successfully.",
                Data = orders
            });
        }

        [HttpGet("delivery-methods")]
        public async Task<ActionResult<ApiResponse<IReadOnlyList<DeliveryMethod>>>> GetDeliveryMethods()
        {
            var methods = await _orderService.GetDeliveryMethodsAsync();
            return Ok(new ApiResponse<IReadOnlyList<DeliveryMethod>>
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Delivery methods retrieved successfully.",
                Data = methods
            });
        }
    }
}
