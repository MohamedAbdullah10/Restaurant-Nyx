using Microsoft.AspNetCore.Mvc;
using BLL.Interfaces;
using PL.ViewModels.MenuItems; using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging; 
namespace PL.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;         private readonly IMenuItemService _menuItemService; 
        public HomeController(ILogger<HomeController> logger, IMenuItemService menuItemService)         {
            _logger = logger;
            _menuItemService = menuItemService;         }

        public async Task<IActionResult> Index()
        {
            var featuredMenuItems = await _menuItemService.GetAvailableMenuItemsAsync();
            var model = featuredMenuItems.Select(item => new MenuItemViewModel
            {
                Id = item.Id,
                Name = item.Name,
                Price = item.Price,
                IsAvailable = item.IsAvailable,
                PreparationTime = item.PreparationTime,
                CategoryName = item.Category?.Name
            }).ToList();

            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

       
    }
}