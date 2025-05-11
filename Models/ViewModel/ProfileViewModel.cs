using System.ComponentModel.DataAnnotations;

namespace PBL3.Models.ViewModel
{
    public class ProfileViewModel
    {
        [Required(ErrorMessage = "Họ tên là bắt buộc")]
        [Display(Name = "Họ Tên")]
        public string HoTen { get; set; }

        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [Display(Name = "Số Điện Thoại")]
        public string SDT { get; set; }

        [Display(Name = "Vai Trò")]
        public string Role { get; set; }

        // Student specific fields
        [Display(Name = "Mã Số Sinh Viên")]
        public string MSSV { get; set; }

        [Display(Name = "Lớp")]
        public string Lop { get; set; }

        // Staff specific fields
        [Display(Name = "Địa Chỉ")]
        public string DiaChi { get; set; }
    }
} 