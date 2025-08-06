using Microsoft.AspNetCore.Mvc;
using BLL.Interfaces;
using PL.ViewModels.Categories;
using DAL.Models;

namespace PL.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public async Task<IActionResult> Index()
        {
            var categories = await _categoryService.GetCategoriesWithActiveMenuItemsAsync();
            var categoryViewModels = categories.Select(c => new CategoryViewModel
            {
                Id = c.Id,
                Name = c.Name,
                ActiveItemsCount = c.MenuItems?.Count(m => m.IsAvailable) ?? 0
            }).ToList();

            return View(categoryViewModels);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _categoryService.GetCategoryByIdAsync(id.Value);
            if (category == null)
            {
                return NotFound();
            }

            var categoryViewModel = new CategoryViewModel
            {
                Id = category.Id,
                Name = category.Name,
                ActiveItemsCount = category.MenuItems?.Count(m => m.IsAvailable) ?? 0
            };

            return View(categoryViewModel);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name")] CategoryCreateEditViewModel categoryViewModel)
        {
            if (ModelState.IsValid)
            {
                var category = new Category
                {
                    Name = categoryViewModel.Name
                };

                await _categoryService.AddCategoryAsync(category);
                return RedirectToAction(nameof(Index));
            }

            return View(categoryViewModel);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _categoryService.GetCategoryByIdAsync(id.Value);
            if (category == null)
            {
                return NotFound();
            }

            var categoryViewModel = new CategoryCreateEditViewModel
            {
                Id = category.Id,
                Name = category.Name
            };

            return View(categoryViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] CategoryCreateEditViewModel categoryViewModel)
        {
            if (id != categoryViewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var category = new Category
                    {
                        Id = categoryViewModel.Id,
                        Name = categoryViewModel.Name
                    };

                    await _categoryService.UpdateCategoryAsync(category);
                }
                catch (KeyNotFoundException)
                {
                    return NotFound();
                }
                catch (Exception)
                {
                    throw;
                }

                return RedirectToAction(nameof(Index));
            }

            return View(categoryViewModel);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _categoryService.GetCategoryByIdAsync(id.Value);
            if (category == null)
            {
                return NotFound();
            }

            var categoryViewModel = new CategoryViewModel
            {
                Id = category.Id,
                Name = category.Name
            };

            return View(categoryViewModel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _categoryService.DeleteCategoryAsync(id);
            }
            catch (KeyNotFoundException)
            {
                // Category not found - we'll just redirect to index
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", $"Cannot delete category: {ex.Message}");

                var category = await _categoryService.GetCategoryByIdAsync(id);
                var categoryViewModel = new CategoryViewModel
                {
                    Id = category.Id,
                    Name = category.Name
                };

                return View("Delete", categoryViewModel);
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> ActiveCategories()
        {
            var categories = await _categoryService.GetCategoriesWithActiveMenuItemsAsync();
            var categoryViewModels = categories.Select(c => new CategoryViewModel
            {
                Id = c.Id,
                Name = c.Name,
                ActiveItemsCount = c.MenuItems?.Count(m => m.IsAvailable) ?? 0
            }).ToList();

            return View("Index", categoryViewModels);
        }
    }
}