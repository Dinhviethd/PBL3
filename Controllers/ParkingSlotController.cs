using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PBL3.Data;
using PBL3.Models;
using PBL3.Models.ViewModel;
using System.Linq;
using System.Threading.Tasks;

namespace PBL3.Controllers
{
    public class ParkingSlotController : Controller
    {
        private readonly AppDBContext _context;

        public ParkingSlotController(AppDBContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string searchTerm, string searchType)
        {
            var viewModel = new ParkingSlotViewModel
            {
                SearchTerm = searchTerm,
                SearchType = searchType,
                ParkingSlots = await _context.ParkingSlots.ToListAsync()
            };

            var query = _context.Tickets
                .Include(t => t.Student)
                .Include(t => t.ParkingSlot)
                .AsQueryable();

            // First apply ParkingSlotId filter
            if (searchType == "not-entered")
            {
                query = query.Where(t => t.ParkingSlotId == null);
            }
            else
            {
                // For all other search types, only show tickets that are assigned to parking slots
                query = query.Where(t => t.ParkingSlotId != null);

                // Then apply search term filters for non-"not-entered" searches
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    switch (searchType)
                    {
                        case "mssv":
                            query = query.Where(t => t.Student.MSSV.Contains(searchTerm));
                            break;
                        case "name":
                            query = query.Where(t => t.Student.HoTen.Contains(searchTerm));
                            break;
                        case "license":
                            query = query.Where(t => t.BienSoXe.Contains(searchTerm));
                            break;
                        case "entry-date":
                            query = query.Where(t => t.NgayDangKy.ToString().Contains(searchTerm));
                            break;
                        case "exit-date":
                            query = query.Where(t => t.NgayHetHan.ToString().Contains(searchTerm));
                            break;
                    }
                }
            }

            viewModel.Tickets = await query.ToListAsync();
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteTickets(int[] ticketIds)
        {
            if (ticketIds != null && ticketIds.Length > 0)
            {
                var tickets = await _context.Tickets
                    .Include(t => t.ParkingSlot)
                    .Where(t => ticketIds.Contains(t.ID_Ticket))
                    .ToListAsync();

                // Get affected parking slots before unassigning tickets
                var slotsToUpdate = tickets
                    .Select(t => t.ParkingSlot)
                    .Where(s => s != null)
                    .GroupBy(s => s!.ParkingSlotId)
                    .ToDictionary(g => g.Key, g => g.Count());

                // Update the count for each affected slot
                foreach (var slot in slotsToUpdate)
                {
                    var parkingSlot = await _context.ParkingSlots.FindAsync(slot.Key);
                    if (parkingSlot != null)
                    {
                        parkingSlot.CurrentCount = Math.Max(0, parkingSlot.CurrentCount - slot.Value);
                    }
                }

                // Unassign tickets by setting ParkingSlotId to null
                foreach (var ticket in tickets)
                {
                    ticket.ParkingSlotId = null;
                }

                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> AssignTickets([FromBody] AssignTicketsRequest request)
        {
            try
            {
                var parkingSlot = await _context.ParkingSlots.FindAsync(request.ParkingSlotId);
                if (parkingSlot == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy ô giữ xe" });
                }

                var tickets = await _context.Tickets
                    .Where(t => request.TicketIds.Contains(t.ID_Ticket))
                    .ToListAsync();

                if (parkingSlot.CurrentCount + tickets.Count > parkingSlot.MaxCapacity)
                {
                    return Json(new { success = false, message = "Số lượng xe vượt quá sức chứa của ô giữ xe" });
                }

                foreach (var ticket in tickets)
                {
                    ticket.ParkingSlotId = request.ParkingSlotId;
                }

                parkingSlot.CurrentCount += tickets.Count;
                await _context.SaveChangesAsync();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetTicketsBySlot(int slotId)
        {
            var tickets = await _context.Tickets
                .Include(t => t.Student)
                .Include(t => t.ParkingSlot)
                .Where(t => t.ParkingSlotId == slotId)
                .ToListAsync();

            return Json(new { 
                success = true, 
                data = tickets.Select(t => new {
                    id = t.ID_Ticket,
                    studentName = t.Student?.HoTen,
                    studentId = t.Student?.MSSV,
                    licensePlate = t.BienSoXe,
                    entryDate = t.NgayDangKy.ToString("dd/MM/yyyy"),
                    exitDate = t.NgayHetHan.ToString("dd/MM/yyyy")
                })
            });
        }
    
        public class AssignTicketsRequest
        {
            public required int[] TicketIds { get; set; }
            public int ParkingSlotId { get; set; }
        }

        public class UpdateTicketRequest
        {
            public int TicketId { get; set; }
            public required string StudentName { get; set; }
            public required string StudentId { get; set; }
            public required string LicensePlate { get; set; }
            public required string EntryDate { get; set; }
            public required string ExitDate { get; set; }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateTicket([FromBody] UpdateTicketRequest request)
        {
            try
            {
                var ticket = await _context.Tickets
                    .Include(t => t.Student)
                    .FirstOrDefaultAsync(t => t.ID_Ticket == request.TicketId);

                if (ticket == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy vé" });
                }

                // Update student information
                if (ticket.Student != null)
                {
                    ticket.Student.HoTen = request.StudentName;
                    ticket.Student.MSSV = request.StudentId;
                }

                // Update ticket information 
                ticket.BienSoXe = request.LicensePlate;
                ticket.NgayDangKy = DateTime.ParseExact(request.EntryDate, "dd/MM/yyyy", null);
                ticket.NgayHetHan = DateTime.ParseExact(request.ExitDate, "dd/MM/yyyy", null);

                await _context.SaveChangesAsync();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
            }
        }
    }
}
