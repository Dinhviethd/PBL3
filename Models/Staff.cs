using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PBL3.Models
{
    public class Staff 
    {
        [Key]
        public string Id { get; set; }
        public string UserId { get; set; } = null!;
        public AppUser User { get; set; } = null!;
        [Required(ErrorMessage = "Địa chỉ là bắt buộc")]
        [StringLength(200, ErrorMessage = "Địa chỉ không được vượt quá 200 ký tự")]
        public string DiaChi { get; set; } = null!;
    }

}
