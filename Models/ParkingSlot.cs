using System.ComponentModel.DataAnnotations;

namespace PBL3.Models
{
    public class ParkingSlot
    {
        [Key]
        public int ParkingSlotId { get; set; }
        [StringLength(10)]
        [Required]
        public string SlotName { get; set; }
        [Display(Name = "Số xe hiện tại")]
        public int CurrentCount { get; set; } = 0;
        [Display(Name = "Sức chứa tối đa")]
        public int MaxCapacity { get; set; } = 10;
        public virtual ICollection<Ticket> Tickets { get; set; }
    }
}
