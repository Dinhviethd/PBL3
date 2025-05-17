using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace PBL3.Models
{
    public class Ticket
    {
        [Key]
        public int ID_Ticket { get; set; }

        [Required(ErrorMessage = "Biển số xe là bắt buộc")]
        [StringLength(20, ErrorMessage = "Biển số xe tối đa 20 ký tự")]
        public string BienSoXe { get; set; }  

        [Required(ErrorMessage = "Ngày đăng ký là bắt buộc")]
        public DateTime NgayDangKy { get; set; }

        [Required(ErrorMessage = "Ngày hết hạn là bắt buộc")]
        public DateTime NgayHetHan { get; set; }

        [Display(Name = "Giá vé")]
        public decimal Price { get; set; }

        public DateTime? ThoiGianVao { get; set; }
        public DateTime? ThoiGianRa { get; set; }

        [ForeignKey("Student")]
        public string StudentId { get; set; }
        public virtual Student Student { get; set; }

        [ForeignKey("ParkingSlot")]
        public int? ParkingSlotId { get; set; }
        public virtual ParkingSlot ParkingSlot { get; set; } //Chỉ chứa trong 1 ParkingSlot
    }
}