using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace PBL3.Models
{
    public class Ticket
    {
        [Key] public int ID_Ticket { get; set; }

        [Required(ErrorMessage = "Ngày đăng ký là bắt buộc")]
        public DateTime NgayDangKy { get; set; }

        [Required(ErrorMessage = "Ngày hết hạn là bắt buộc")]
        public DateTime NgayHetHan { get; set; }
    }
}
