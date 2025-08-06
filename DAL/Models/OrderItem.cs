using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class OrderItem : ModelBase
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        public int Quantity { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Unit Price must be positive.")]
        public decimal UnitPrice { get; set; } 

        [Range(0.0, double.MaxValue)]
        public decimal Subtotal { get; set; } 


        public int MenuItemId { get; set; }
        public MenuItem MenuItem { get; set; }

     
        public int OrderId { get; set; }
        public Order Order { get; set; }
    }
}