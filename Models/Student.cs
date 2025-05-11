using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
namespace PBL3.Models
{
    public class Student:AppUser
    {
        [Required(ErrorMessage ="MSSV là bắt buộc")]
        public string MSSV { get; set; }

        [Required(ErrorMessage = "Lớp là bắt buộc")]
        public string Lop { get; set; }  

        // Navigation property cho quan hệ one-to-many với Ticket
        public virtual ICollection<Ticket> Tickets { get; set; }
    }
}