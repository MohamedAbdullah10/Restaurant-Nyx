using DAL.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace PL.ViewModels.Orders
{
    public class OrderStatusUpdateViewModel
    {
        public int OrderId { get; set; }

        [Display(Name = "Current Status")]
        public OrderStatus CurrentStatus { get; set; }

        [Required(ErrorMessage = "New Status is required.")]
        [Display(Name = "New Status")]
        public OrderStatus NewStatus { get; set; }

        public IEnumerable<SelectListItem> AvailableStatuses { get; set; } // Dropdown list for status
    }
}
