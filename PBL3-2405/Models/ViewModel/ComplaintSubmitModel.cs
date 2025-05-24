using System.ComponentModel.DataAnnotations;

namespace PBL3.Models.ViewModel
{
    public class ComplaintSubmitModel
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }
    }
}
