using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PBL3.Models
{
    public class VehicleInfo
    {
        [Required(ErrorMessage = "ID là bắt buộc")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Biển số xe là bắt buộc")]
        public string BienSo { get; set; }      // Biển số xe

        [Required(ErrorMessage = "ID chủ xe là bắt buộc")]
        public string IdChu { get; set; }       // ID chủ xe
        
        [Required(ErrorMessage = "Họ và tên chủ xe là bắt buộc")]
        public string TenChu { get; set; }      // Họ và tên chủ xe

        [Required(ErrorMessage = "Ngày hết hạn là bắt buộc")]
        public string HetHan { get; set; }      // Ngày hết hạn

        [Required(ErrorMessage = "Ngày ra là bắt buộc")]
        public string NgayRa { get; set; }      // Ngày ra

        [Required(ErrorMessage = "Ô giữ xe là bắt buộc")]
        public int OgiuXe { get; set; }         // Ô giữ xe
    }
}
