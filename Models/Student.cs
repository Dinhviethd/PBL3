using System.ComponentModel.DataAnnotations;

namespace PBL3.Models
{
    public class Student
    {
        [Key] public int ID_HS { get; set; }

        [Required(ErrorMessage = "Tên học sinh là bắt buộc")]
        public string TenHS { get; set; }

        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [StringLength(15, ErrorMessage = "Số điện thoại không được vượt quá 15 ký tự")]
        public string SDT { get; set; }
    }
}
