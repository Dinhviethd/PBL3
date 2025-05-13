using System.ComponentModel.DataAnnotations;

namespace PBL3.Models.ViewModel
{
    public class TicketViewModel
    {
        [Display(Name = "Họ Tên")]
        public string HoTen { get; set; }

        [Display(Name = "MSSV")]
        public string MSSV { get; set; }

        [Display(Name = "Lớp")]
        public string Lop { get; set; }

        [Required(ErrorMessage = "Biển số xe là bắt buộc")]
        [Display(Name = "Biển Số Xe")]
        public string BienSoXe { get; set; }

        [Required(ErrorMessage = "Gói đăng ký là bắt buộc")]
        [Display(Name = "Gói Đăng Ký")]
        public string PackageName { get; set; }

        [Required(ErrorMessage = "Giá vé là bắt buộc")]
        [Display(Name = "Giá Vé")]
        public decimal Price { get; set; }
    }
}