using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace PBL3.Models
{
    public class Registered_Student : Student
    {
        [Required(ErrorMessage = "Số tháng đăng ký là bắt buộc")][Range(1, int.MaxValue, ErrorMessage = "Số tháng đăng ký phải lớn hơn 0")] public int SoThangDangKi { get; set; }

        [Required(ErrorMessage = "Số tiền đã nộp là bắt buộc")]
        [Range(0, float.MaxValue, ErrorMessage = "Số tiền đã nộp không được âm")]
        public float SoTienDaNop { get; set; }

        [Required(ErrorMessage = "Biển số xe là bắt buộc")]
        [StringLength(20, ErrorMessage = "Biển số xe không được vượt quá 20 ký tự")]
        public string BienSoXe { get; set; }
    }
}
