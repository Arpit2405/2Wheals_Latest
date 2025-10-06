using System.ComponentModel.DataAnnotations;

namespace _2whealers.Models
{
    public class UserModel
    {
        public Guid? Id { get; set; }
        [Required(ErrorMessage = "Username is required")]
        [RegularExpression(@"^\S+$", ErrorMessage = "Username cannot contain spaces")]
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Phone number is required")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Phone number must be 10 digits")]
        public string PhoneNumber { get; set; }
        public string ProfileImage { get; set; }

        public string PasswordHash { get; set; } // plain for create/edit
        public string PasswordSalt { get; set; }
        public string DisplayName { get; set; }
        public int? RoleId { get; set; }
        public bool Approved { get; set; }
        public bool IsActive { get; set; }

        public string RoleName { get; set; }


        //Only For Update Profile
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }


        //For Regions
        public int? RegionId { get; set; }
        public string RegionName { get; set; }
    }
}
