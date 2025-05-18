using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PBL3.Models
{
    public class History
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string TrangThai { get; set; }  // "check-in" or "check-out"

        [Required]
        public string BienSo { get; set; }

        [Required]
        public string MSSV { get; set; }

        [Required]
        public string TenSinhVien { get; set; }

        [Required]
        public string Lop { get; set; }

        [Required]
        public DateTime ThoiGian { get; set; }

        public string KhuVuc { get; set; }  // ParkingSlot.SlotName

        [ForeignKey("Ticket")]
        public int TicketId { get; set; }
        public virtual Ticket Ticket { get; set; }
    }
}
