using Microsoft.AspNetCore.Mvc;
using BLL.Interfaces;
using PL.ViewModels.MenuItems;
using DAL.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace PL.Controllers
{
    public class MenuItemsController : Controller
    {
        private readonly IMenuItemService _menuItemService;
        private readonly ICategoryService _categoryService;

        public MenuItemsController(IMenuItemService menuItemService, ICategoryService categoryService)
        {
            _menuItemService = menuItemService;
            _categoryService = categoryService;
        }

        private async Task PopulateCategoriesDropDownList(int? selectedCategory = null)
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            var categoryListItems = categories.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name,
                Selected = c.Id == selectedCategory
            }).ToList();

            categoryListItems.Insert(0, new SelectListItem
            {
                Value = "",
                Text = "-- Select a Category --",
                Selected = selectedCategory == null || selectedCategory == 0
            });

            ViewBag.Categories = new SelectList(categoryListItems, "Value", "Text", selectedCategory);
        }

        public async Task<IActionResult> Index()
        {
            var menuItems = await _menuItemService.GetAllMenuItemsAsync();
            var menuItemViewModels = menuItems.Select(item => new MenuItemViewModel
            {
                Id = item.Id,
                Name = item.Name,
                Price = item.Price,
                IsAvailable = item.IsAvailable,
                PreparationTime = item.PreparationTime,
                CategoryName = item.Category?.Name
            }).ToList();

            return View(menuItemViewModels);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var menuItem = await _menuItemService.GetMenuItemByIdAsync(id.Value);
            if (menuItem == null)
            {
                return NotFound();
            }

            var menuItemViewModel = new MenuItemViewModel
            {
                Id = menuItem.Id,
                Name = menuItem.Name,
                Price = menuItem.Price,
                IsAvailable = menuItem.IsAvailable,
                PreparationTime = menuItem.PreparationTime,
                CategoryName = menuItem.Category?.Name
            };

            return View(menuItemViewModel);
        }

        public async Task<IActionResult> Create()
        {
            await PopulateCategoriesDropDownList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Price,IsAvailable,PreparationTime,CategoryId")] MenuItemCreateEditViewModel menuItemViewModel)
        {
            if (ModelState.IsValid)
            {
                var menuItem = new MenuItem
                {
                    Name = menuItemViewModel.Name,
                    Price = menuItemViewModel.Price,
                    IsAvailable = menuItemViewModel.IsAvailable,
                    PreparationTime = menuItemViewModel.PreparationTime,
                    CategoryId = menuItemViewModel.CategoryId
                };

                try
                {
                    await _menuItemService.AddMenuItemAsync(menuItem);
                    return RedirectToAction(nameof(Index));
                }
                catch (ArgumentException ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                    await PopulateCategoriesDropDownList(menuItemViewModel.CategoryId);
                    return View(menuItemViewModel);
                }
            }

            await PopulateCategoriesDropDownList(menuItemViewModel.CategoryId);
            return View(menuItemViewModel);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var menuItem = await _menuItemService.GetMenuItemByIdAsync(id.Value);
            if (menuItem == null)
            {
                return NotFound();
            }

            var menuItemViewModel = new MenuItemCreateEditViewModel
            {
                Id = menuItem.Id,
                Name = menuItem.Name,
                Price = menuItem.Price,
                IsAvailable = menuItem.IsAvailable,
                PreparationTime = menuItem.PreparationTime,
                CategoryId = menuItem.CategoryId
            };

            await PopulateCategoriesDropDownList(menuItemViewModel.CategoryId);
            return View(menuItemViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Price,IsAvailable,PreparationTime,CategoryId")] MenuItemCreateEditViewModel menuItemViewModel)
        {
            if (id != menuItemViewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var menuItem = new MenuItem
                    {
                        Id = menuItemViewModel.Id,
                        Name = menuItemViewModel.Name,
                        Price = menuItemViewModel.Price,
                        IsAvailable = menuItemViewModel.IsAvailable,
                        PreparationTime = menuItemViewModel.PreparationTime,
                        CategoryId = menuItemViewModel.CategoryId
                    };

                    await _menuItemService.UpdateMenuItemAsync(menuItem);
                }
                catch (KeyNotFoundException)
                {
                    return NotFound();
                }
                catch (ArgumentException ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                    await PopulateCategoriesDropDownList(menuItemViewModel.CategoryId);
                    return View(menuItemViewModel);
                }
                catch (Exception)
                {
                    throw;
                }

                return RedirectToAction(nameof(Index));
            }

            await PopulateCategoriesDropDownList(menuItemViewModel.CategoryId);
            return View(menuItemViewModel);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var menuItem = await _menuItemService.GetMenuItemByIdAsync(id.Value);
            if (menuItem == null)
            {
                return NotFound();
            }

            var menuItemViewModel = new MenuItemViewModel
            {
                Id = menuItem.Id,
                Name = menuItem.Name,
                Price = menuItem.Price,
                IsAvailable = menuItem.IsAvailable,
                PreparationTime = menuItem.PreparationTime,
                CategoryName = menuItem.Category?.Name
            };

            return View(menuItemViewModel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _menuItemService.DeleteMenuItemAsync(id);
            }
            catch (KeyNotFoundException)
            {
                
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Could not delete menu item: {ex.Message}");
                var menuItem = await _menuItemService.GetMenuItemByIdAsync(id);
                var menuItemViewModel = new MenuItemViewModel
                {
                    Id = menuItem.Id,
                    Name = menuItem.Name,
                    Price = menuItem.Price,
                    IsAvailable = menuItem.IsAvailable,
                    PreparationTime = menuItem.PreparationTime,
                    CategoryName = menuItem.Category?.Name
                };
                return View("Delete", menuItemViewModel);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}