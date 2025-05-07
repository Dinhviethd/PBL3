using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
namespace PBL3.Models
{
    public class Student
    {
        public string UserId { get; set; } = null!;
        public AppUser User { get; set; } = null!;
        [Required(ErrorMessage ="MSSV là bắt buộc")]
        public string MSSV { get; set; } = null!;

        [Required(ErrorMessage = "Lớp là bắt buộc")]
        public string Lop { get; set; } = string.Empty;

        public bool DKyVe { get; set; } = false;  // đăng ký vé (mặc định false)

        public Ticket Tickets { get; set; } = null!;
    }
}