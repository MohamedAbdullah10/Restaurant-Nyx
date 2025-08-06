
using DAL;

using DAL.Models;
using DAL.Repositories.Interfaces;
using DAL.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories
{
    public class MenuItemRepository : GenericRepository<MenuItem>, IMenuItemRepository
    {
     

        public MenuItemRepository(AppDbContext context) : base(context)
        {
            
        }

        public async Task<IEnumerable<MenuItem>> GetAllWithCategoryAsync()
        {
            return await _context.MenuItems.Include(m => m.Category).ToListAsync();
        }

        public async Task<MenuItem> GetByIdWithCategoryAsync(int id)
        {
            return await _context.MenuItems
                .Include(m => m.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
        }
    }
}
