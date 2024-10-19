using Microsoft.AspNetCore.Mvc;
using OrderManagementSystem.Application.Order.Dto;
using OrderManagementSystem.Application.Order;
using OrderManagementSystem.Domain.Common;

namespace OrderManagementSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderAppService _orderAppService;
        private readonly ILogger<OrderController> _logger;

        public OrderController(IOrderAppService orderAppService, ILogger<OrderController> logger)
        {
            _orderAppService = orderAppService;
            _logger = logger;
        }

        [HttpPost("PlaceOrder")]
        public async Task<ActionResult<ApiResponse<string>>> PlaceOrderAsync([FromBody] PlaceOrderInput input)
        {
            try
            {
                var result = await _orderAppService.PlaceOrderAsync(input);
                return Ok(new ApiResponse<string>(200, "Order placed successfully", result));
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogError(ex, "Input parameter is null.");
                return BadRequest(new ApiResponse<string>(400, "Input parameter cannot be null."));
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Invalid operation occurred.");
                return BadRequest(new ApiResponse<string>(400, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred.");
                return StatusCode(500, new ApiResponse<string>(500, "Internal server error"));
            }
        }

        [HttpGet("GetOrderDetails")]
        public async Task<ActionResult<ApiResponse<OrderDetailsDto>>> GetOrderDetailsAsync(int orderId)
        {
            if (orderId <= 0)
                return BadRequest(new ApiResponse<OrderDetailsDto>(400, "Invalid order ID."));

            try
            {
                var orderDetails = await _orderAppService.GetOrderDetailsAsync(orderId);
                if (orderDetails == null)
                    return NotFound(new ApiResponse<OrderDetailsDto>(404, $"Order with ID {orderId} not found."));

                return Ok(new ApiResponse<OrderDetailsDto>(200, "Order details retrieved successfully", orderDetails));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving order details for order ID {OrderId}.", orderId);
                return StatusCode(500, new ApiResponse<OrderDetailsDto>(500, "Internal server error"));
            }
        }

        [HttpGet("ListOrders")]
        public async Task<ActionResult<ApiResponse<List<OrderDto>>>> ListOrdersAsync()
        {
            try
            {
                var orders = await _orderAppService.ListOrdersAsync();
                return Ok(new ApiResponse<List<OrderDto>>(200, "Orders retrieved successfully", orders));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the list of orders.");
                return StatusCode(500, new ApiResponse<List<OrderDto>>(500, "Internal server error"));
            }
        }

        [HttpPost("CancelOrder/{orderId}")]
        public async Task<ActionResult<ApiResponse<string>>> CancelOrderAsync(int orderId)
        {
            try
            {
                var result = await _orderAppService.CancelOrderAsync(orderId);
                return Ok(new ApiResponse<string>(200, "Order canceled successfully", result));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while canceling order with ID: {OrderId}", orderId);
                return StatusCode(500, new ApiResponse<string>(500, "Internal server error"));
            }
        }
    }
}
