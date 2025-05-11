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

        // Ticket information (read-only)
        [Display(Name = "Biển Số Xe")]
        public string BienSoXe { get; set; }

        [Display(Name = "Vị Trí Gửi")]
        public string ViTriGui { get; set; }

        [Display(Name = "Ngày Đăng Ký")]
        public DateTime NgayDangKy { get; set; }

        [Display(Name = "Ngày Hết Hạn")]
        public DateTime NgayHetHan { get; set; }

        [Display(Name = "Giá Vé")]
        public decimal Price { get; set; }

        // Staff specific fields
        [Display(Name = "Địa Chỉ")]
        public string? DiaChi { get; set; }
    }
} 