using DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        public IMenuItemRepository MenuItemRepository { get; private set; }
        public ICategoryRepository CategoryRepository { get; private set; }
        public IOrderRepository OrderRepository { get; private set; }      
        public IOrderItemRepository OrderItemRepository { get; private set; } 
        public IDailyMenuItemActivityRepository DailyMenuItemActivityRepository { get; private set; }
        public UnitOfWork(AppDbContext context)
        {
            _context = context;
            MenuItemRepository = new MenuItemRepository(_context);
            CategoryRepository = new CategoryRepository(_context);
            OrderRepository = new OrderRepository(_context);       
            OrderItemRepository = new OrderItemRepository(_context);
            DailyMenuItemActivityRepository = new DailyMenuItemActivityRepository(_context);
        }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}