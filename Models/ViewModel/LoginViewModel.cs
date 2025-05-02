using System.ComponentModel.DataAnnotations;

namespace PBL3.Models.ViewModel
{
    public class LoginViewModel
    {
        [Required(ErrorMessage ="Email là bắt buộc")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage ="Mật khẩu là bắt buộc")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }
}
