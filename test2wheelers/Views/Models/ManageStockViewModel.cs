using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace _2whealers.Models
{
    public class ManageStockViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Please select a brand.")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid brand.")]
        public int BrandsId { get; set; }

        [Required(ErrorMessage = "Please select a model.")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid model.")]
        public int ModelId { get; set; }

        [Required(ErrorMessage = "Quantity is required.")]
        [Range(1, 1000, ErrorMessage = "Quantity must be at least 1 and max 1000.")]
        public int Quantity { get; set; }

        public DateTime LastUpdated { get; set; } = DateTime.Now;
         
        public string? BrandName { get; set; }
        public string? ModelName { get; set; }

        public bool Status { get; set; }
    }
}
