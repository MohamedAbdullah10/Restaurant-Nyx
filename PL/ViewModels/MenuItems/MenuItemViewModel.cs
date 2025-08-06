using System.ComponentModel.DataAnnotations;
using DAL.Models;
namespace PL.ViewModels.MenuItems
{
        public class MenuItemViewModel
        {
            public int Id { get; set; }

            [Display(Name = "Item Name")]
            public string Name { get; set; }

            [DataType(DataType.Currency)]
            public decimal Price { get; set; }

            [Display(Name = "Available")]
            public bool IsAvailable { get; set; }

            [Display(Name = "Preparation Time (min)")]
            public int PreparationTime { get; set; }

            [Display(Name = "Category")]
            public string CategoryName { get; set; }
        }
    
}
