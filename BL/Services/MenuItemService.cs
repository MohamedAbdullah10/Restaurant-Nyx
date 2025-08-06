
using BLL.Interfaces;
using DAL.Models;
using DAL.Repositories.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.Logging;

namespace BLL.Services
{
    public class MenuItemService : IMenuItemService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<MenuItemService> _logger;
        public MenuItemService(IUnitOfWork unitOfWork, ILogger<MenuItemService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<IEnumerable<MenuItem>> GetAllMenuItemsAsync()
        {
            return await _unitOfWork.MenuItemRepository.GetAllWithCategoryAsync();
        }

        public async Task<MenuItem> GetMenuItemByIdAsync(int id)
        {
            return await _unitOfWork.MenuItemRepository.GetByIdWithCategoryAsync(id);
        }

        public async Task AddMenuItemAsync(MenuItem item)
        {
            if (item.Price <= 0)
            {
                throw new ArgumentException("Item price must be positive.");
            }
            await _unitOfWork.MenuItemRepository.AddAsync(item);
            await _unitOfWork.CompleteAsync();
        }

        public async Task UpdateMenuItemAsync(MenuItem item)
        {
            if (item.Price <= 0)
            {
                throw new ArgumentException("Item price must be positive.");
            }
            var existingItem = await _unitOfWork.MenuItemRepository.GetByIdAsync(item.Id);
            if (existingItem == null)
            {
                throw new KeyNotFoundException($"MenuItem with ID {item.Id} not found.");
            }

            existingItem.Name = item.Name;
            existingItem.Price = item.Price;
            existingItem.IsAvailable = item.IsAvailable;
            existingItem.PreparationTime = item.PreparationTime;
            existingItem.CategoryId = item.CategoryId;

            await _unitOfWork.MenuItemRepository.UpdateAsync(existingItem);
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteMenuItemAsync(int id)
        {
            await _unitOfWork.MenuItemRepository.DeleteAsync(id);
            await _unitOfWork.CompleteAsync();
        }

        public async Task<IEnumerable<MenuItem>> GetAvailableMenuItemsAsync()
        {
            var allItems = await _unitOfWork.MenuItemRepository.GetAllWithCategoryAsync();

            return allItems.Where(item => item.IsAvailable);
        }

        public async Task<IEnumerable<Category>> GetMenuCategoriesWithAvailableItemsAsync()
        {
            return await _unitOfWork.CategoryRepository.GetCategoriesWithAvailableItemsAsync();
        }

        public async Task MarkItemUnavailableIfExceedsDailyOrdersAsync(int menuItemId)
        {
            
            var today = DateTime.Today;
            var activity = await _unitOfWork.DailyMenuItemActivityRepository.GetActivityByMenuItemAndDateAsync(menuItemId, today);

            if (activity != null && activity.OrderCount > 50)
            {
                var item = await _unitOfWork.MenuItemRepository.GetByIdAsync(menuItemId);
                if (item != null && item.IsAvailable)
                {
                    item.IsAvailable = false;
                    await _unitOfWork.MenuItemRepository.UpdateAsync(item);
                    await _unitOfWork.CompleteAsync();
                }
            }
           
        }

        public async Task<bool> IsMenuItemAvailableForOrder(int menuItemId)
        {
            var item = await _unitOfWork.MenuItemRepository.GetByIdAsync(menuItemId);
           
            var today = DateTime.Today;
            var activity = await _unitOfWork.DailyMenuItemActivityRepository.GetActivityByMenuItemAndDateAsync(menuItemId, today);

            
            if (item == null || !item.IsAvailable || (activity != null && activity.OrderCount > 50))
            {
                return false;
            }
            return true;
        }

        public async Task ResetDailyAvailabilityAtMidnight()
        {
            _logger.LogInformation("Attempting to reset daily availability for menu items.");

            var allMenuItems = await _unitOfWork.MenuItemRepository.GetAllAsync();

            foreach (var item in allMenuItems)
            {
                var yesterdayActivity = await _unitOfWork.DailyMenuItemActivityRepository
                                                          .GetActivityByMenuItemAndDateAsync(item.Id, DateTime.Today.AddDays(-1));

                if (item.IsAvailable == false && yesterdayActivity != null && yesterdayActivity.OrderCount > 50)
                {
                    item.IsAvailable = true;
                    await _unitOfWork.MenuItemRepository.UpdateAsync(item);
                    _logger.LogInformation("Menu item {MenuItemName} (ID: {MenuItemId}) availability reset to true.", item.Name, item.Id);
                }
            }
            await _unitOfWork.CompleteAsync();
            _logger.LogInformation("Daily availability reset process completed for all relevant items.");
        }
    }
}