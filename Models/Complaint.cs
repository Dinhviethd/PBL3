using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PBL3.Models
{
    public class Complaint
    {
        [Key]
        public int ComplaintId { get; set; }
        
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public AppUser User { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        public DateTime CreatedAt { get; set; }

        public ComplaintStatus Status { get; set; }
    }

    public enum ComplaintStatus
    {
        Pending,
        InProgress,
        Resolved,
        Rejected
    }
}
