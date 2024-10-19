using Microsoft.EntityFrameworkCore;

using OrderManagementSystem.Application.Customer;
using OrderManagementSystem.Application.Customer.Dto;
using OrderManagementSystem.Application.Order.Dto;
using OrderManagementSystem.Domain.Entities;
using OrderManagementSystem.Infrastructure.Data.Repositories;


namespace OrderManagementSystem.Application.Order
{
    public class OrderAppService : IOrderAppService
    {
        private readonly IRepository<Domain.Entities.Order> _orderRepository;
        private readonly IRepository<Domain.Entities.Product> _productRepository;
        private readonly ICustomerAppService _customerAppService;
        public OrderAppService(IRepository<Domain.Entities.Order> orderRepository, IRepository<Domain.Entities.Product> productRepository, ICustomerAppService customerAppService)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _customerAppService = customerAppService;
        }

        public async Task<string> PlaceOrderAsync(PlaceOrderInput input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input), "Order input cannot be null.");
            }
            int customerId = await _customerAppService.CheckForNewOrder(new CreateCustomerInput
            {
                FullName = input.CustomerName,
                DateOfBirth = input.DateOfBirth,
                Email = input.Email,
                PhoneNumber = input.PhoneNumber,
            });
            var order = new Domain.Entities.Order
            {
                OrderDate = DateTime.UtcNow,
                CustomerId = customerId,
                TotalAmount = 0,
                OrderItems = new List<OrderItem>()
            };

            foreach (var item in input.OrderItems)
            {
                var product = await _productRepository.GetByIdAsync(item.ProductId);
                if (product == null)
                    throw new InvalidOperationException($"Product with ID {item.ProductId} does not exist.");

                if (product.StockQuantity < item.Quantity)
                    throw new InvalidOperationException($"Insufficient stock for product ID {item.ProductId}. Available: {product.StockQuantity}, Requested: {item.Quantity}.");

                product.StockQuantity -= item.Quantity;
                await _productRepository.UpdateAsync(product);

                order.TotalAmount += product.Price * item.Quantity;
                order.OrderItems.Add(new OrderItem
                {
                    Quantity = item.Quantity,
                    UnitPrice = product.Price,
                    ProductId = product.ProductId
                });
            }

            await _orderRepository.InsertAsync(order);
            return "Order Created Successfully";
        }

        public async Task<OrderDetailsDto> GetOrderDetailsAsync(int orderId)
        {
            if (orderId <= 0)
                throw new ArgumentException("Order ID must be greater than zero.", nameof(orderId));

            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null) return null;

            return new OrderDetailsDto
            {
                OrderId = order.OrderId,
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount,
                Customer = new CustomerDto
                {
                    Id = order.Customer.CustomerId,
                    Name = order.Customer.FullName
                },
                OrderItems = order.OrderItems.Select(oi => new OrderItemDto
                {
                    OrderItemId = oi.OrderItemId,
                    ProductId = oi.ProductId,
                    ProductName = oi.Product.Name,
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice
                }).ToList()
            };
        }

        public async Task<List<OrderDto>> ListOrdersAsync()
        {
            var orders = await _orderRepository.GetAll()
                .Include(x => x.Customer)
                .Include(x => x.OrderItems)
                .ThenInclude(x => x.Product)
                .ToListAsync();

            return orders.Select(order => new OrderDto
            {
                OrderId = order.OrderId,
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount,
                Customer = new CustomerDto
                {
                    Id = order.Customer.CustomerId,
                    Name = order.Customer.FullName,
                    Email = order.Customer.Email,
                    PhoneNumber = order.Customer.PhoneNumber,
                },
                OrderItems = order.OrderItems.Select(oi => new OrderItemDto
                {
                    OrderItemId = oi.OrderItemId,
                    ProductId = oi.ProductId,
                    ProductName = oi.Product.Name,
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice
                }).ToList()
            }).ToList();
        }
        public async Task<string> CancelOrderAsync(int orderId)
        {
            var order = await _orderRepository.GetAll()
                .Where(x => x.OrderId == orderId)
                .Include(x => x.OrderItems)
                .FirstOrDefaultAsync();
            if (order == null)
                throw new Exception($"Order with ID {orderId} not found.");

            foreach (var item in order.OrderItems)
            {
                var product = await _productRepository.GetByIdAsync(item.ProductId);
                if (product != null)
                {
                    // Restore stock
                    product.StockQuantity += item.Quantity;
                    await _productRepository.UpdateAsync(product);
                }
            }

            await _orderRepository.DeleteAsync(orderId);
            return "Order Canceled Successfully";
        }

    }

}
