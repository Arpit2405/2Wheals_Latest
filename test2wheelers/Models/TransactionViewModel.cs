using _2whealers.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace _2whealers.Models
{
    public class PurchaseViewModel : CommonVm
    {
        [Required(ErrorMessage = "Registration Number is required")]
        [StringLength(20, ErrorMessage = "Registration No cannot exceed 20 characters.")]
        [RegularExpression(@"^[A-Za-z]{2}[0-9]{1,2}[A-Za-z]{1,3}[0-9]{1,4}$",
         ErrorMessage = "Enter valid Registration No (e.g. MH12AB1234 or DL2CAE3422)")]
        public string RegNo { get; set; }
        public bool InsuranceValid { get; set; }

        public bool NocValid { get; set; }

        //Region Details
        public string RegionLogo { get; set; }
        public string RegionName { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string MobileNo { get; set; }

    }

    public class CommonVm
    {

        public int Id { get; set; }

        [Required(ErrorMessage = "Select a bike")]
        [Display(Name = "Select Bike")]
        public int? BikeId { get; set; }

        [Required(ErrorMessage = "Select a model")]
        [Display(Name = "Select Model")]
        public int? ModelId { get; set; }
 
        public string? ModelName { get; set; }



        [Required(ErrorMessage = "Date of Sale  is required")]
        [DataType(DataType.Date)]
        public DateTime? dateofsale { get; set; }

         
        [Required(ErrorMessage = "Engine number is required")]
        [StringLength(20, MinimumLength = 5, ErrorMessage = "Engine number must be between 5 and 30 characters")]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*[0-9])[A-Za-z0-9]+$", ErrorMessage = "Engine number must contain both letters and numbers")]
        public string EngineNo { get; set; }

        [Required(ErrorMessage = "Chassis number is required")]
        [StringLength(20, MinimumLength = 5, ErrorMessage = "Chassis number must be between 5 and 30 characters")]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z0-9]+$", ErrorMessage = "Must contain letters and numbers")]
        public string ChassisNo { get; set; }
 
        public string? Notes { get; set; }


        [Required(ErrorMessage = "Financed By is required")]
        [StringLength(50, ErrorMessage = "Financed By cannot exceed 50 characters.")]
        public string FinancedBy { get; set; }


        [Required(ErrorMessage = "Cash amount is required")]
        [Range(100, 10000000, ErrorMessage = "Cash amount must be between 100 and 1 crore")]
        [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "Cash amount can have up to 2 decimal places.")]
        public decimal CashAmount { get; set; }

        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }

        [Required(ErrorMessage = "Mobile number is required")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Mobile number must be 10 digits")]
        [RegularExpression("^[0-9]{10}$", ErrorMessage = "Enter a valid 10-digit number")]
        [Display(Name = "Mobile No")]
        public string MobileNo { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(30, ErrorMessage = "Name cannot be longer than 50 characters")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Name can contain only letters and spaces")]
        [Display(Name = "Name")]
        public string Name { get; set; }


   
        [EmailAddress(ErrorMessage = "Enter a valid email address")]
        [Display(Name = "Email")]
        public string?Email { get; set; }


        [Required(ErrorMessage = "Address 1 is required")]
        [Display(Name = "Address 1")]
        public string Address1 { get; set; }

        [Display(Name = "Address 2")]
        public string? Address2 { get; set; }





    }
    public class TransactionViewModel : CommonVm
    {
        public string RegNo { get; set; } = "A/F";

        [Display(Name = "Manufacturing Year")]
        public string? ManufacturingYear { get; set; }


        [Required(ErrorMessage = "Daily Running Mileage is required")]
        [Range(1, 500, ErrorMessage = "Daily running mileage must be between 1 and 500 KM")]
        public int DRM { get; set; }

        [Required(ErrorMessage = "Meter Reading (K.M) is required")]
        [Range(1, 999999, ErrorMessage = "Enter a valid numeric meter reading between 1 and 999999")]
        public int MeterReading { get; set; }

        [Required(ErrorMessage = "Date of Sale Reminder is required")]
        [DataType(DataType.Date)]
        public DateTime? DateOfSaleReminder { get; set; } 

        [Required(ErrorMessage = "Regular Service is required")]
        public bool IsRegularService { get; set; }

        [Required(ErrorMessage = "Insurance Due Date is required")]
        [Display(Name = "Insurance Due Date")]
        [DataType(DataType.Date)]
        public DateTime? InsuranceDuDate { get; set; }



        [Required(ErrorMessage = "Downpayment is required")]
        [Range(1000, 1000000, ErrorMessage = "Downpayment must be between 1,000 and 10 lakh")]
        [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "Downpayment can have up to 2 decimal places.")]
        public decimal Downpayment { get; set; }


        [Required(ErrorMessage = "No Of Installments is required")]
        [Range(1, 120, ErrorMessage = "Installments must be between 1 and 120")]
        public int NoOfInstallments { get; set; }


        [Required(ErrorMessage = "Date Of Birth is required")]
        [Display(Name = "Date Of Birth")]
        [DataType(DataType.Date)]
        //[MinimumAge(18)]
        public DateTime? DateOfBirth { get; set; }


        [Required(ErrorMessage = "State is required")]
        public string State { get; set; }

        [Required(ErrorMessage = "City is required")]
        public string City { get; set; }

        [Required(ErrorMessage = "Pincode is required")]
        [RegularExpression(@"^\d{6}$", ErrorMessage = "Enter a valid 6-digit pincode")]
        public string Pincode { get; set; }

        [Required(ErrorMessage = "Location is required")]
        public string Location { get; set; }


       

    }
}
