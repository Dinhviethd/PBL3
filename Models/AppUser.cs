using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
namespace PBL3.Models
{
    public class AppUser : IdentityUser
    {
        [Required(ErrorMessage = "Họ tên là bắt buộc")]
        public string HoTen { get; set; } = string.Empty;

        [Required(ErrorMessage = "Role là bắt buộc")]
        public string Role { get; set; } = string.Empty;

    }
}
