using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace PL.ViewModels.MenuItems
{
    public class MenuItemCreateEditViewModel
    {
        public int Id { get; set; } 

        [Required(ErrorMessage = "Item Name is required.")]
        [StringLength(100, ErrorMessage = "Item Name cannot exceed 100 characters.")]
        [Display(Name = "Item Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Price is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be positive.")]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        [Display(Name = "Is Available?")]
        public bool IsAvailable { get; set; } = true;

        [Required(ErrorMessage = "Preparation Time is required.")]
        [Range(1, 120, ErrorMessage = "Preparation Time must be between 1 and 120 minutes.")]
        [Display(Name = "Preparation Time (min)")]
        public int PreparationTime { get; set; }

        [Required(ErrorMessage = "Category is required.")]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }


        public IEnumerable<SelectListItem> Categories { get; set; }
    }
}
