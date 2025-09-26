using System.ComponentModel.DataAnnotations;

namespace _2whealers.Models
{
    public class RegionModel
    {
        public int Id { get; set; }
        public int RegionId { get; set; }


        [Required(ErrorMessage = "Region Name is required.")]
        [StringLength(100, ErrorMessage = "Region Name cannot be longer than 100 characters.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Region Name can only contain letters and spaces.")]
        public string RegionName { get; set; }

        [Required(ErrorMessage = "Address Line 1 is required.")]
        [StringLength(100, ErrorMessage = "Address Line 1 cannot be longer than 100 characters.")]
        public string AddressLine1 { get; set; }

        [StringLength(100, ErrorMessage = "Address Line 2 cannot be longer than 100 characters.")]
        public string AddressLine2 { get; set; }

        [Required(ErrorMessage = "Mobile number is required")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Mobile number must be 10 digits")]
        public string MobileNo { get; set; }
         
        [RegularExpression(@".+\.(jpg|png)$", ErrorMessage = "Region Logo must end with .jpg or .png")]
        public string? RegionLogo { get; set; }
        public bool IsActive { get; set; }
    }
}
