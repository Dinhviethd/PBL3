using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace PBL3.Models
{
    public class Ticket
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Biển số xe là bắt buộc")]
        [StringLength(20, ErrorMessage = "Biển số xe tối đa 20 ký tự")]
        public string BienSoXe { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vị trí gửi là bắt buộc")]
        public int ViTriGui { get; set; } 

        [Required(ErrorMessage = "Ngày đăng ký là bắt buộc")]
        public DateTime NgayDangKy { get; set; }

        [Required(ErrorMessage = "Ngày hết hạn là bắt buộc")]
        public DateTime NgayHetHan { get; set; }

        public string StudentId { get; set; } = null!;
        public Student Student { get; set; } = null!;
    }
}