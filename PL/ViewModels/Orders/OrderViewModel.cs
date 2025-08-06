using DAL.Models;
using System.ComponentModel.DataAnnotations;

namespace PL.ViewModels.Orders
{
    public class OrderViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Order Date")]
        public DateTime OrderDate { get; set; }

        public OrderStatus Status { get; set; }

        public OrderType Type { get; set; }

        [Display(Name = "Delivery Address")]
        public string DeliveryAddress { get; set; }

        [DataType(DataType.Currency)]
        public decimal TotalAmount { get; set; }

        [DataType(DataType.Currency)]
        public decimal DiscountAmount { get; set; }

        [DataType(DataType.Currency)]
        public decimal TaxAmount { get; set; }
        [Display(Name = "Estimated Delivery")]
        public DateTime? EstimatedDeliveryTime { get; set; }

        // List of items in the order
        public ICollection<OrderItemViewModel> OrderItems { get; set; } = new List<OrderItemViewModel>();
    }
}
