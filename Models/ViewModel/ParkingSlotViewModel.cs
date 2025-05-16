using PBL3.Models;
using System.Collections.Generic;

namespace PBL3.Models.ViewModel
{
    public class ParkingSlotViewModel
    {
        public List<Ticket> Tickets { get; set; }
        public string SearchTerm { get; set; }
        public string SearchType { get; set; }
        public List<ParkingSlot> ParkingSlots { get; set; }
        public bool IsStaff { get; set; }
    }
}
