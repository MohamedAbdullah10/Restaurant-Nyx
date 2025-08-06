using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    public interface IMenuItemRepository : IGenericRepository<MenuItem>
    {
        Task<IEnumerable<MenuItem>> GetAllWithCategoryAsync();
        Task<MenuItem> GetByIdWithCategoryAsync(int id);
    }
}
