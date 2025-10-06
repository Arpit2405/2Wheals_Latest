using System.ComponentModel.DataAnnotations;

namespace _2whealers.Models
{
    public class Login
    {
        [Required(ErrorMessage = "User Name is required")]
        [Display(Name = "User Name")]
        public string UserName { get; set; }
         
        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long")]
        public string Password { get; set; } 

        [Display(Name = "Remember Me")] 
        public bool RememberMe { get; set; }

        [Required(ErrorMessage = "Captcha is required")]
        [Display(Name = "Captcha")]
        public string CaptchaCode { get; set; }
        public string HiddenCaptcha { get; set; }
    }
}
