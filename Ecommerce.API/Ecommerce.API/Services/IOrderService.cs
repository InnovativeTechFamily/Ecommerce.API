using Ecommerce.API.DTOs.Orders;
using Ecommerce.API.Entities.Orders;

namespace Ecommerce.API.Services
{
    public interface IOrderService
    {
        Task<IReadOnlyList<Order>> CreateOrdersAsync(CreateOrderRequestDto dto);
        Task<IReadOnlyList<Order>> GetOrdersByUserIdAsync(string userId);
        Task<IReadOnlyList<Order>> GetOrdersByShopIdAsync(string shopId); // NEW

        Task<Order> UpdateOrderStatusAsync(string orderId, string status, string sellerId); // NEW
        Task<Order> RequestOrderRefundAsync(string orderId, string status); // NEW
        Task<Order> AcceptOrderRefundAsync(string orderId, string status, string sellerId); // NEW
        Task<IReadOnlyList<Order>> GetAllOrdersAdminAsync(); // NEW

    }

}
