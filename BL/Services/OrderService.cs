using BLL.Interfaces;
using DAL.Models;
using DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMenuItemService _menuItemService; 

     
        private const decimal TAX_RATE = 0.085m; 
        private const decimal HAPPY_HOUR_DISCOUNT = 0.20m;
        private const decimal BULK_DISCOUNT = 0.10m; 
        private const decimal BULK_DISCOUNT_THRESHOLD = 100m; 

        public OrderService(IUnitOfWork unitOfWork, IMenuItemService menuItemService)
        {
            _unitOfWork = unitOfWork;
            _menuItemService = menuItemService;
        }

        public async Task<Order> CreateOrderAsync(Order order, List<OrderItem> orderItems)
        {
            if (order == null || orderItems == null || !orderItems.Any())
            {
                throw new ArgumentException("Order and order items cannot be null or empty.");
            }

            int maxPreparationTime = 0; 

           
            foreach (var item in orderItems)
            {
                var menuItem = await _menuItemService.GetMenuItemByIdAsync(item.MenuItemId);
              

                if (menuItem == null || !menuItem.IsAvailable)
                {
                    throw new InvalidOperationException($"Menu item '{item.MenuItemId}' is not available.");
                }

                if (!await _menuItemService.IsMenuItemAvailableForOrder(item.MenuItemId))
                {
                    throw new InvalidOperationException($"Menu item '{menuItem.Name}' has exceeded daily order limit or is temporarily unavailable.");
                }

                item.UnitPrice = menuItem.Price;
                item.Subtotal = item.Quantity * item.UnitPrice;

               
                if (menuItem.PreparationTime > maxPreparationTime)
                {
                    maxPreparationTime = menuItem.PreparationTime;
                }
               
            }

           
            order.OrderItems = orderItems;
            await _unitOfWork.OrderRepository.AddAsync(order);

            
            foreach (var item in orderItems)
            {
                await _unitOfWork.DailyMenuItemActivityRepository.IncrementOrderCountAsync(item.MenuItemId, DateTime.Today);
            }

         
            order.TotalAmount = order.OrderItems.Sum(oi => oi.Subtotal);
            order.TaxAmount = order.TotalAmount * TAX_RATE;
            order.TotalAmount += order.TaxAmount;

           
            await ApplyDiscountsAsync(order);

           
            order.Status = OrderStatus.Pending;
            order.OrderDate = DateTime.Now;

            if (order.Type == OrderType.Delivery && string.IsNullOrWhiteSpace(order.DeliveryAddress))
            {
                throw new ArgumentException("Delivery address is required for delivery orders.");
            }

            
            order.EstimatedDeliveryTime = DateTime.Now.AddMinutes(maxPreparationTime + 30);
           

            
            await _unitOfWork.CompleteAsync();

           
            _ = AutoProgressOrderStatusAsync(order.Id);

            return order;
        }
        public async Task<IEnumerable<Order>> GetAllOrdersAsync()
        {
            return await _unitOfWork.OrderRepository.GetAllWithItemsAndMenuItemsAsync();
        }

        public async Task<Order> GetOrderByIdAsync(int id)
        {
            return await _unitOfWork.OrderRepository.GetByIdWithItemsAndMenuItemsAsync(id);
        }

        public async Task UpdateOrderAsync(Order order)
        {
            var existingOrder = await _unitOfWork.OrderRepository.GetByIdAsync(order.Id);
            if (existingOrder == null)
            {
                throw new KeyNotFoundException($"Order with ID {order.Id} not found.");
            }

            
            existingOrder.Type = order.Type;
            existingOrder.DeliveryAddress = order.DeliveryAddress;
            

            await _unitOfWork.OrderRepository.UpdateAsync(existingOrder);
            await _unitOfWork.CompleteAsync();
        }

        public async Task<bool> CancelOrderAsync(int orderId)
        {
            var order = await _unitOfWork.OrderRepository.GetByIdAsync(orderId);
            if (order == null) return false;

            if (order.Status == OrderStatus.Ready || order.Status == OrderStatus.Delivered)
            {
               
                throw new InvalidOperationException($"Cannot cancel order in '{order.Status}' status.");
            }

            order.Status = OrderStatus.Cancelled;
            await _unitOfWork.OrderRepository.UpdateAsync(order);
            await _unitOfWork.CompleteAsync();
            return true;
        }

        public async Task<bool> UpdateOrderStatusAsync(int orderId, OrderStatus newStatus)
        {
            var order = await _unitOfWork.OrderRepository.GetByIdAsync(orderId);
            if (order == null) return false;

          

            order.Status = newStatus;
            await _unitOfWork.OrderRepository.UpdateAsync(order);
            await _unitOfWork.CompleteAsync();

          

            return true;
        }

        public async Task ApplyDiscountsAsync(Order order)
        {
            
            order.TotalAmount -= order.DiscountAmount; 
            order.DiscountAmount = 0;

            var originalSubtotal = order.OrderItems.Sum(oi => oi.Subtotal);
            decimal discountApplied = 0;

            
            var now = DateTime.Now.TimeOfDay;
            var happyHourStart = new TimeSpan(15, 0, 0); 
            var happyHourEnd = new TimeSpan(17, 0, 0);   

            if (now >= happyHourStart && now <= happyHourEnd)
            {
                discountApplied += originalSubtotal * HAPPY_HOUR_DISCOUNT;
            }

           
            if (originalSubtotal >= BULK_DISCOUNT_THRESHOLD)
            {
                discountApplied += originalSubtotal * BULK_DISCOUNT;
            }

         
            order.DiscountAmount = discountApplied;
            order.TotalAmount -= discountApplied; 
         
            order.TaxAmount = (originalSubtotal - discountApplied) * TAX_RATE;
            order.TotalAmount = (originalSubtotal - discountApplied) + order.TaxAmount;

            
        }

        public async Task RecalculateOrderTotalsAsync(int orderId)
        {
            var order = await _unitOfWork.OrderRepository.GetByIdWithItemsAndMenuItemsAsync(orderId);
            if (order == null)
            {
                throw new KeyNotFoundException($"Order with ID {orderId} not found.");
            }

           
            foreach (var item in order.OrderItems)
            {
                
                item.Subtotal = item.Quantity * item.UnitPrice;
            }
            order.TotalAmount = order.OrderItems.Sum(oi => oi.Subtotal);

           
            order.TaxAmount = order.TotalAmount * TAX_RATE;
            order.TotalAmount += order.TaxAmount;
            await ApplyDiscountsAsync(order); 

            await _unitOfWork.OrderRepository.UpdateAsync(order); 
            await _unitOfWork.CompleteAsync(); 
        }

        public async Task AutoProgressOrderStatusAsync(int orderId)
        {
            var order = await _unitOfWork.OrderRepository.GetByIdWithItemsAndMenuItemsAsync(orderId);
            if (order == null || order.Status != OrderStatus.Pending) return;

           
            await Task.Delay(TimeSpan.FromMinutes(5));
            if (order.Status == OrderStatus.Pending) 
            {
                order.Status = OrderStatus.Preparing;
                await _unitOfWork.OrderRepository.UpdateAsync(order);
                await _unitOfWork.CompleteAsync();
               
            }

            
            if (order.Status == OrderStatus.Preparing) 
            {
               
                var maxPrepTime = order.OrderItems.Max(oi => oi.MenuItem.PreparationTime);
                await Task.Delay(TimeSpan.FromMinutes(maxPrepTime));

                if (order.Status == OrderStatus.Preparing) 
                {
                    order.Status = OrderStatus.Ready;
                    await _unitOfWork.OrderRepository.UpdateAsync(order);
                    await _unitOfWork.CompleteAsync();
                    
                }
            }
           
        }

        
        public async Task ResetDailyAvailabilityAtMidnight()
        {
           
            var activities = await _unitOfWork.DailyMenuItemActivityRepository.GetAllAsync();
            foreach (var activity in activities)
            {
                if (activity.ActivityDate.Date < DateTime.Today.Date) 
                {
                   
                }
            }

        
            var allMenuItems = await _unitOfWork.MenuItemRepository.GetAllAsync();
            foreach (var item in allMenuItems)
            {
          
                var yesterdayActivity = await _unitOfWork.DailyMenuItemActivityRepository.GetActivityByMenuItemAndDateAsync(item.Id, DateTime.Today.AddDays(-1));
                if (item.IsAvailable == false && yesterdayActivity != null && yesterdayActivity.OrderCount > 50)
                {
                    item.IsAvailable = true;
                    await _unitOfWork.MenuItemRepository.UpdateAsync(item);
                }
            }
            await _unitOfWork.CompleteAsync();
        }
    }
}