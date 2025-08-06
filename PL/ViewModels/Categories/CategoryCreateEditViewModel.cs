using System.ComponentModel.DataAnnotations;

namespace PL.ViewModels.Categories
{
    public class CategoryCreateEditViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Category Name is required.")]
        [StringLength(100, ErrorMessage = "Category Name cannot exceed 100 characters.")]
        [Display(Name = "Category Name")]
        public string Name { get; set; }
    }
}
