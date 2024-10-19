using OrderManagementSystem.Application.Order.Dto;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagementSystem.Application.Order
{
    public interface IOrderAppService
    {
        Task<string> PlaceOrderAsync(PlaceOrderInput input);
        Task<OrderDetailsDto> GetOrderDetailsAsync(int orderId);
        Task<List<OrderDto>> ListOrdersAsync();
        Task<string> CancelOrderAsync(int orderId);
    }

}
