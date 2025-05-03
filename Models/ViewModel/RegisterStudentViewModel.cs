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

        public IEnumerable<Student> Students { get; set; }
        public PageInfo PageInfo { get; set; }
    }
    public class PageInfo
    {
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int TotalItems { get; set; }
        public int ItemsPerPage { get; set; }
        public int StartRecord { get; set; }
        public int EndRecord { get; set; }
    }
}
