using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    public interface IOrderRepository : IGenericRepository<Order>
    {
        Task<IEnumerable<Order>> GetAllWithItemsAndMenuItemsAsync();
        Task<Order> GetByIdWithItemsAndMenuItemsAsync(int id);
        // You might add specific methods here for order-related queries, e.g., GetOrdersByStatus
        Task<IEnumerable<Order>> GetOrdersByStatusAsync(OrderStatus status);
    }
}