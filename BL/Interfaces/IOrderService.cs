using DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IOrderService
    {
        Task<Order> CreateOrderAsync(Order order, List<OrderItem> orderItems);
        Task<IEnumerable<Order>> GetAllOrdersAsync();
        Task<Order> GetOrderByIdAsync(int id);
        Task UpdateOrderAsync(Order order);       
        Task<bool> CancelOrderAsync(int orderId);
        Task<bool> UpdateOrderStatusAsync(int orderId, OrderStatus newStatus);  
        Task ApplyDiscountsAsync(Order order); 
        Task RecalculateOrderTotalsAsync(int orderId);
        Task AutoProgressOrderStatusAsync(int orderId);     }
}