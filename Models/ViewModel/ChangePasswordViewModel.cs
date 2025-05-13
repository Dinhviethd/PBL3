using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace PBL3.Models.ViewModel
{
    public class ChangePasswordViewModel
    {
        [Required (ErrorMessage ="Email là bắt buộc")]
        [EmailAddress]
        public string Email { get; set; }

        [Required (ErrorMessage ="Mật khẩu là bắt buộc")]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu mới")]
        public string NewPassword {get; set;}

        [Required (ErrorMessage ="Mật Khẩu xác thực là bắt buộc")]
        [Compare("ConfirmNewPassword", ErrorMessage = "Password không khớp")]
        [DataType(DataType.Password)]
        [Display(Name = "Xác thực mật khẩu mới")]
        public string ConfirmNewPassword { get; set; }

    }
}
