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
    public class DailyMenuItemActivityRepository : GenericRepository<DailyMenuItemActivity>, IDailyMenuItemActivityRepository
    {
        public DailyMenuItemActivityRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<DailyMenuItemActivity> GetActivityByMenuItemAndDateAsync(int menuItemId, DateTime date)
        {
            return await _context.DailyMenuItemActivities
                                 .FirstOrDefaultAsync(d => d.MenuItemId == menuItemId && d.ActivityDate.Date == date.Date);
        }

        public async Task IncrementOrderCountAsync(int menuItemId, DateTime date)
        {
            var activity = await GetActivityByMenuItemAndDateAsync(menuItemId, date.Date);

            if (activity == null)
            {
                // لو مفيش نشاط لليوم ده، أنشئ واحد جديد
                activity = new DailyMenuItemActivity
                {
                    MenuItemId = menuItemId,
                    ActivityDate = date.Date,
                    OrderCount = 1
                };
                await _dbSet.AddAsync(activity);
            }
            else
            {
                // لو فيه نشاط، زود العداد
                activity.OrderCount++;
                _dbSet.Update(activity);
            }
            // لاحظ أن SaveChangesAsync() مش هنا، هتكون في الـ Unit of Work أو الـ BLL Service
        }
    }
}