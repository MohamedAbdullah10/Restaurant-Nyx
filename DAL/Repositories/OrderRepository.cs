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
    public class OrderRepository : GenericRepository<Order>, IOrderRepository
    {
        public OrderRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Order>> GetAllWithItemsAndMenuItemsAsync()
        {
            return await _context.Orders
                                 .Include(o => o.OrderItems)
                                    .ThenInclude(oi => oi.MenuItem)
                                 .ToListAsync();
        }

        public async Task<Order> GetByIdWithItemsAndMenuItemsAsync(int id)
        {
            return await _context.Orders
                                 .Include(o => o.OrderItems)
                                    .ThenInclude(oi => oi.MenuItem)
                                 .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<IEnumerable<Order>> GetOrdersByStatusAsync(OrderStatus status)
        {
            return await _context.Orders
                                 .Where(o => o.Status == status)
                                 .ToListAsync();
        }
    }
}