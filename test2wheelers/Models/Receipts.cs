using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace _2whealers.Models
{
    public class Receipts
    {
        public int Id { get; set; }
          
        [Required(ErrorMessage = "Customer Name is required")]
        [StringLength(100, ErrorMessage = "Customer Name cannot exceed 100 characters")]
        [Display(Name = "Customer Name")]
        public string CustomerName { get; set; }

        [Required(ErrorMessage = "Mobile Number is required")]
        [RegularExpression(@"^[6-9]\d{9}$", ErrorMessage = "Enter a valid 10-digit mobile number")]
        [Display(Name = "Mobile Number")]
        public string MobileNo { get; set; }

        [Required(ErrorMessage = "Please select Brands")]
        public int BrandsId { get; set; }

        [Required(ErrorMessage = "Please select Model")]
        public int ModelId { get; set; }

        [Required(ErrorMessage = "Amount is required")]
        [Range(1, 1000000, ErrorMessage = "Amount must be between 1 and 10,00,000")]
        [DataType(DataType.Currency)]
        [Display(Name = "Amount")]
        public decimal Amount { get; set; }


        [Required(ErrorMessage = "Receipt Date is required")]
        [DataType(DataType.Date)]
        [Display(Name = "Receipt Date")]
        public DateTime? ReceiptDate { get; set; }

        [Required(ErrorMessage = "Payment Mode is required")]
        [StringLength(20, ErrorMessage = "Payment Mode cannot exceed 20 characters")]
        [Display(Name = "Payment Mode")]
        public string PaymentMode { get; set; }
        public string? ModelName { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; } = true;

    }
}
