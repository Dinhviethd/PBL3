using Microsoft.AspNetCore.Identity;
namespace PBL3.Models
{
    public class AppUser : IdentityUser
    {
        public string HoTen { get; set; }
        public string Role { get; set; }

    }
}
