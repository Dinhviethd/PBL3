using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
namespace PBL3.Models
{
    public class Student:IdentityUser
    {
        [Key]
        public int ID_SV { get; set; }  

        [Required(ErrorMessage = "Họ tên SV là bắt buộc")]
        public string HoTen { get; set; } 

        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string SDT { get; set; }

        [Required(ErrorMessage = "Lớp là bắt buộc")]
        public string Lop { get; set; }  

        public bool DKyVe { get; set; } = false;  // đăng ký vé (mặc định false)
    }
}