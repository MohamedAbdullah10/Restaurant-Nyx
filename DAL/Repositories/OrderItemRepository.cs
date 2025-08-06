using DAL.Models;
using DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public class OrderItemRepository : GenericRepository<OrderItem>, IOrderItemRepository
    {
        public OrderItemRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<OrderItem>> GetOrderItemsByOrderIdAsync(int orderId)
        {
            return await _context.OrderItems
                                 .Where(oi => oi.OrderId == orderId)
                                 .Include(oi => oi.MenuItem) 
                                 .ToListAsync();
        }

        public async Task<OrderItem> GetOrderItemDetailsAsync(int orderItemId)
        {
            return await _context.OrderItems
                                 .Include(oi => oi.MenuItem)
                                 .Include(oi => oi.Order)
                                 .FirstOrDefaultAsync(oi => oi.Id == orderItemId);
        }
    }
}