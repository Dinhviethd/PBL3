using System.ComponentModel.DataAnnotations;

namespace PBL3.Models.ViewModel
{
    public class StaffViewModel: RegisterViewModel
    {
   

        [Required(ErrorMessage = "Địa chỉ là bắt buộc")]
        [StringLength(200, ErrorMessage = "Địa chỉ không được vượt quá 200 ký tự")]
        public string DiaChi { get; set; }
    }
}
