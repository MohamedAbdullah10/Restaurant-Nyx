using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class MenuItem:ModelBase
    {
       
        [Required , MaxLength(100) ]
        public string Name { get; set; }

        [Required, Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }

        public bool IsAvailable { get; set; } = true;

        public int PreparationTime { get; set; } 

        
        public int CategoryId { get; set; }

        
        public Category Category { get; set; }
    }
}
