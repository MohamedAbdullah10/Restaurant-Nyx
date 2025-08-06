using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    public interface IDailyMenuItemActivityRepository : IGenericRepository<DailyMenuItemActivity>
    {
        Task<DailyMenuItemActivity> GetActivityByMenuItemAndDateAsync(int menuItemId, DateTime date);
        // ممكن تضيف ميثود لزيادة العداد مباشرةً
        Task IncrementOrderCountAsync(int menuItemId, DateTime date);
    }
}