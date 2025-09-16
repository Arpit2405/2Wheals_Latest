using Microsoft.AspNetCore.Mvc.Rendering;

namespace _2whealers.Models
{
    public class ManageStockViewModel
    {
        public int BranchId { get; set; }
        public int BrandsId { get; set; }
        public int ModelId { get; set; }
        public int Quantity { get; set; }  
        public DateTime LastUpdated { get; set; }  
    }
}
