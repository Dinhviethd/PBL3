using System.ComponentModel.DataAnnotations;

namespace PBL3.Models.ViewModel
{
    public class VerifyEmailViewModel
    {
        [Required(ErrorMessage ="Email là bắt buộc")]
        [EmailAddress]
        public string Email { get; set; }
    }
}
