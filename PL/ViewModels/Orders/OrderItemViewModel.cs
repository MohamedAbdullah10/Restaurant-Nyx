using System.ComponentModel.DataAnnotations;

namespace PL.ViewModels.Orders
{
    public class OrderItemViewModel
    {
        public int MenuItemId { get; set; }

        [Display(Name = "Item Name")]
        public string MenuItemName { get; set; }

        public int Quantity { get; set; }

        [DataType(DataType.Currency)]
        public decimal UnitPrice { get; set; }

        [DataType(DataType.Currency)]
        public decimal Subtotal { get; set; }
    }
}
