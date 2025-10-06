using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace _2whealers.Models
{
    public class Brands
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Brand name is required")]
        [Display(Name = "Brand Name")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Only alphabets and spaces are allowed")]
        public string BrandName { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}