using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public enum OrderStatus
    {
        Pending,
        Preparing,
        Ready,
        Delivered,
        Cancelled
    }

    public enum OrderType
    {
        DineIn,
        TakeOut,
        Delivery
    }

    public class Order : ModelBase
    {
        public DateTime OrderDate { get; set; } = DateTime.Now; 
        public OrderStatus Status { get; set; } = OrderStatus.Pending; 
        public OrderType Type { get; set; }

        public string DeliveryAddress { get; set; } 

        [Range(0.0, double.MaxValue)] 
        public decimal TotalAmount { get; set; }

        [Range(0.0, double.MaxValue)] 
        public decimal DiscountAmount { get; set; }

        [Range(0.0, double.MaxValue)]  
        public decimal TaxAmount { get; set; }


        public DateTime? EstimatedDeliveryTime { get; set; }

        public ICollection<OrderItem> OrderItems { get; set; } = new HashSet<OrderItem>();
    }
}