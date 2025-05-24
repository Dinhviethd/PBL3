using System.ComponentModel.DataAnnotations;

namespace PBL3.Models.ViewModel
{
    public class EditStudentViewModel
    {
        public string Id { get; set; } = string.Empty;

        [Required(ErrorMessage = "Họ tên là bắt buộc")]
        [Display(Name = "Họ và tên")]
        public string HoTen { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [Display(Name = "Số điện thoại")]
        public string SDT { get; set; } = string.Empty;

        [Required(ErrorMessage = "MSSV là bắt buộc")]
        [Display(Name = "MSSV")]
        public string MSSV { get; set; } = string.Empty;

        [Required(ErrorMessage = "Lớp là bắt buộc")]
        [Display(Name = "Lớp")]
        public string Lop { get; set; } = string.Empty;
    }
}