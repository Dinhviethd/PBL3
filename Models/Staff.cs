using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace PBL3.Models
{
    public class Staff:AppUser
    {

        [Required(ErrorMessage = "Địa chỉ là bắt buộc")]
        [StringLength(200, ErrorMessage = "Địa chỉ không được vượt quá 200 ký tự")]
        public string DiaChi { get; set; }
    }

}
