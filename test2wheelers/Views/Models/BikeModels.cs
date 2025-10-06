using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web; 

namespace _2whealers.Models
{
    public class BikeModels
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Brand name is required")]
        [Display(Name = "Brand Name")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Only alphabets and spaces are allowed")]
        public string ModelName { get; set; }
        public int Brand_Id_Fk { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }

        public string BrandName { get; set; }
        public List<SelectListItem> BrandList { get; set; } = new List<SelectListItem>();
    }
}