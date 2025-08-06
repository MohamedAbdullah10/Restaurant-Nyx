using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using BLL.Interfaces;
using PL.ViewModels.Orders;
using PL.ViewModels.MenuItems;
using DAL.Models;

namespace PL.Controllers
{
    public class OrdersController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly IMenuItemService _menuItemService;

        public OrdersController(IOrderService orderService, IMenuItemService menuItemService)
        {
            _orderService = orderService;
            _menuItemService = menuItemService;
        }

        public async Task<IActionResult> Index()
        {
            var orders = await _orderService.GetAllOrdersAsync();
            var orderViewModels = orders.Select(o => new OrderViewModel
            {
                Id = o.Id,
                OrderDate = o.OrderDate,
                Status = o.Status,
                Type = o.Type,
                DeliveryAddress = o.DeliveryAddress,
                TotalAmount = o.TotalAmount,
                DiscountAmount = o.DiscountAmount,
                TaxAmount = o.TaxAmount,
                OrderItems = o.OrderItems.Select(oi => new OrderItemViewModel
                {
                    MenuItemId = oi.MenuItemId,
                    MenuItemName = oi.MenuItem?.Name,
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice,
                    Subtotal = oi.Subtotal
                }).ToList()
            }).ToList();

            return View(orderViewModels);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _orderService.GetOrderByIdAsync(id.Value);
            if (order == null)
            {
                return NotFound();
            }

            var orderViewModel = new OrderViewModel
            {
                Id = order.Id,
                OrderDate = order.OrderDate,
                Status = order.Status,
                Type = order.Type,
                DeliveryAddress = order.DeliveryAddress,
                TotalAmount = order.TotalAmount,
                DiscountAmount = order.DiscountAmount,
                EstimatedDeliveryTime = order.EstimatedDeliveryTime,
                TaxAmount = order.TaxAmount,
                OrderItems = order.OrderItems.Select(oi => new OrderItemViewModel
                {
                    MenuItemId = oi.MenuItemId,
                    MenuItemName = oi.MenuItem?.Name,
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice,
                    Subtotal = oi.Subtotal
                }).ToList()
            };

            return View(orderViewModel);
        }

        public async Task<IActionResult> Create()
        {
            var availableMenuItems = await _menuItemService.GetAvailableMenuItemsAsync();

            ViewBag.MenuItems = availableMenuItems.Select(item => new SelectListItem
            {
                Value = item.Id.ToString(),
                Text = $"{item.Name} - {item.Price:C} ({item.PreparationTime} min)"
            }).ToList();

            var model = new OrderCreateViewModel();
            model.OrderItems.Add(new OrderItemInputViewModel());

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(OrderCreateViewModel orderViewModel)
        {
            var availableMenuItems = await _menuItemService.GetAvailableMenuItemsAsync();
            ViewBag.MenuItems = availableMenuItems.Select(item => new SelectListItem
            {
                Value = item.Id.ToString(),
                Text = $"{item.Name} - {item.Price:C} ({item.PreparationTime} min)"
            }).ToList();

            // Filter out invalid order items
            orderViewModel.OrderItems = orderViewModel.OrderItems
                .Where(oi => oi.MenuItemId > 0 && oi.Quantity > 0)
                .ToList();

            if (!orderViewModel.OrderItems.Any())
            {
                ModelState.AddModelError(string.Empty, "At least one menu item is required to create an order.");
            }

            if (ModelState.IsValid)
            {
                var order = new Order
                {
                    Type = orderViewModel.Type,
                    DeliveryAddress = (orderViewModel.Type == OrderType.Delivery) ?
                        orderViewModel.DeliveryAddress : null,
                };

                var orderItems = orderViewModel.OrderItems.Select(oi => new OrderItem
                {
                    MenuItemId = oi.MenuItemId,
                    Quantity = oi.Quantity,
                }).ToList();

                try
                {
                    await _orderService.CreateOrderAsync(order, orderItems);
                    return RedirectToAction(nameof(Index));
                }
                catch (ArgumentException ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
                catch (InvalidOperationException ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
                catch (Exception)
                {
                    ModelState.AddModelError(string.Empty,
                        "An unexpected error occurred while placing your order. Please try again.");
                }
            }

            return View(orderViewModel);
        }

        public async Task<IActionResult> UpdateStatus(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _orderService.GetOrderByIdAsync(id.Value);
            if (order == null)
            {
                return NotFound();
            }

            var model = new OrderStatusUpdateViewModel
            {
                OrderId = order.Id,
                CurrentStatus = order.Status,
                NewStatus = order.Status,
                AvailableStatuses = Enum.GetValues(typeof(OrderStatus))
                    .Cast<OrderStatus>()
                    .Select(s => new SelectListItem
                    {
                        Text = s.ToString(),
                        Value = s.ToString(),
                        Selected = s == order.Status
                    })
                    .ToList()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, OrderStatusUpdateViewModel model)
        {
            if (id != model.OrderId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    bool success = await _orderService.UpdateOrderStatusAsync(model.OrderId, model.NewStatus);
                    if (success)
                    {
                        TempData["Message"] = $"Order {model.OrderId} status updated to {model.NewStatus}.";
                        return RedirectToAction(nameof(Details), new { id = model.OrderId });
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty,
                            "Failed to update order status. Please check the order details.");
                    }
                }
                catch (InvalidOperationException ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
                catch (KeyNotFoundException)
                {
                    return NotFound();
                }
                catch (Exception)
                {
                    ModelState.AddModelError(string.Empty,
                        "An unexpected error occurred while updating order status.");
                }
            }

            model.AvailableStatuses = Enum.GetValues(typeof(OrderStatus))
                .Cast<OrderStatus>()
                .Select(s => new SelectListItem
                {
                    Text = s.ToString(),
                    Value = s.ToString(),
                    Selected = s == model.NewStatus
                })
                .ToList();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelOrder(int id)
        {
            try
            {
                bool success = await _orderService.CancelOrderAsync(id);
                if (success)
                {
                    TempData["Message"] = $"Order {id} has been cancelled successfully.";
                    return RedirectToAction(nameof(Details), new { id = id });
                }
                else
                {
                    TempData["ErrorMessage"] = $"Could not cancel order {id}.";
                    return RedirectToAction(nameof(Details), new { id = id });
                }
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = $"Cancellation failed: {ex.Message}";
                return RedirectToAction(nameof(Details), new { id = id });
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "An unexpected error occurred during cancellation. Please try again.";
                return RedirectToAction(nameof(Details), new { id = id });
            }
        }
    }
}