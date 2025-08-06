using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
       
        IMenuItemRepository MenuItemRepository { get; }
        ICategoryRepository CategoryRepository { get; }
        IOrderRepository OrderRepository { get; }   
        IOrderItemRepository OrderItemRepository { get; } 
        IDailyMenuItemActivityRepository DailyMenuItemActivityRepository { get; }
        Task<int> CompleteAsync(); 
    }
}