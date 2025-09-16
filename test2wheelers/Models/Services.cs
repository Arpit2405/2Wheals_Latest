using System.ComponentModel.DataAnnotations;

namespace _2whealers.Models
{
    public class Services:CommonVm
    {

        public int Id { get; set; }

        public int UserId_fk { get; set; }
        public int BrandId_Fk { get; set; }
        public int ModelId_Fk { get; set; }

        public string? ModelName { get; set; }


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

        public DateTime ManufacturingYear { get; set; }

        [Required(ErrorMessage = "Registration Number is required")]
        [StringLength(20, ErrorMessage = "Registration No cannot exceed 20 characters.")]
        [RegularExpression(@"^[A-Za-z]{2}[0-9]{1,2}[A-Za-z]{1,3}[0-9]{1,4}$",
       ErrorMessage = "Enter valid Registration No (e.g. MH12AB1234 or DL2CAE3422)")]
        public string RegistrationNumber { get; set; }


        [Required(ErrorMessage = "Engine number is required")]
        [StringLength(20, MinimumLength = 5, ErrorMessage = "Engine number must be between 5 and 30 characters")]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*[0-9])[A-Za-z0-9]+$", ErrorMessage = "Engine number must contain both letters and numbers")]
        public string EngineNo { get; set; }

        [Required(ErrorMessage = "Chassis number is required")]
        [StringLength(20, MinimumLength = 5, ErrorMessage = "Chassis number must be between 5 and 30 characters")]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z0-9]+$", ErrorMessage = "Must contain letters and numbers")]
        public string ChassisNo { get; set; }


        [Required(ErrorMessage = "Daily Running Mileage is required")]
        [Range(1, 500, ErrorMessage = "Daily running mileage must be between 1 and 500 KM")]
        public int DRM { get; set; }

        public DateTime DateOfServicing { get; set; }
        public string SerivceDetails { get; set; }


        [Required(ErrorMessage = "Meter Reading (K.M) is required")]
        [Range(1, 999999, ErrorMessage = "Enter a valid numeric meter reading between 1 and 999999")]
        public int MeterReading { get; set; }
        public int? Amount { get; set; }

        [Required(ErrorMessage = "Date of Service Reminder is required")]
        [DataType(DataType.Date)]
        public DateTime? DateOfServiceReminder { get; set; }


        [Required(ErrorMessage = "Insurance Due Date is required")]
        [Display(Name = "Insurance Due Date")]
        [DataType(DataType.Date)]
        public DateTime? InsuranceDuDate { get; set; }
        public string Notes { get; set; }
        public string CustomerComplaints { get; set; }
         
        public bool RegularService { get; set; }
        public bool WashingPolishing { get; set; }
        public bool Brake { get; set; }
        public bool SparkPlug { get; set; }
        public bool ChainServices { get; set; }
        public bool Engine { get; set; }
        public bool ElectricalIssues { get; set; }
        public bool Suspension { get; set; }
        public bool Clutch { get; set; }
        public bool Accident { get; set; }
        public bool OilChange { get; set; }
        public bool RunningRepair { get; set; }
        public bool Gear { get; set; }
        public bool PaintRepair { get; set; }
        public bool Accessories { get; set; }
        public bool GeneralCheckup { get; set; }
        public bool Complaints { get; set; }
        public bool Filters { get; set; }
        public bool Labour { get; set; }
        public bool FlushingTreatement { get; set; }
        public bool Petrol { get; set; }
        public bool SpareParts { get; set; }

        public DateTime? CreatedDate { get; set; }
        public bool IsActive { get; set; }
    }
} 
