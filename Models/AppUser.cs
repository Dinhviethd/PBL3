using Microsoft.AspNetCore.Identity;
namespace PBL3.Models
{
    public class AppUser : IdentityUser
    {
        public string HoTen { get; set; }
        public string Role { get; set; }

        // Thông tin dành cho Staff
        public string DiaChi { get; set; }

        // Thông tin dành cho Student
        public string MSSV { get; set; }
        public string Lop { get; set; }
        public bool DKyVe { get; set; } = false;
    }
}
