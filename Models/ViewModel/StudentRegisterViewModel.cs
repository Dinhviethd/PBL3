using System.ComponentModel.DataAnnotations;

namespace PBL3.Models.ViewModel
{
    public class RegisterStudentViewModel : RegisterViewModel
    {
        [Required(ErrorMessage = "MSSV là bắt buộc cho sinh viên")]
        [Display(Name = "Mã số sinh viên")]
        public string MSSV { get; set; }

        [Required(ErrorMessage = "Lớp là bắt buộc cho sinh viên")]
        [Display(Name = "Lớp")]
        public string Lop { get; set; }
    }
}
