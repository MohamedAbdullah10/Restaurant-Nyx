using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class DailyMenuItemActivity : ModelBase
    {
       
        public int MenuItemId { get; set; }
        public MenuItem MenuItem { get; set; }

      
        [DataType(DataType.Date)]
        public DateTime ActivityDate { get; set; } = DateTime.Today; 
     
        [Range(0, int.MaxValue)]
        public int OrderCount { get; set; }
    }
}