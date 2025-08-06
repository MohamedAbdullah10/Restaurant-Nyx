using DAL.Models;
using System.ComponentModel.DataAnnotations;

namespace PL.ViewModels.Orders
{
    public class OrderCreateViewModel: IValidatableObject
    {
        [Required(ErrorMessage = "Order Type is required.")]
        [Display(Name = "Order Type")]
        public OrderType Type { get; set; }

        [Display(Name = "Delivery Address")]
        [StringLength(200, ErrorMessage = "Delivery Address cannot exceed 200 characters.")]
    
        public string DeliveryAddress { get; set; }

        [Required(ErrorMessage = "At least one item is required.")]
        public List<OrderItemInputViewModel> OrderItems { get; set; } = new List<OrderItemInputViewModel>();

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Type == OrderType.Delivery && string.IsNullOrWhiteSpace(DeliveryAddress))
            {
                yield return new ValidationResult(
                    "Delivery Address is required for Delivery orders.",
                    new[] { nameof(DeliveryAddress) });
            }
        }
    }

    public class OrderItemInputViewModel
    {
        [Required(ErrorMessage = "Menu Item is required.")]
        [Display(Name = "Menu Item")]
        public int MenuItemId { get; set; }

        [Required(ErrorMessage = "Quantity is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        public int Quantity { get; set; }
    }
}
