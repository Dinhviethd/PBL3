using System.ComponentModel.DataAnnotations;
using PBL3.Models;

namespace PBL3.Models.ViewModel
{
    public class StaffViewModel : RegisterViewModel
    {
        [Required(ErrorMessage = "Địa chỉ là bắt buộc cho nhân viên")]
        [Display(Name = "Địa chỉ")]
        public string DiaChi { get; set; }

        public IEnumerable<Staff> Staffs { get; set; }
        public PageInfo PageInfo { get; set; }
    }
} 