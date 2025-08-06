using System.ComponentModel.DataAnnotations;

namespace PL.ViewModels.Categories
{
    public class CategoryViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Category Name")]
        public string Name { get; set; }

        [Display(Name = "Active Items Count")]
        public int ActiveItemsCount { get; set; }
    }
}
