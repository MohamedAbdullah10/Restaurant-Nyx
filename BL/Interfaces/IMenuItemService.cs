using DAL.Models; 

namespace BLL.Interfaces
{
    public interface IMenuItemService
    {
        Task<IEnumerable<MenuItem>> GetAllMenuItemsAsync();
        Task<MenuItem> GetMenuItemByIdAsync(int id);
        Task AddMenuItemAsync(MenuItem item);
        Task UpdateMenuItemAsync(MenuItem item);
        Task DeleteMenuItemAsync(int id);
        Task<IEnumerable<MenuItem>> GetAvailableMenuItemsAsync();
        Task<IEnumerable<Category>> GetMenuCategoriesWithAvailableItemsAsync();
        Task MarkItemUnavailableIfExceedsDailyOrdersAsync(int menuItemId);      
        Task ResetDailyAvailabilityAtMidnight();
        Task<bool> IsMenuItemAvailableForOrder(int menuItemId);
    }
}