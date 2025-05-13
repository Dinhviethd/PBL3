using System.ComponentModel.DataAnnotations;

namespace PBL3.Models
{
    public class ParkingSlot
    {
        [Key]
        public int ParkingSlotId { get; set; }

        [Display(Name = "Ô Giữ Xe")]
        [StringLength(10)]
        [Required]
        public string SlotName { get; set; }

        [Display(Name = "Số Xe Hiện Tại")]
        public int CurrentCount { get; set; } = 0;

        [Display(Name = "Sức chứa tối đa")]
        public int MaxCapacity { get; set; } = 20;
        public virtual ICollection<Ticket> Tickets { get; set; }
    }
}
